param(
    [string]$HostName = "127.0.0.1",
    [int]$Port = 9876
)

$ErrorActionPreference = "Stop"

Write-Host "Checking Claude MCP registration..."
claude mcp list

Write-Host ""
Write-Host "Checking uvx blender-mcp availability..."
uvx blender-mcp --help

Write-Host ""
Write-Host "Checking Blender addon socket $HostName`:$Port ..."
$result = Test-NetConnection $HostName -Port $Port
if ($result.TcpTestSucceeded) {
    Write-Host "OK: Blender MCP addon is listening on $HostName`:$Port"
    exit 0
}

Write-Host "NOT READY: Blender MCP addon is not listening on $HostName`:$Port"
Write-Host "Open Blender, enable the Blender MCP addon, then click Connect to Claude in the BlenderMCP sidebar."
exit 1
