using System.Globalization;
using System.Text;
using System.Xml;
using RackVisualization.Models;

namespace RackVisualization.Services;

public class SvgGenerator
{
    private const int DefaultRackHeightUnits = 42;
    private const int DefaultRackSpacingPoints = 25;
    private const int DefaultRackUnitPoints = 25;
    private const int DefaultRackWidthPoints = 300;
    private const int DefaultSvgMargin = 25;

    public string GenerateSvg(RackSet rackSet)
    {
        var maxRackHeight = rackSet.Racks.Count > 0 
            ? rackSet.Racks.Max(r => r.Height) 
            : DefaultRackHeightUnits;

        var rackCount = rackSet.Racks.Count;
        var svgHeight = (2 * DefaultSvgMargin) + (DefaultRackUnitPoints * maxRackHeight);
        var svgWidth = (2 * DefaultSvgMargin) + (rackCount * DefaultRackWidthPoints) + 
                       ((rackCount - 1) * DefaultRackSpacingPoints);

        var svg = new StringBuilder();
        svg.AppendLine($@"<svg baseProfile=""full"" height=""{svgHeight}"" version=""1.1"" width=""{svgWidth}"" xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"">");
        
        // Add CSS styles
        svg.AppendLine(@"<style>
            a:hover {
                filter: brightness(90%);
            }
        </style>");

        // Add empty pattern
        svg.AppendLine(GenerateEmptyPattern());

        var xOffset = DefaultSvgMargin;
        foreach (var rack in rackSet.Racks)
        {
            svg.AppendLine($@"<g transform=""translate({xOffset}, 0)"">");
            svg.AppendLine(GenerateRack(rack, rackSet.BaseHref));
            svg.AppendLine("</g>");

            xOffset += DefaultRackWidthPoints + DefaultRackSpacingPoints;
        }

        svg.AppendLine("</svg>");
        return svg.ToString();
    }

    private string GenerateEmptyPattern()
    {
        return @"<pattern id=""pattern-empty"" patternUnits=""userSpaceOnUse"" width=""25"" height=""25"">
            <path d=""M 0 12.5 L 25 12.5"" fill=""none"" stroke=""#ccc"" stroke-width=""1""/>
        </pattern>";
    }

    private string GenerateRack(Rack rack, string? baseHref)
    {
        var rackSvg = new StringBuilder();

        // Add rack name if present
        if (!string.IsNullOrEmpty(rack.Name))
        {
            var nameY = (DefaultSvgMargin / 2.0) + 4;
            rackSvg.AppendLine($@"<text x=""{DefaultRackWidthPoints / 2}"" y=""{nameY.ToString(CultureInfo.InvariantCulture)}"" text-anchor=""middle"" dominant-baseline=""central"" font-family=""sans-serif"">{XmlEscape(rack.Name)}</text>");
        }

        // Add rack background
        rackSvg.AppendLine($@"<rect x=""0"" y=""{DefaultSvgMargin}"" width=""{DefaultRackWidthPoints}"" height=""{rack.Height * DefaultRackUnitPoints}"" fill=""url(#pattern-empty)"" stroke=""black""/>");

        // Calculate device positions
        var rackBottomY = (rack.Height * DefaultRackUnitPoints) + DefaultSvgMargin;
        var currentPosition = 0;

        // Process devices from bottom to top (reverse order)
        var devices = rack.Devices.ToList();
        devices.Reverse();

        foreach (var device in devices)
        {
            var at = device.At.HasValue ? device.At.Value - 1 : currentPosition;
            var deviceY = rackBottomY - ((at + device.Height) * DefaultRackUnitPoints);

            rackSvg.AppendLine($@"<g transform=""translate(0, {deviceY})"">");
            rackSvg.AppendLine(GenerateDevice(device, baseHref));
            rackSvg.AppendLine("</g>");

            currentPosition = at + device.Height;
        }

        return rackSvg.ToString();
    }

    private string GenerateDevice(RackDevice device, string? baseHref)
    {
        var deviceHeight = device.Height * DefaultRackUnitPoints;
        var color = device.Color ?? ColorSchemes.GetColor(device.Type);

        var deviceSvg = new StringBuilder();

        // Device background rectangle
        deviceSvg.AppendLine($@"<rect x=""0"" y=""0"" width=""{DefaultRackWidthPoints}"" height=""{deviceHeight}"" fill=""{color}"" stroke=""black""/>");

        // Device text - better vertical centering
        if (!string.IsNullOrEmpty(device.Text))
        {
            // Calculate better vertical center position - add small offset for better visual centering
            var textY = (deviceHeight / 2.0) + 4;
            deviceSvg.AppendLine($@"<text x=""{DefaultRackWidthPoints / 2}"" y=""{textY.ToString(CultureInfo.InvariantCulture)}"" text-anchor=""middle"" dominant-baseline=""central"" font-family=""sans-serif"">{XmlEscape(device.Text)}</text>");
        }

        // Device symbols based on type
        switch (device.Type)
        {
            case "patch":
                // Patch panel ports (8 rectangles in 2 rows)
                var ports = DeviceSymbols.GetPatchPanelPorts();
                foreach (var (x, y, width, height) in ports)
                {
                    deviceSvg.AppendLine($@"<rect x=""{x}"" y=""{y}"" width=""{width}"" height=""{height}"" fill=""#555"" stroke=""none""/>");
                }
                break;

            case "pdu":
                // PDU symbol with specific stroke properties
                var pduPath = DeviceSymbols.GetSymbolPath("pdu");
                deviceSvg.AppendLine($@"<path d=""{pduPath}"" stroke=""#555"" fill=""none"" stroke-width=""2"" stroke-linecap=""round""/>");
                break;

            case "san":
                // SAN symbol with filled rectangles
                var sanPath = DeviceSymbols.GetSymbolPath("san");
                deviceSvg.AppendLine($@"<path d=""{sanPath}"" stroke=""none"" fill=""#555"" fill-rule=""evenodd""/>");
                break;

            case "server":
                // Server symbol with filled path
                var serverPath = DeviceSymbols.GetSymbolPath("server");
                deviceSvg.AppendLine($@"<path d=""{serverPath}"" stroke=""none"" fill=""#555"" fill-rule=""evenodd""/>");
                break;

            case "switch":
                // Switch symbol with filled arrows
                var switchPath = DeviceSymbols.GetSymbolPath("switch");
                deviceSvg.AppendLine($@"<path d=""{switchPath}"" fill=""#555"" stroke=""none""/>");
                break;

            case "tape":
                // Tape drive with background rectangle and two circles
                var (bgX, bgY, bgWidth, bgHeight) = DeviceSymbols.GetTapeBackground();
                deviceSvg.AppendLine($@"<rect x=""{bgX}"" y=""{bgY}"" width=""{bgWidth}"" height=""{bgHeight}"" fill=""#555"" stroke=""none""/>");
                
                var (cx1, cy1, r1) = DeviceSymbols.GetTapeDriveCircle1();
                var (cx2, cy2, r2) = DeviceSymbols.GetTapeDriveCircle2();
                deviceSvg.AppendLine($@"<circle cx=""{cx1}"" cy=""{cy1}"" r=""{r1}"" stroke=""#ccc"" fill=""#555"" stroke-width=""2""/>");
                deviceSvg.AppendLine($@"<circle cx=""{cx2}"" cy=""{cy2}"" r=""{r2}"" stroke=""#ccc"" fill=""#555"" stroke-width=""2""/>");
                break;

            case "ups":
                // UPS symbol with filled lightning bolt
                var upsPath = DeviceSymbols.GetSymbolPath("ups");
                deviceSvg.AppendLine($@"<path d=""{upsPath}"" fill=""#555"" stroke=""none""/>");
                break;

            default:
                // Default symbols with stroke
                var symbolPath = DeviceSymbols.GetSymbolPath(device.Type);
                if (!string.IsNullOrEmpty(symbolPath))
                {
                    deviceSvg.AppendLine($@"<path d=""{symbolPath}"" fill=""none"" stroke=""#555"" stroke-width=""2""/>");
                }
                break;
        }

        var deviceContent = deviceSvg.ToString();

        // Wrap in link if href is provided
        if (!string.IsNullOrEmpty(device.Href))
        {
            var href = !string.IsNullOrEmpty(baseHref) && Uri.IsWellFormedUriString(baseHref, UriKind.Absolute)
                ? new Uri(new Uri(baseHref), device.Href).ToString()
                : device.Href;

            return $@"<a href=""{XmlEscape(href)}"">{deviceContent}</a>";
        }

        return deviceContent;
    }

    private static string XmlEscape(string text)
    {
        return text.Replace("&", "&amp;")
                  .Replace("<", "&lt;")
                  .Replace(">", "&gt;")
                  .Replace("\"", "&quot;")
                  .Replace("'", "&apos;");
    }
}