# aspnetcore10-mcp-otel-appinsights-redis_simulacaocotacoes
Implementação em ASP.NET Core + .NET 10 de MCP Server para consulta a cotações de moedas estrangeiras (simulação) armazenadas em uma instância do Redis. Inclui o uso de um script do Docker Compose (ambiente de testes), monitoramento/observabilidade com OpenTelemetry + Azure Application Insights e um Dockerfile (build de imagens deste MCP).

Imagem publicada no Docker Hub para este MCP Server:

```
renatogroffe/aspnetcore10-mcp-simulacaocotacoes:2
```

Aplicação para geração das cotações (dólar, euro, libra esterlina): https://github.com/renatogroffe/dotnet10-consoleapp-redis_simulacaocotacoes

Arquivo **mcp.json (VS Code)** com configurações para testes locais:

```json
{
	"servers": {
		"mcp-cotacoes-moedas": {
			"url": "http://localhost:5200/",
			"type": "http"
		}
	},
	"inputs": []
}
```