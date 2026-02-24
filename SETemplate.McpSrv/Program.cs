namespace SETemplate.McpSrv;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        _ = builder.Services
            .AddMcpServer()
            .WithHttpTransport()
            .WithToolsFromAssembly();

        var app = builder.Build();

        // MCP via MapMcp (Standard)
        app.MapMcp("/mcp");

        app.Run();
    }
}