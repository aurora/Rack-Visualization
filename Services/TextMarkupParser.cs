using System.Text.RegularExpressions;
using RackVisualization.Models;

namespace RackVisualization.Services;

public class TextMarkupParser
{
    private readonly string[] _lines;
    private int _currentLine;

    public TextMarkupParser(string content)
    {
        _lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(line => line.Trim())
                       .Where(line => !string.IsNullOrEmpty(line))
                       .ToArray();
        _currentLine = 0;
    }

    public RackSet Parse()
    {
        var rackSet = new RackSet();
        var rack = ParseRack();
        rackSet.Racks.Add(rack);
        return rackSet;
    }

    private Rack ParseRack()
    {
        var rack = new Rack();
        
        // Parse header (caption and height)
        ParseHeader(rack);
        
        // Parse items
        ParseItems(rack);
        
        return rack;
    }

    private void ParseHeader(Rack rack)
    {
        // Parse caption line: "caption: <text>"
        var captionLine = GetNextLine();
        if (!captionLine.StartsWith("caption:"))
            throw new FormatException($"Expected 'caption:' at line {_currentLine}, got: {captionLine}");
        
        rack.Name = captionLine.Substring(8).Trim();

        // Parse height line: "height: <int>"
        var heightLine = GetNextLine();
        if (!heightLine.StartsWith("height:"))
            throw new FormatException($"Expected 'height:' at line {_currentLine}, got: {heightLine}");
        
        var heightStr = heightLine.Substring(7).Trim();
        if (!int.TryParse(heightStr, out int height))
            throw new FormatException($"Invalid height value: {heightStr}");
        
        rack.Height = height;
    }

    private void ParseItems(Rack rack)
    {
        // Parse items header: "items:"
        var itemsLine = GetNextLine();
        if (!itemsLine.StartsWith("items:"))
            throw new FormatException($"Expected 'items:' at line {_currentLine}, got: {itemsLine}");

        // Parse item list
        while (HasMoreLines())
        {
            var line = PeekNextLine();
            if (!line.TrimStart().StartsWith("-"))
                break;
                
            var device = ParseItemEntry();
            rack.Devices.Add(device);
        }
    }

    private RackDevice ParseItemEntry()
    {
        var line = GetNextLine();
        
        // Remove leading whitespace and dash
        line = line.TrimStart();
        if (!line.StartsWith("-"))
            throw new FormatException($"Expected '-' at beginning of item entry at line {_currentLine}");
        
        line = line.Substring(1).Trim();
        
        // Split at first colon to separate key from label
        var colonIndex = line.IndexOf(':');
        if (colonIndex == -1)
            throw new FormatException($"Expected ':' in item entry at line {_currentLine}");
        
        var keyPart = line.Substring(0, colonIndex).Trim();
        var labelPart = line.Substring(colonIndex + 1).Trim();
        
        var device = ParseKey(keyPart);
        device.Text = ParseLabel(labelPart);
        device.Href = ExtractHrefFromLabel(labelPart);
        
        return device;
    }

    private RackDevice ParseKey(string keyPart)
    {
        var device = new RackDevice();
        
        // Check for height specification [n]
        var heightMatch = Regex.Match(keyPart, @"^(.+?)\[(\d+)\]$");
        if (heightMatch.Success)
        {
            keyPart = heightMatch.Groups[1].Value.Trim();
            device.Height = int.Parse(heightMatch.Groups[2].Value);
        }
        else
        {
            device.Height = 1; // Default height
        }
        
        // Check for custom type (identifier:identifier)
        var customTypeMatch = Regex.Match(keyPart, @"^([a-zA-Z][a-zA-Z0-9_-]*):([a-zA-Z][a-zA-Z0-9_-]*)$");
        if (customTypeMatch.Success)
        {
            // For custom types, use the second identifier as the type
            device.Type = customTypeMatch.Groups[2].Value;
        }
        else
        {
            // Simple identifier
            if (!Regex.IsMatch(keyPart, @"^[a-zA-Z][a-zA-Z0-9_-]*$"))
                throw new FormatException($"Invalid identifier: {keyPart}");
            
            device.Type = keyPart;
        }
        
        return device;
    }

    private string ParseLabel(string labelPart)
    {
        // Parse Markdown-style links: [text](url)
        var linkMatch = Regex.Match(labelPart, @"^\[([^\]]+)\]\(([^)]+)\)$");
        if (linkMatch.Success)
        {
            // This is a link - we'll need to set the href in the calling method
            return linkMatch.Groups[1].Value; // Return just the text part
        }
        
        // Return the label as-is if it's not a link
        return labelPart;
    }
    
    private string? ExtractHrefFromLabel(string labelPart)
    {
        // Extract href from Markdown-style links: [text](url)
        var linkMatch = Regex.Match(labelPart, @"^\[([^\]]+)\]\(([^)]+)\)$");
        if (linkMatch.Success)
        {
            return linkMatch.Groups[2].Value; // Return the URL part
        }
        
        return null; // No link found
    }

    private string GetNextLine()
    {
        if (_currentLine >= _lines.Length)
            throw new FormatException("Unexpected end of input");
        
        return _lines[_currentLine++];
    }

    private string PeekNextLine()
    {
        if (_currentLine >= _lines.Length)
            return string.Empty;
        
        return _lines[_currentLine];
    }

    private bool HasMoreLines()
    {
        return _currentLine < _lines.Length;
    }
}