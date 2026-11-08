namespace MiniBitcoin.Core;

/// <summary>
/// Registro imutável de uma operação de compra ou venda de Bitcoin.
/// </summary>
public record Transacao(
    TipoTransacao Tipo,
    decimal ValorBrl,
    decimal QuantidadeBtc,
    decimal CotacaoUtilizada,
    DateTime DataHora);