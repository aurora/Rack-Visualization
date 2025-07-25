namespace RackVisualization.Models;

public class RackDevice
{
    public string Type { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Height { get; set; } = 1;
    public int? At { get; set; }
    public string? Href { get; set; }
    public string? Color { get; set; }
}

public class Rack
{
    public string Name { get; set; } = string.Empty;
    public int Height { get; set; } = 42;
    public List<RackDevice> Devices { get; set; } = new();
}

public class RackSet
{
    public string? BaseHref { get; set; }
    public List<Rack> Racks { get; set; } = new();
}