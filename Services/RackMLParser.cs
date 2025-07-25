using System.Xml;
using RackVisualization.Models;

namespace RackVisualization.Services;

public class RackMLParser
{
    public static RackSet ParseRackML(string rackmlContent)
    {
        var doc = new XmlDocument();
        doc.LoadXml(rackmlContent);

        var racksElement = doc.DocumentElement;
        if (racksElement?.Name != "racks")
        {
            throw new ArgumentException("Root element must be 'racks'");
        }

        var rackSet = new RackSet
        {
            BaseHref = racksElement.GetAttribute("base")
        };

        foreach (XmlNode rackNode in racksElement.ChildNodes)
        {
            if (rackNode.NodeType != XmlNodeType.Element || rackNode.Name != "rack")
                continue;

            var rack = ParseRack((XmlElement)rackNode);
            rackSet.Racks.Add(rack);
        }

        return rackSet;
    }

    private static Rack ParseRack(XmlElement rackElement)
    {
        var rack = new Rack
        {
            Name = rackElement.GetAttribute("name") ?? string.Empty,
            Height = int.TryParse(rackElement.GetAttribute("height"), out var height) ? height : 42
        };

        foreach (XmlNode deviceNode in rackElement.ChildNodes)
        {
            if (deviceNode.NodeType != XmlNodeType.Element)
                continue;

            var device = ParseDevice((XmlElement)deviceNode);
            if (device != null)
            {
                rack.Devices.Add(device);
            }
        }

        return rack;
    }

    private static RackDevice? ParseDevice(XmlElement deviceElement)
    {
        var deviceType = deviceElement.Name;
        
        // Skip gap elements - they don't create actual devices
        if (deviceType == "gap")
            return null;

        var device = new RackDevice
        {
            Type = deviceType,
            Text = deviceElement.InnerText?.Trim() ?? string.Empty,
            Height = int.TryParse(deviceElement.GetAttribute("height"), out var height) ? height : 1,
            Href = string.IsNullOrEmpty(deviceElement.GetAttribute("href")) ? null : deviceElement.GetAttribute("href"),
            Color = string.IsNullOrEmpty(deviceElement.GetAttribute("color")) ? null : deviceElement.GetAttribute("color")
        };

        if (int.TryParse(deviceElement.GetAttribute("at"), out var at))
        {
            device.At = at;
        }

        return device;
    }
}