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

            Console.WriteLine($"Lese Eingabedatei: {inputFile}");
            string inputContent = File.ReadAllText(inputFile);

            // Determine format based on file extension or content
            var rackSet = ParseInput(inputFile, inputContent);

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

    static Models.RackSet ParseInput(string inputFile, string inputContent)
    {
        string extension = Path.GetExtension(inputFile).ToLowerInvariant();
        
        if (extension == ".xml")
        {
            Console.WriteLine("Parse RackML (XML-Format)...");
            return RackMLParser.ParseRackML(inputContent);
        }
        else if (extension == ".txt" || extension == ".rack")
        {
            Console.WriteLine("Parse Text-Markup-Format...");
            var parser = new TextMarkupParser(inputContent);
            return parser.Parse();
        }
        else
        {
            // Try to auto-detect format based on content
            if (inputContent.TrimStart().StartsWith("<"))
            {
                Console.WriteLine("Parse RackML (XML-Format, auto-detected)...");
                return RackMLParser.ParseRackML(inputContent);
            }
            else
            {
                Console.WriteLine("Parse Text-Markup-Format (auto-detected)...");
                var parser = new TextMarkupParser(inputContent);
                return parser.Parse();
            }
        }
    }

    static void ShowUsage()
    {
        Console.WriteLine("Verwendung:");
        Console.WriteLine("  RackVisualization.exe <eingabedatei> [ausgabe.svg]");
        Console.WriteLine();
        Console.WriteLine("Parameter:");
        Console.WriteLine("  eingabedatei  - Eingabedatei (.xml für RackML, .txt/.rack für Text-Markup)");
        Console.WriteLine("  ausgabe.svg   - SVG-Ausgabedatei (optional, Standard: output.svg)");
        Console.WriteLine();
        Console.WriteLine("Beispiele:");
        Console.WriteLine("  RackVisualization.exe rack.xml rack-diagram.svg");
        Console.WriteLine("  RackVisualization.exe rack.txt rack-diagram.svg");
        Console.WriteLine();
        Console.WriteLine("RackML XML-Format:");
        Console.WriteLine("  <racks>");
        Console.WriteLine("    <rack height=\"42\" name=\"Server Rack\">");
        Console.WriteLine("      <server height=\"2\">Web Server</server>");
        Console.WriteLine("      <switch>Network Switch</switch>");
        Console.WriteLine("      <firewall href=\"firewall-01\">Firewall</firewall>");
        Console.WriteLine("    </rack>");
        Console.WriteLine("  </racks>");
        Console.WriteLine();
        Console.WriteLine("Text-Markup-Format:");
        Console.WriteLine("  caption: Server Rack");
        Console.WriteLine("  height: 42");
        Console.WriteLine("  items:");
        Console.WriteLine("    - server[2]: Web Server");
        Console.WriteLine("    - switch: Network Switch");
        Console.WriteLine("    - firewall: [Firewall](firewall-01)");
    }
}