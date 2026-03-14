using ModelContextProtocol.Server;
using SimulacaoCotacoesMcpServer.Models;
using SimulacaoCotacoesMcpServer.Tracing;
using StackExchange.Redis;
using System.ComponentModel;
using System.Text.Json;

namespace SimulacaoCotacoesMcpServer.Tools;

/// <summary>
/// MCP Tool para obter a cotacao atual do dolar em reais.
/// </summary>
internal class CotacaoDolarTool(ConnectionMultiplexer redisConnection)
{
    private ConnectionMultiplexer _redisConnection = redisConnection;

    [McpServerTool]
    [Description("Retorna a cotacao atual em reais do dolar.")]
    public async Task<Result> ObterCotacaoDolar()
    {
        using var activity1 = OpenTelemetryExtensions.ActivitySource
            .StartActivity("obter_cotacao_dolar_tool")!;
        try
        {
            var result = new Result();
            var dbRedis = _redisConnection.GetDatabase();
            if (dbRedis.KeyExists("SimulacaoCotacoes"))
            {
                var hashEntries = dbRedis.HashGetAll("SimulacaoCotacoes");
                var dict = hashEntries.ToDictionary(
                    he => he.Name.ToString(),
                    he => he.Value.ToString());
                result.IsSuccess = true;
                result.Message = "Cotacao do dolar obtida com sucesso!";
                result.Data = new Cotacao
                {
                    Moeda = "Dolar (USD)",
                    UltimaAtualizacao = $"{JsonSerializer.Deserialize<DateTime>(dict["UltimaAtualizacao"]):yyyy-MM-dd HH:mm:ss} UTC-3",
                    Valor = JsonSerializer.Deserialize<decimal>(dict["Dolar"])
                };
                activity1.SetTag("cotacao_dolar", result.Data.Valor);
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Cotacao do dolar nao encontrada no Redis.";
            }
            activity1.SetTag("is_success", result.IsSuccess);
            activity1.SetTag("message_validation", result.Message);
            return await Task.FromResult(result);
        }
        catch (Exception ex)
        {
            return new Result
            {
                IsSuccess = false,
                Message = $"Erro ao obter a cotacao do dolar: {ex.Message}"
            };
        }
        finally
        {
            activity1.Stop();
        }
    }
}