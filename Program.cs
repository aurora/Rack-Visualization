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
                Console.WriteLine($"Fehler: Eingabedatei '{inputFile}' wurde nicht gefunden.");
                return;
            }

            Console.WriteLine($"Lese RackML aus: {inputFile}");
            string rackmlContent = File.ReadAllText(inputFile);

            Console.WriteLine("Parse RackML...");
            var rackSet = RackMLParser.ParseRackML(rackmlContent);

            Console.WriteLine($"Gefunden: {rackSet.Racks.Count} Rack(s)");
            foreach (var rack in rackSet.Racks)
            {
                Console.WriteLine($"  - {rack.Name} ({rack.Height}U, {rack.Devices.Count} Geräte)");
            }

            Console.WriteLine("Generiere SVG...");
            var svgGenerator = new SvgGenerator();
            string svgContent = svgGenerator.GenerateSvg(rackSet);

            Console.WriteLine($"Schreibe SVG nach: {outputFile}");
            File.WriteAllText(outputFile, svgContent);

            Console.WriteLine("✓ Erfolgreich abgeschlossen!");
            Console.WriteLine($"SVG-Datei erstellt: {outputFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Details: {ex.InnerException.Message}");
            }
        }
    }

    static void ShowUsage()
    {
        Console.WriteLine("Verwendung:");
        Console.WriteLine("  RackVisualization.exe <eingabe.xml> [ausgabe.svg]");
        Console.WriteLine();
        Console.WriteLine("Parameter:");
        Console.WriteLine("  eingabe.xml   - RackML XML-Eingabedatei");
        Console.WriteLine("  ausgabe.svg   - SVG-Ausgabedatei (optional, Standard: output.svg)");
        Console.WriteLine();
        Console.WriteLine("Beispiel:");
        Console.WriteLine("  RackVisualization.exe rack.xml rack-diagram.svg");
        Console.WriteLine();
        Console.WriteLine("RackML Format:");
        Console.WriteLine("  <racks>");
        Console.WriteLine("    <rack height=\"42\" name=\"Server Rack\">");
        Console.WriteLine("      <server height=\"2\">Web Server</server>");
        Console.WriteLine("      <switch>Network Switch</switch>");
        Console.WriteLine("      <firewall href=\"firewall-01\">Firewall</firewall>");
        Console.WriteLine("    </rack>");
        Console.WriteLine("  </racks>");
    }
}