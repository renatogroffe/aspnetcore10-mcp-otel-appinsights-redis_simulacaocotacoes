using ModelContextProtocol.Server;
using SimulacaoCotacoesMcpServer.Models;
using SimulacaoCotacoesMcpServer.Tracing;
using StackExchange.Redis;
using System.ComponentModel;
using System.Text.Json;

namespace SimulacaoCotacoesMcpServer.Tools;

/// <summary>
/// MCP Tool para obter a cotacao atual do euro em reais.
/// </summary>
internal class CotacaoEuroTool(ConnectionMultiplexer redisConnection)
{
    private ConnectionMultiplexer _redisConnection = redisConnection;

    [McpServerTool]
    [Description("Retorna a cotacao atual em reais do euro.")]
    public async Task<Result> ObterCotacaoEuro()
    {
        using var activity1 = OpenTelemetryExtensions.ActivitySource
            .StartActivity("obter_cotacao_euro_tool")!;
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
                result.Message = "Cotacao do euro obtida com sucesso!";
                result.Data = new Cotacao
                {
                    Moeda = "Euro (EUR)",
                    UltimaAtualizacao = $"{JsonSerializer.Deserialize<DateTime>(dict["UltimaAtualizacao"]):yyyy-MM-dd HH:mm:ss} UTC-3",
                    Valor = JsonSerializer.Deserialize<decimal>(dict["Euro"])
                };
                activity1.SetTag("cotacao_euro", result.Data.Valor);
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Cotacao do euro nao encontrada no Redis.";
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
                Message = $"Erro ao obter a cotacao do euro: {ex.Message}"
            };
        }
        finally
        {
            activity1.Stop();
        }
    }
}