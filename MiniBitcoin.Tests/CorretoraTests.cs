using MiniBitcoin.Core;

namespace MiniBitcoin.Tests;

public class CorretoraTests
{
    private static Corretora CriarCorretora(decimal saldoBrl = 100000m, decimal saldoBtc = 0m, decimal cotacao = 300000m)
        => new(new Carteira(saldoBrl, saldoBtc), new Cotacao(cotacao));

    // ----- Compra -----

    [Fact]
    public void Comprar_SaldoSuficiente_DebitaBrlECreditaBtc()
    {
        var corretora = CriarCorretora(saldoBrl: 100000m, cotacao: 300000m);

        corretora.Comprar(15000m);

        Assert.Equal(85000m, corretora.Carteira.SaldoBrl);
        Assert.Equal(0.05m, corretora.Carteira.SaldoBtc);
    }

    [Fact]
    public void Comprar_RetornaTransacaoDeCompraComDadosDaOperacao()
    {
        var corretora = CriarCorretora(cotacao: 300000m);

        var transacao = corretora.Comprar(15000m);

        Assert.Equal(TipoTransacao.Compra, transacao.Tipo);
        Assert.Equal(15000m, transacao.ValorBrl);
        Assert.Equal(0.05m, transacao.QuantidadeBtc);
        Assert.Equal(300000m, transacao.CotacaoUtilizada);
    }

    [Fact]
    public void Comprar_FracaoDeBitcoin_ArredondaParaBaixoEmOitoCasas()
    {
        var corretora = CriarCorretora(saldoBrl: 1000m, cotacao: 300000m);

        var transacao = corretora.Comprar(1000m);

        Assert.Equal(0.00333333m, transacao.QuantidadeBtc);
    }

    [Fact]
    public void Comprar_UsaAUltimaCotacaoAtualizada()
    {
        var corretora = CriarCorretora(saldoBrl: 100000m, cotacao: 300000m);
        corretora.Cotacao.Atualizar(200000m);

        var transacao = corretora.Comprar(50000m);

        Assert.Equal(0.25m, transacao.QuantidadeBtc);
        Assert.Equal(200000m, transacao.CotacaoUtilizada);
    }

    [Fact]
    public void Comprar_ValorTotalDoSaldo_ZeraSaldoBrl()
    {
        var corretora = CriarCorretora(saldoBrl: 30000m, cotacao: 300000m);

        corretora.Comprar(30000m);

        Assert.Equal(0m, corretora.Carteira.SaldoBrl);
        Assert.Equal(0.1m, corretora.Carteira.SaldoBtc);
    }

    [Fact]
    public void Comprar_SaldoBrlInsuficiente_LancaExcecaoESemAlterarSaldos()
    {
        var corretora = CriarCorretora(saldoBrl: 1000m, saldoBtc: 0.2m);

        Assert.Throws<SaldoInsuficienteException>(() => corretora.Comprar(1000.01m));

        Assert.Equal(1000m, corretora.Carteira.SaldoBrl);
        Assert.Equal(0.2m, corretora.Carteira.SaldoBtc);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Comprar_ValorInvalido_LancaArgumentException(double valor)
    {
        var corretora = CriarCorretora();

        Assert.Throws<ArgumentException>(() => corretora.Comprar((decimal)valor));
    }

    [Fact]
    public void Comprar_ValorMuitoPequenoParaGerarBitcoin_LancaArgumentExceptionESemAlterarSaldos()
    {
        var corretora = CriarCorretora(saldoBrl: 100m, cotacao: 300000m);

        // 0,0001 BRL / 300.000 = menos de 1 satoshi
        Assert.Throws<ArgumentException>(() => corretora.Comprar(0.0001m));

        Assert.Equal(100m, corretora.Carteira.SaldoBrl);
        Assert.Equal(0m, corretora.Carteira.SaldoBtc);
    }

    // ----- Venda -----

    [Fact]
    public void Vender_SaldoSuficiente_DebitaBtcECreditaBrl()
    {
        var corretora = CriarCorretora(saldoBrl: 0m, saldoBtc: 1m, cotacao: 300000m);

        corretora.Vender(0.5m);

        Assert.Equal(0.5m, corretora.Carteira.SaldoBtc);
        Assert.Equal(150000m, corretora.Carteira.SaldoBrl);
    }

    [Fact]
    public void Vender_RetornaTransacaoDeVendaComDadosDaOperacao()
    {
        var corretora = CriarCorretora(saldoBtc: 1m, cotacao: 300000m);

        var transacao = corretora.Vender(0.5m);

        Assert.Equal(TipoTransacao.Venda, transacao.Tipo);
        Assert.Equal(150000m, transacao.ValorBrl);
        Assert.Equal(0.5m, transacao.QuantidadeBtc);
        Assert.Equal(300000m, transacao.CotacaoUtilizada);
    }

    [Fact]
    public void Vender_ValorEmBrlArredondadoParaBaixoEmDuasCasas()
    {
        var corretora = CriarCorretora(saldoBrl: 0m, saldoBtc: 1m, cotacao: 123456.78m);

        var transacao = corretora.Vender(0.00000123m);

        // 0,00000123 * 123456,78 = 0,15185... -> 0,15
        Assert.Equal(0.15m, transacao.ValorBrl);
    }

    [Fact]
    public void Vender_TodoOBitcoin_ZeraSaldoBtc()
    {
        var corretora = CriarCorretora(saldoBtc: 0.3m, cotacao: 300000m);

        corretora.Vender(0.3m);

        Assert.Equal(0m, corretora.Carteira.SaldoBtc);
    }

    [Fact]
    public void Vender_SaldoBtcInsuficiente_LancaExcecaoESemAlterarSaldos()
    {
        var corretora = CriarCorretora(saldoBrl: 500m, saldoBtc: 0.1m);

        Assert.Throws<SaldoInsuficienteException>(() => corretora.Vender(0.10000001m));

        Assert.Equal(0.1m, corretora.Carteira.SaldoBtc);
        Assert.Equal(500m, corretora.Carteira.SaldoBrl);
    }

    [Fact]
    public void Vender_CarteiraSemBitcoin_LancaSaldoInsuficienteException()
    {
        var corretora = CriarCorretora(saldoBtc: 0m);

        Assert.Throws<SaldoInsuficienteException>(() => corretora.Vender(0.01m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.5)]
    public void Vender_QuantidadeInvalida_LancaArgumentException(double quantidade)
    {
        var corretora = CriarCorretora(saldoBtc: 1m);

        Assert.Throws<ArgumentException>(() => corretora.Vender((decimal)quantidade));
    }

    // ----- Construtor -----

    [Fact]
    public void Construtor_CarteiraNula_LancaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Corretora(null!, new Cotacao(300000m)));
    }

    [Fact]
    public void Construtor_CotacaoNula_LancaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Corretora(new Carteira(), null!));
    }
}