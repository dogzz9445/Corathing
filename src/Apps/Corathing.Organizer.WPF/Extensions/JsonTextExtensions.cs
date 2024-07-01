using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Corathing.Organizer.WPF.Extensions;

public static class JsonTextExtensions
{
    public static JsonNode? ToNode(this JsonDocument document)
    {
        return document.Deserialize<JsonNode>();
    }

    public static JsonDocument? ToDocument(this JsonNode node)
    {
        return node.Deserialize<JsonDocument>();
    }
}
