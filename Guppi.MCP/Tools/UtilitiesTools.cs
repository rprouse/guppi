using System;
using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Guppi.MCP.Tools
{
    [McpServerToolType]
    public class UtilitiesTools
    {
        [McpServerTool, Description("Gets the current date and time")]
        public static string GetDateTime()
        {
            return DateTime.Now.ToString("F");
        }

        [McpServerTool, Description("Generates a new GUID")]
        public static string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
