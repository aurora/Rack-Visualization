# Rack Visualization - C# Konsolenapplikation

> **Hinweis**: Dies ist eine automatisch mittels Roo unter Verwendung von Modellen von Anthropic und Mistral portierte und erweiterte Version des ursprünglichen Projekts [https://github.com/balki97/Rack-Visualization](https://github.com/balki97/Rack-Visualization).

Eine C# Konsolenapplikation zur Generierung von Rack-Elevation-Diagrammen aus RackML XML-Dateien oder Text-Markup-Format.

## Funktionen

- Parst RackML XML-Dateien und Text-Markup-Format
- Generiert SVG-Diagramme von Server-Racks
- Unterstützt verschiedene Gerätetypen (Server, Switch, Firewall, etc.)
- Farbschemas für verschiedene Gerätetypen
- Gerätesymbole und Icons
- Klickbare Links in SVG-Ausgabe (mit visueller Hervorhebung)
- Markdown-Syntax für Links im Text-Format

## Installation

1. .NET 8.0 SDK installieren
2. Projekt kompilieren:
   ```bash
   dotnet build RackVisualization.csproj
   ```

## Verwendung

```bash
dotnet run --project RackVisualization.csproj <eingabedatei> [ausgabe.svg]
```

### Parameter

- `eingabedatei` - Eingabedatei (.xml für RackML, .txt/.rack für Text-Markup)
- `ausgabe.svg` - SVG-Ausgabedatei (optional, Standard: output.svg)

### Beispiele

```bash
# XML-Format
dotnet run --project RackVisualization.csproj Examples/example.xml rack-diagram.svg

# Text-Markup-Format
dotnet run --project RackVisualization.csproj Examples/example.txt rack-diagram.svg
```

## Eingabeformate

### RackML XML-Format

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

### Text-Markup-Format

Das Text-Markup-Format bietet eine einfachere, lesbarere Alternative zum XML-Format:

```
caption: Server Rack
height: 42
items:
  - server[2]: [Web Server](server-01)
  - switch: Network Switch
  - firewall: [Firewall](firewall-01)
  - cables: Cable Management
  - patch: Patch Panel
  - pdu: Power Distribution Unit
  - ups[4]: UPS
  - blank[2]: 
  - gap[2]: 
```

#### Text-Markup-Syntax

- **Header**: `caption:` und `height:` definieren Rack-Name und -Höhe
- **Items**: Liste mit `-` Präfix für jedes Gerät
- **Gerätetyp**: `type` oder `custom:type` für benutzerdefinierte Typen
- **Höhe**: `[n]` nach dem Typ für Geräte mit mehr als 1U
- **Links**: Markdown-Syntax `[Text](URL)` für klickbare Labels
- **Leere Geräte**: Leer lassen für `blank` und `gap` Einträge

#### EBNF-Grammatik

```ebnf
rack          = header, items ;
header        = caption_line, height_line ;
caption_line  = "caption", ":", ws, text, eol ;
height_line   = "height", ":", ws, int, eol ;
items         = "items", ":", ws, item_list ;
item_list     = { ws, "-", ws, item_entry, eol } ;
item_entry    = key, ":", ws, label ;
key           = type, [ height ] ;
type          = identifier | custom_type ;
custom_type   = identifier, ":", identifier ;
height        = "[", int, "]" ;
label         = text ;  (* kann Markdown enthalten *)
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
│   ├── TextMarkupParser.cs    # Text-Markup-Parser
│   └── SvgGenerator.cs        # SVG-Generierung
├── Program.cs                 # Hauptprogramm
├── RackVisualization.csproj   # Projektdatei
└── Examples/
    ├── example.xml           # Beispiel-RackML-Datei
    └── example.txt           # Beispiel-Text-Markup-Datei
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

Diese C# Konsolenapplikation erweitert die Funktionalität der ursprünglichen JavaScript-Web-Applikation:

- ✅ RackML XML-Parsing
- ✅ Text-Markup-Format (neu)
- ✅ SVG-Generierung
- ✅ Farbschemas (Pastel/Default)
- ✅ Gerätesymbole
- ✅ Rack-Layout und -Positionierung
- ✅ Klickbare Links mit visueller Hervorhebung (blau, unterstrichen)
- ✅ Markdown-Syntax für Links im Text-Format
- ✅ Kommandozeilen-Interface
- ✅ Auto-Erkennung des Eingabeformats

Die Ausgabe ist kompatibel mit der ursprünglichen Web-Version und kann in Webbrowsern oder SVG-Viewern angezeigt werden.

## Neue Features

### Visuelle Link-Hervorhebung
Klickbare Labels werden jetzt automatisch durch blaue Farbe (#0066cc) und Unterstreichung hervorgehoben, mit einem dunkleren Hover-Effekt (#004499).

### Text-Markup-Format
Das neue Text-Markup-Format bietet eine einfachere Alternative zum XML-Format mit:
- Lesbarerer Syntax
- Markdown-Links: `[Text](URL)`
- Höhenangaben: `type[n]`
- Automatische Formaterkennnung