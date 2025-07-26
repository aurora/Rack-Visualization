using RackVisualization.Services;

namespace RackVisualization;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Rack Visualization Tool");
        Console.WriteLine("=======================");

        if (args.Length == 0)
        {
            ShowUsage();
            return;
        }

        try
        {
            string inputFile = args[0];
            string outputFile = args.Length > 1 ? args[1] : "output.svg";

            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Error: Input file '{inputFile}' not found.");
                return;
            }

            Console.WriteLine($"Reading input file: {inputFile}");
            string inputContent = File.ReadAllText(inputFile);

            // Determine format based on file extension or content
            var rackSet = ParseInput(inputFile, inputContent);

            Console.WriteLine($"Found: {rackSet.Racks.Count} Rack(s)");
            foreach (var rack in rackSet.Racks)
            {
                Console.WriteLine($"  - {rack.Name} ({rack.Height}U, {rack.Devices.Count} devices)");
            }

            Console.WriteLine("Generating SVG...");
            var svgGenerator = new SvgGenerator();
            string svgContent = svgGenerator.GenerateSvg(rackSet);

            Console.WriteLine($"Writing SVG to: {outputFile}");
            File.WriteAllText(outputFile, svgContent);

            Console.WriteLine("✓ Successfully completed!");
            Console.WriteLine($"SVG file created: {outputFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Details: {ex.InnerException.Message}");
            }
        }
    }

    static Models.RackSet ParseInput(string inputFile, string inputContent)
    {
        string extension = Path.GetExtension(inputFile).ToLowerInvariant();
        
        if (extension == ".xml")
        {
            Console.WriteLine("Parsing RackML (XML format)...");
            return RackMLParser.ParseRackML(inputContent);
        }
        else if (extension == ".txt" || extension == ".rack")
        {
            Console.WriteLine("Parsing text markup format...");
            var parser = new TextMarkupParser(inputContent);
            return parser.Parse();
        }
        else
        {
            // Try to auto-detect format based on content
            if (inputContent.TrimStart().StartsWith("<"))
            {
                Console.WriteLine("Parsing RackML (XML format, auto-detected)...");
                return RackMLParser.ParseRackML(inputContent);
            }
            else
            {
                Console.WriteLine("Parsing text markup format (auto-detected)...");
                var parser = new TextMarkupParser(inputContent);
                return parser.Parse();
            }
        }
    }

    static void ShowUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  RackVisualization.exe <input-file> [output.svg]");
        Console.WriteLine();
        Console.WriteLine("Parameters:");
        Console.WriteLine("  input-file    - Input file (.xml for RackML, .txt/.rack for text markup)");
        Console.WriteLine("  output.svg    - SVG output file (optional, default: output.svg)");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  RackVisualization.exe rack.xml rack-diagram.svg");
        Console.WriteLine("  RackVisualization.exe rack.txt rack-diagram.svg");
        Console.WriteLine();
        Console.WriteLine("RackML XML Format:");
        Console.WriteLine("  <racks>");
        Console.WriteLine("    <rack height=\"42\" name=\"Server Rack\">");
        Console.WriteLine("      <server height=\"2\">Web Server</server>");
        Console.WriteLine("      <switch>Network Switch</switch>");
        Console.WriteLine("      <firewall href=\"firewall-01\">Firewall</firewall>");
        Console.WriteLine("    </rack>");
        Console.WriteLine("  </racks>");
        Console.WriteLine();
        Console.WriteLine("Text Markup Format:");
        Console.WriteLine("  caption: Server Rack");
        Console.WriteLine("  height: 42");
        Console.WriteLine("  items:");
        Console.WriteLine("    - server[2]: Web Server");
        Console.WriteLine("    - switch: Network Switch");
        Console.WriteLine("    - firewall: [Firewall](firewall-01)");
    }
}