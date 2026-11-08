using System.Text;

namespace MiniBitcoin.Core;

/// <summary>
/// Histórico das operações realizadas e cálculo do patrimônio consolidado.
/// </summary>
public class Extrato
{
    private readonly List<Transacao> _transacoes = new();

    public IReadOnlyList<Transacao> Transacoes => _transacoes.AsReadOnly();

    public decimal TotalComprado =>
        _transacoes.Where(t => t.Tipo == TipoTransacao.Compra).Sum(t => t.ValorBrl);

    public decimal TotalVendido =>
        _transacoes.Where(t => t.Tipo == TipoTransacao.Venda).Sum(t => t.ValorBrl);

    public void Registrar(Transacao transacao)
    {
        ArgumentNullException.ThrowIfNull(transacao);
        _transacoes.Add(transacao);
    }

    /// <summary>
    /// Patrimônio total em BRL: saldo em reais + saldo em Bitcoin convertido pela cotação atual.
    /// </summary>
    public decimal CalcularPatrimonio(Carteira carteira, Cotacao cotacao)
    {
        ArgumentNullException.ThrowIfNull(carteira);
        ArgumentNullException.ThrowIfNull(cotacao);

        var patrimonio = carteira.SaldoBrl + carteira.SaldoBtc * cotacao.PrecoBrl;
        return Math.Round(patrimonio, 2, MidpointRounding.ToZero);
    }

    public string Gerar(Carteira carteira, Cotacao cotacao)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== Extrato de Transações ===");

        if (_transacoes.Count == 0)
        {
            sb.AppendLine("Nenhuma transação realizada.");
        }
        else
        {
            foreach (var t in _transacoes)
            {
                sb.AppendLine(
                    $"{t.DataHora:dd/MM/yyyy HH:mm} | {t.Tipo,-6} | {Formatador.Moeda(t.ValorBrl)} | {Formatador.Btc(t.QuantidadeBtc)} | cotação {Formatador.Moeda(t.CotacaoUtilizada)}");
            }

            sb.AppendLine($"Total comprado: {Formatador.Moeda(TotalComprado)}");
            sb.AppendLine($"Total vendido: {Formatador.Moeda(TotalVendido)}");
        }

        sb.AppendLine("--------------------------------");
        sb.AppendLine($"Saldo em BRL: {Formatador.Moeda(carteira.SaldoBrl)}");
        sb.AppendLine($"Saldo em BTC: {Formatador.Btc(carteira.SaldoBtc)}");
        sb.AppendLine($"Cotação atual: {Formatador.Moeda(cotacao.PrecoBrl)}");
        sb.AppendLine($"Patrimônio total: {Formatador.Moeda(CalcularPatrimonio(carteira, cotacao))}");
        return sb.ToString();
    }
}