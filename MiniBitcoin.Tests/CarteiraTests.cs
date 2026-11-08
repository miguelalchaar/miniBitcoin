using MiniBitcoin.Core;

namespace MiniBitcoin.Tests;

public class CarteiraTests
{
    [Fact]
    public void Construtor_SaldosValidos_CriaCarteiraComSaldosIniciais()
    {
        var carteira = new Carteira(1000m, 0.5m);

        Assert.Equal(1000m, carteira.SaldoBrl);
        Assert.Equal(0.5m, carteira.SaldoBtc);
    }

    [Fact]
    public void Construtor_SemParametros_IniciaComSaldosZerados()
    {
        var carteira = new Carteira();

        Assert.Equal(0m, carteira.SaldoBrl);
        Assert.Equal(0m, carteira.SaldoBtc);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1)]
    [InlineData(-1000)]
    public void Construtor_SaldoBrlNegativo_LancaArgumentException(double saldoBrl)
    {
        Assert.Throws<ArgumentException>(() => new Carteira((decimal)saldoBrl, 0m));
    }

    [Theory]
    [InlineData(-0.00000001)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Construtor_SaldoBtcNegativo_LancaArgumentException(double saldoBtc)
    {
        Assert.Throws<ArgumentException>(() => new Carteira(0m, (decimal)saldoBtc));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0.00000001, 1)]
    [InlineData(0.001, 100000)]
    [InlineData(1, 100000000)]
    public void SaldoSatoshis_ConverteBitcoinParaSatoshis(double saldoBtc, long esperado)
    {
        var carteira = new Carteira(0m, (decimal)saldoBtc);

        Assert.Equal(esperado, carteira.SaldoSatoshis);
    }

    [Fact]
    public void CreditarBrl_ValorValido_AumentaSaldo()
    {
        var carteira = new Carteira(100m, 0m);

        carteira.CreditarBrl(50m);

        Assert.Equal(150m, carteira.SaldoBrl);
    }

    [Fact]
    public void CreditarBtc_ValorValido_AumentaSaldo()
    {
        var carteira = new Carteira(0m, 0.1m);

        carteira.CreditarBtc(0.2m);

        Assert.Equal(0.3m, carteira.SaldoBtc);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreditarBrl_ValorInvalido_LancaArgumentException(double valor)
    {
        var carteira = new Carteira(100m, 0m);

        Assert.Throws<ArgumentException>(() => carteira.CreditarBrl((decimal)valor));
        Assert.Equal(100m, carteira.SaldoBrl);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.5)]
    public void CreditarBtc_ValorInvalido_LancaArgumentException(double valor)
    {
        var carteira = new Carteira(0m, 1m);

        Assert.Throws<ArgumentException>(() => carteira.CreditarBtc((decimal)valor));
        Assert.Equal(1m, carteira.SaldoBtc);
    }

    [Fact]
    public void DebitarBrl_ValorValido_DiminuiSaldo()
    {
        var carteira = new Carteira(100m, 0m);

        carteira.DebitarBrl(40m);

        Assert.Equal(60m, carteira.SaldoBrl);
    }

    [Fact]
    public void DebitarBrl_ValorIgualAoSaldo_ZeraSaldo()
    {
        var carteira = new Carteira(100m, 0m);

        carteira.DebitarBrl(100m);

        Assert.Equal(0m, carteira.SaldoBrl);
    }

    [Fact]
    public void DebitarBrl_ValorMaiorQueSaldo_LancaSaldoInsuficienteExceptionESemAlterarSaldo()
    {
        var carteira = new Carteira(100m, 0m);

        Assert.Throws<SaldoInsuficienteException>(() => carteira.DebitarBrl(100.01m));
        Assert.Equal(100m, carteira.SaldoBrl);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void DebitarBrl_ValorInvalido_LancaArgumentException(double valor)
    {
        var carteira = new Carteira(100m, 0m);

        Assert.Throws<ArgumentException>(() => carteira.DebitarBrl((decimal)valor));
        Assert.Equal(100m, carteira.SaldoBrl);
    }

    [Fact]
    public void DebitarBtc_ValorValido_DiminuiSaldo()
    {
        var carteira = new Carteira(0m, 1m);

        carteira.DebitarBtc(0.25m);

        Assert.Equal(0.75m, carteira.SaldoBtc);
    }

    [Fact]
    public void DebitarBtc_ValorMaiorQueSaldo_LancaSaldoInsuficienteExceptionESemAlterarSaldo()
    {
        var carteira = new Carteira(0m, 0.5m);

        Assert.Throws<SaldoInsuficienteException>(() => carteira.DebitarBtc(0.50000001m));
        Assert.Equal(0.5m, carteira.SaldoBtc);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.1)]
    public void DebitarBtc_ValorInvalido_LancaArgumentException(double valor)
    {
        var carteira = new Carteira(0m, 1m);

        Assert.Throws<ArgumentException>(() => carteira.DebitarBtc((decimal)valor));
        Assert.Equal(1m, carteira.SaldoBtc);
    }

    [Fact]
    public void DebitarBtc_CarteiraSemBitcoin_LancaSaldoInsuficienteException()
    {
        var carteira = new Carteira(1000m, 0m);

        Assert.Throws<SaldoInsuficienteException>(() => carteira.DebitarBtc(0.1m));
    }
}