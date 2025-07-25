namespace RackVisualization.Services;

public static class DeviceSymbols
{
    public static string GetSymbolPath(string deviceType)
    {
        return deviceType switch
        {
            "router" => @"M 12 7 l -5 5 l 5 5 m 3 -5 a 1,1 0 1,1 -2,0 a 1,1 0 1,1 2,0 m 5 0 a 1,1 0 1,1 -2,0 a 1,1 0 1,1 2,0 m 5 0 a 1,1 0 1,1 -2,0 a 1,1 0 1,1 2,0 m 1 -5 l 5 5 l -5 5",
            
            "cables" => @"M 12 7 l -5 5 l 5 5 m 3 -5 a 1,1 0 1,1 -2,0 a 1,1 0 1,1 2,0 m 5 0 a 1,1 0 1,1 -2,0 a 1,1 0 1,1 2,0 m 5 0 a 1,1 0 1,1 -2,0 a 1,1 0 1,1 2,0 m 1 -5 l 5 5 l -5 5",
            
            "firewall" => @"M 8 6.5 h 20 v 12 h -20 z m 0 4 h 20 m 0 4 h -20 M 7 7 m 7 0 v 4 m 8 0 v -4 m -4 4 v 4 m -4 0 v 4 m 8 0 v -4",
            
            "pdu" => @"M 17 5 v 7 m 3 -5 a 6 6 240 1 1 -6 0",
            
            "san" => @"M 7 7 v 15 h 5 v -15 h -5 m 7 0 v 15 h 5 v -15 h -5 m 7 0 v 15 h 5 v -15 h -5",
            
            "server" => @"M 7 5 h 20 v 7 h -20 v -7 m 0 8 h 20 v 7 h -20 v -7 M 25 8 a 2,2 0 1,1 -4,0 a 2,2 0 1,1 4,0 M 25 16 a 2,2 0 1,1 -4,0 a 2,2 0 1,1 4,0",
            
            "switch" => @"M 19 5 h 6 v -2.5 l 4 4 l -4 4 v -2.5 h -6 m -2 1 h -6 v -2.5 l -4 4 l 4 4 v -2.5 h 6 m 2 2 h 6 v -2.5 l 4 4 l -4 4 v -2.5 h -6 m -2 1 h -6 v -2.5 l -4 4 l 4 4 v -2.5 h 6",
            
            "ups" => @"M 15 7 h 6 l -3 5 h 3 l -8 10 l 2 -7 h -3",
            
            _ => string.Empty
        };
    }

    public static List<(double x, double y, double width, double height)> GetPatchPanelPorts()
    {
        var ports = new List<(double, double, double, double)>();
        
        // Create 8 ports in 2 rows, exactly like JavaScript version
        var positions = new[]
        {
            (7, 7), (14, 7), (21, 7), (28, 7),
            (7, 14), (14, 14), (21, 14), (28, 14)
        };
        
        foreach (var (x, y) in positions)
        {
            ports.Add((x, y, 4, 4));
        }
        
        return ports;
    }

    public static (double x, double y, double width, double height) GetTapeBackground()
    {
        return (6, 7, 24, 15);
    }

    public static (double cx, double cy, double r) GetTapeDriveCircle1()
    {
        return (13, 15, 3);
    }

    public static (double cx, double cy, double r) GetTapeDriveCircle2()
    {
        return (23, 15, 3);
    }
}