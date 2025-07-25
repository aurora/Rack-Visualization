# Rack Visualization - C# Konsolenapplikation

Eine C# Konsolenapplikation zur Generierung von Rack-Elevation-Diagrammen aus RackML XML-Dateien.

## Funktionen

- Parst RackML XML-Dateien
- Generiert SVG-Diagramme von Server-Racks
- Unterstützt verschiedene Gerätetypen (Server, Switch, Firewall, etc.)
- Farbschemas für verschiedene Gerätetypen
- Gerätesymbole und Icons
- Klickbare Links in SVG-Ausgabe

## Installation

1. .NET 8.0 SDK installieren
2. Projekt kompilieren:
   ```bash
   dotnet build RackVisualization.csproj
   ```

## Verwendung

```bash
dotnet run --project RackVisualization.csproj <eingabe.xml> [ausgabe.svg]
```

### Parameter

- `eingabe.xml` - RackML XML-Eingabedatei (erforderlich)
- `ausgabe.svg` - SVG-Ausgabedatei (optional, Standard: output.svg)

### Beispiel

```bash
dotnet run --project RackVisualization.csproj example.xml rack-diagram.svg
```

## RackML Format

```xml
<racks base="https://kb/">
  <rack height="42" name="Server Rack">
    <server height="2" href="server-01">Web Server</server>
    <switch>Network Switch</switch>
    <firewall href="firewall-01">Firewall</firewall>
    <cables>Cable Management</cables>
    <patch>Patch Panel</patch>
    <pdu>Power Distribution Unit</pdu>
    <ups height="4">UPS</ups>
    <blank height="2"/>
    <gap height="2"/>
  </rack>
</racks>
```

## Unterstützte Gerätetypen

- `server` - Server
- `switch` - Netzwerk-Switch
- `firewall` - Firewall
- `router` - Router
- `cables` - Kabelmanagement
- `patch` - Patch Panel
- `pdu` - Power Distribution Unit
- `ups` - Unterbrechungsfreie Stromversorgung
- `san` - Storage Area Network
- `tape` - Bandlaufwerk
- `monitor` - Monitor
- `keyboard` - Tastatur
- `kvm` - KVM-Switch
- `blank` - Leerer Platz
- `gap` - Lücke (wird nicht gezeichnet)

## Attribute

### Rack-Attribute
- `height` - Höhe des Racks in Rack-Einheiten (Standard: 42)
- `name` - Name des Racks (wird über dem Rack angezeigt)

### Geräte-Attribute
- `height` - Höhe des Geräts in Rack-Einheiten (Standard: 1)
- `at` - Position des Geräts im Rack (von unten gezählt)
- `href` - URL für zusätzliche Informationen (macht das Gerät klickbar)
- `color` - Benutzerdefinierte Farbe (überschreibt Standard-Farbschema)

## Projektstruktur

```
RackVisualization/
├── Models/
│   └── RackDevice.cs          # Datenmodelle
├── Services/
│   ├── ColorSchemes.cs        # Farbschemas
│   ├── DeviceSymbols.cs       # Gerätesymbole
│   ├── RackMLParser.cs        # XML-Parser
│   └── SvgGenerator.cs        # SVG-Generierung
├── Program.cs                 # Hauptprogramm
├── RackVisualization.csproj   # Projektdatei
└── example.xml               # Beispiel-RackML-Datei
```

## Beispielausgabe

Die Applikation generiert eine SVG-Datei mit:
- Rack-Rahmen mit Rasterlinien
- Farbkodierte Geräte nach Typ
- Gerätesymbole und -icons
- Gerätenamen als Text
- Klickbare Links (falls href-Attribute vorhanden)
- Responsive Größenanpassung basierend auf Rack-Höhe

## Konvertierung von der JavaScript-Version

Diese C# Konsolenapplikation repliziert die Funktionalität der ursprünglichen JavaScript-Web-Applikation:

- ✅ RackML XML-Parsing
- ✅ SVG-Generierung
- ✅ Farbschemas (Pastel/Default)
- ✅ Gerätesymbole
- ✅ Rack-Layout und -Positionierung
- ✅ Klickbare Links
- ✅ Kommandozeilen-Interface

Die Ausgabe ist kompatibel mit der ursprünglichen Web-Version und kann in Webbrowsern oder SVG-Viewern angezeigt werden.