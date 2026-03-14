using System.ComponentModel;

namespace SimulacaoCotacoesMcpServer.Models;

public class Cotacao
{
    public string? Moeda { get; set; }
    public decimal? Valor { get; set; }
    public string? UltimaAtualizacao { get; set; }
}