# Rack Visualization - C# Console Application

> **Note**: This is an automatically ported and extended version of the original project [https://github.com/balki97/Rack-Visualization](https://github.com/balki97/Rack-Visualization), created using Roo with models from Anthropic and Mistral.

A C# console application for generating rack elevation diagrams from RackML XML files or text markup format.

## Features

- Parses RackML XML files and text markup format
- Generates SVG diagrams of server racks
- Supports various device types (Server, Switch, Firewall, etc.)
- Color schemes for different device types
- Device symbols and icons
- Clickable links in SVG output (with visual highlighting)
- Markdown syntax for links in text format

## Installation

1. Install .NET 8.0 SDK
2. Build the project:
   ```bash
   dotnet build RackVisualization.csproj
   ```

## Usage

```bash
dotnet run --project RackVisualization.csproj <input-file> [output.svg]
```

### Parameters

- `input-file` - Input file (.xml for RackML, .txt/.rack for text markup)
- `output.svg` - SVG output file (optional, default: output.svg)

### Examples

```bash
# XML format
dotnet run --project RackVisualization.csproj Examples/example.xml rack-diagram.svg

# Text markup format
dotnet run --project RackVisualization.csproj Examples/example.txt rack-diagram.svg
```

## Input Formats

### RackML XML Format

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

### Text Markup Format

The text markup format provides a simpler, more readable alternative to the XML format:

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

#### Text Markup Syntax

- **Header**: `caption:` and `height:` define rack name and height
- **Items**: List with `-` prefix for each device
- **Device Type**: `type` or `custom:type` for custom types
- **Height**: `[n]` after type for devices with more than 1U
- **Links**: Markdown syntax `[Text](URL)` for clickable labels
- **Empty Devices**: Leave empty for `blank` and `gap` entries

#### EBNF Grammar

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
label         = text ;  (* can contain Markdown *)
```

## Supported Device Types

- `server` - Server
- `switch` - Network Switch
- `firewall` - Firewall
- `router` - Router
- `cables` - Cable Management
- `patch` - Patch Panel
- `pdu` - Power Distribution Unit
- `ups` - Uninterruptible Power Supply
- `san` - Storage Area Network
- `tape` - Tape Drive
- `monitor` - Monitor
- `keyboard` - Keyboard
- `kvm` - KVM Switch
- `blank` - Empty Space
- `gap` - Gap (not drawn)

## Attributes

### Rack Attributes
- `height` - Rack height in rack units (default: 42)
- `name` - Rack name (displayed above the rack)

### Device Attributes
- `height` - Device height in rack units (default: 1)
- `at` - Device position in rack (counted from bottom)
- `href` - URL for additional information (makes device clickable)
- `color` - Custom color (overrides default color scheme)

## Project Structure

```
RackVisualization/
├── Models/
│   └── RackDevice.cs          # Data models
├── Services/
│   ├── ColorSchemes.cs        # Color schemes
│   ├── DeviceSymbols.cs       # Device symbols
│   ├── RackMLParser.cs        # XML parser
│   ├── TextMarkupParser.cs    # Text markup parser
│   └── SvgGenerator.cs        # SVG generation
├── Program.cs                 # Main program
├── RackVisualization.csproj   # Project file
└── Examples/
    ├── example.xml           # Example RackML file
    └── example.txt           # Example text markup file
```

## Example Output

The application generates an SVG file with:
- Rack frame with grid lines
- Color-coded devices by type
- Device symbols and icons
- Device names as text
- Clickable links (if href attributes present)
- Responsive sizing based on rack height

## Conversion from JavaScript Version

This C# console application extends the functionality of the original JavaScript web application:

- ✅ RackML XML parsing
- ✅ Text markup format (new)
- ✅ SVG generation
- ✅ Color schemes (Pastel/Default)
- ✅ Device symbols
- ✅ Rack layout and positioning
- ✅ Clickable links with visual highlighting (blue, underlined)
- ✅ Markdown syntax for links in text format
- ✅ Command-line interface
- ✅ Auto-detection of input format

The output is compatible with the original web version and can be displayed in web browsers or SVG viewers.

## New Features

### Visual Link Highlighting
Clickable labels are now automatically highlighted with blue color (#0066cc) and underline, with a darker hover effect (#004499).

### Text Markup Format
The new text markup format provides a simpler alternative to the XML format with:
- More readable syntax
- Markdown links: `[Text](URL)`
- Height specifications: `type[n]`
- Automatic format detection
