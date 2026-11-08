namespace MiniBitcoin.Core;

/// <summary>
/// Executa ordens de compra e venda de Bitcoin usando a cotação atual e a carteira do usuário.
/// </summary>
public class Corretora
{
    private const int CasasDecimaisBtc = 8;
    private const int CasasDecimaisBrl = 2;

    public Carteira Carteira { get; }
    public Cotacao Cotacao { get; }

    public Corretora(Carteira carteira, Cotacao cotacao)
    {
        ArgumentNullException.ThrowIfNull(carteira);
        ArgumentNullException.ThrowIfNull(cotacao);

        Carteira = carteira;
        Cotacao = cotacao;
    }

    /// <summary>
    /// Compra Bitcoin gastando o valor informado em reais. A fração de BTC é arredondada para baixo (8 casas).
    /// </summary>
    public Transacao Comprar(decimal valorBrl)
    {
        if (valorBrl <= 0)
            throw new ArgumentException("O valor da compra deve ser maior que zero.", nameof(valorBrl));

        var cotacao = Cotacao.PrecoBrl;
        var quantidadeBtc = Math.Round(valorBrl / cotacao, CasasDecimaisBtc, MidpointRounding.ToZero);

        if (quantidadeBtc <= 0)
            throw new ArgumentException("O valor informado é muito baixo para comprar a menor fração de Bitcoin (1 satoshi).", nameof(valorBrl));

        Carteira.DebitarBrl(valorBrl);
        Carteira.CreditarBtc(quantidadeBtc);

        return new Transacao(TipoTransacao.Compra, valorBrl, quantidadeBtc, cotacao, DateTime.Now);
    }

    /// <summary>
    /// Vende a quantidade de Bitcoin informada e credita o valor em reais (arredondado para baixo, 2 casas).
    /// </summary>
    public Transacao Vender(decimal quantidadeBtc)
    {
        var cotacao = Cotacao.PrecoBrl;
        var valorBrl = Math.Round(quantidadeBtc * cotacao, CasasDecimaisBrl, MidpointRounding.ToZero);

        if (quantidadeBtc <= 0 || valorBrl <= 0)
            throw new ArgumentException("A quantidade de Bitcoin deve ser maior que zero e render pelo menos R$ 0,01.", nameof(quantidadeBtc));

        Carteira.DebitarBtc(quantidadeBtc);
        Carteira.CreditarBrl(valorBrl);

        return new Transacao(TipoTransacao.Venda, valorBrl, quantidadeBtc, cotacao, DateTime.Now);
    }
}