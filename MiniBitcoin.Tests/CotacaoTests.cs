using MiniBitcoin.Core;

namespace MiniBitcoin.Tests;

public class CotacaoTests
{
    [Fact]
    public void Construtor_PrecoValido_DefinePrecoAtual()
    {
        var cotacao = new Cotacao(350000m);

        Assert.Equal(350000m, cotacao.PrecoBrl);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-350000)]
    public void Construtor_PrecoInvalido_LancaArgumentException(double preco)
    {
        Assert.Throws<ArgumentException>(() => new Cotacao((decimal)preco));
    }

    [Fact]
    public void Atualizar_PrecoValido_AlteraPrecoAtual()
    {
        var cotacao = new Cotacao(350000m);

        cotacao.Atualizar(400000m);

        Assert.Equal(400000m, cotacao.PrecoBrl);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Atualizar_PrecoInvalido_LancaArgumentExceptionEMantemPreco(double novoPreco)
    {
        var cotacao = new Cotacao(350000m);

        Assert.Throws<ArgumentException>(() => cotacao.Atualizar((decimal)novoPreco));
        Assert.Equal(350000m, cotacao.PrecoBrl);
    }
}