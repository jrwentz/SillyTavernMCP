# SillyTavernMCP

A local C# MCP server for reading information from a SillyTavern instance.

## Features

The server currently exposes three read-only MCP tools:

- `get_sillytavern_settings`
- `list_sillytavern_characters`
- `list_sillytavern_groups`

Each tool returns formatted JSON text from the configured SillyTavern instance.

## Configuration

Set these environment variables before starting the server:

- `SILLY_TAVERN_BASE_URL` - Optional. Defaults to `http://127.0.0.1:8000`
- `SILLY_TAVERN_API_KEY` - Optional. Sent as the `X-API-KEY` header when present

## Run

```bash
cd src
dotnet run
```

The server uses the MCP stdio transport, so it can be launched directly by an MCP-compatible client.
