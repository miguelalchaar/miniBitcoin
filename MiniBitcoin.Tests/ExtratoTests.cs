using MiniBitcoin.Core;

namespace MiniBitcoin.Tests;

public class ExtratoTests
{
    private static Transacao CriarTransacao(TipoTransacao tipo, decimal valorBrl, decimal quantidadeBtc, decimal cotacao = 300000m)
        => new(tipo, valorBrl, quantidadeBtc, cotacao, new DateTime(2026, 11, 1, 10, 30, 0));

    // ----- Registro de transações -----

    [Fact]
    public void NovoExtrato_IniciaVazioComTotaisZerados()
    {
        var extrato = new Extrato();

        Assert.Empty(extrato.Transacoes);
        Assert.Equal(0m, extrato.TotalComprado);
        Assert.Equal(0m, extrato.TotalVendido);
    }

    [Fact]
    public void Registrar_TransacaoValida_AdicionaAoHistorico()
    {
        var extrato = new Extrato();
        var transacao = CriarTransacao(TipoTransacao.Compra, 15000m, 0.05m);

        extrato.Registrar(transacao);

        var registrada = Assert.Single(extrato.Transacoes);
        Assert.Same(transacao, registrada);
    }

    [Fact]
    public void Registrar_VariasTransacoes_MantemAOrdemDeRegistro()
    {
        var extrato = new Extrato();
        extrato.Registrar(CriarTransacao(TipoTransacao.Compra, 15000m, 0.05m));
        extrato.Registrar(CriarTransacao(TipoTransacao.Venda, 6000m, 0.02m));

        Assert.Equal(2, extrato.Transacoes.Count);
        Assert.Equal(TipoTransacao.Compra, extrato.Transacoes[0].Tipo);
        Assert.Equal(TipoTransacao.Venda, extrato.Transacoes[1].Tipo);
    }

    [Fact]
    public void Registrar_TransacaoNula_LancaArgumentNullException()
    {
        var extrato = new Extrato();

        Assert.Throws<ArgumentNullException>(() => extrato.Registrar(null!));
    }

    [Fact]
    public void TotalComprado_SomaApenasAsCompras()
    {
        var extrato = new Extrato();
        extrato.Registrar(CriarTransacao(TipoTransacao.Compra, 15000m, 0.05m));
        extrato.Registrar(CriarTransacao(TipoTransacao.Compra, 3000m, 0.01m));
        extrato.Registrar(CriarTransacao(TipoTransacao.Venda, 6000m, 0.02m));

        Assert.Equal(18000m, extrato.TotalComprado);
    }

    [Fact]
    public void TotalVendido_SomaApenasAsVendas()
    {
        var extrato = new Extrato();
        extrato.Registrar(CriarTransacao(TipoTransacao.Compra, 15000m, 0.05m));
        extrato.Registrar(CriarTransacao(TipoTransacao.Venda, 6000m, 0.02m));
        extrato.Registrar(CriarTransacao(TipoTransacao.Venda, 3000m, 0.01m));

        Assert.Equal(9000m, extrato.TotalVendido);
    }

    // ----- Patrimônio -----

    [Fact]
    public void CalcularPatrimonio_ApenasBrl_RetornaSaldoBrl()
    {
        var extrato = new Extrato();
        var carteira = new Carteira(5000m, 0m);

        Assert.Equal(5000m, extrato.CalcularPatrimonio(carteira, new Cotacao(300000m)));
    }

    [Fact]
    public void CalcularPatrimonio_ApenasBtc_ConverteBitcoinPelaCotacaoAtual()
    {
        var extrato = new Extrato();
        var carteira = new Carteira(0m, 0.5m);

        Assert.Equal(150000m, extrato.CalcularPatrimonio(carteira, new Cotacao(300000m)));
    }

    [Fact]
    public void CalcularPatrimonio_BrlEBtc_SomaOsDoisSaldos()
    {
        var extrato = new Extrato();
        var carteira = new Carteira(1000m, 0.1m);

        Assert.Equal(31000m, extrato.CalcularPatrimonio(carteira, new Cotacao(300000m)));
    }

    [Fact]
    public void CalcularPatrimonio_CotacaoAtualizada_RefleteOnovoPreco()
    {
        var extrato = new Extrato();
        var carteira = new Carteira(0m, 1m);
        var cotacao = new Cotacao(300000m);

        cotacao.Atualizar(400000m);

        Assert.Equal(400000m, extrato.CalcularPatrimonio(carteira, cotacao));
    }

    [Fact]
    public void CalcularPatrimonio_ArredondaParaBaixoEmDuasCasas()
    {
        var extrato = new Extrato();
        var carteira = new Carteira(0m, 0.00000123m);

        // 0,00000123 * 123456,78 = 0,15185... -> 0,15
        Assert.Equal(0.15m, extrato.CalcularPatrimonio(carteira, new Cotacao(123456.78m)));
    }

    [Fact]
    public void CalcularPatrimonio_CarteiraNula_LancaArgumentNullException()
    {
        var extrato = new Extrato();

        Assert.Throws<ArgumentNullException>(() => extrato.CalcularPatrimonio(null!, new Cotacao(300000m)));
    }

    // ----- Integração com a Corretora -----

    private static Corretora CriarCorretora(decimal saldoBrl = 100000m, decimal saldoBtc = 0m, decimal cotacao = 300000m)
        => new(new Carteira(saldoBrl, saldoBtc), new Cotacao(cotacao));

    [Fact]
    public void Corretora_Comprar_RegistraACompraNoExtrato()
    {
        var corretora = CriarCorretora();

        var transacao = corretora.Comprar(15000m);

        var registrada = Assert.Single(corretora.Extrato.Transacoes);
        Assert.Same(transacao, registrada);
        Assert.Equal(TipoTransacao.Compra, registrada.Tipo);
    }

    [Fact]
    public void Corretora_Vender_RegistraAVendaNoExtrato()
    {
        var corretora = CriarCorretora(saldoBtc: 1m);

        corretora.Vender(0.5m);

        var registrada = Assert.Single(corretora.Extrato.Transacoes);
        Assert.Equal(TipoTransacao.Venda, registrada.Tipo);
    }

    [Fact]
    public void Corretora_CompraEVenda_RegistramNaOrdemDasOperacoes()
    {
        var corretora = CriarCorretora();

        corretora.Comprar(30000m);
        corretora.Vender(0.05m);

        Assert.Equal(2, corretora.Extrato.Transacoes.Count);
        Assert.Equal(TipoTransacao.Compra, corretora.Extrato.Transacoes[0].Tipo);
        Assert.Equal(TipoTransacao.Venda, corretora.Extrato.Transacoes[1].Tipo);
    }

    [Fact]
    public void Corretora_OperacaoInvalida_NaoRegistraNoExtrato()
    {
        var corretora = CriarCorretora(saldoBrl: 100m);

        Assert.Throws<SaldoInsuficienteException>(() => corretora.Comprar(1000m));
        Assert.Throws<SaldoInsuficienteException>(() => corretora.Vender(1m));

        Assert.Empty(corretora.Extrato.Transacoes);
    }

    [Fact]
    public void Corretora_AposComprar_PatrimonioContinuaIgualAoInicial()
    {
        var corretora = CriarCorretora(saldoBrl: 100000m, cotacao: 300000m);

        corretora.Comprar(15000m);

        var patrimonio = corretora.Extrato.CalcularPatrimonio(corretora.Carteira, corretora.Cotacao);
        Assert.Equal(100000m, patrimonio);
    }

    [Fact]
    public void Corretora_ComprarEValorizar_PatrimonioAumenta()
    {
        var corretora = CriarCorretora(saldoBrl: 100000m, cotacao: 300000m);
        corretora.Comprar(30000m); // 0,1 BTC

        corretora.Cotacao.Atualizar(400000m);

        var patrimonio = corretora.Extrato.CalcularPatrimonio(corretora.Carteira, corretora.Cotacao);
        Assert.Equal(110000m, patrimonio); // 70.000 em BRL + 0,1 BTC * 400.000
    }

    // ----- Relatório -----

    [Fact]
    public void Gerar_ExtratoVazio_InformaQueNaoHaTransacoes()
    {
        var extrato = new Extrato();

        var texto = extrato.Gerar(new Carteira(1000m, 0m), new Cotacao(300000m));

        Assert.Contains("Nenhuma transação", texto);
        Assert.Contains(Formatador.Moeda(1000m), texto);
    }

    [Fact]
    public void Gerar_ComTransacoes_ListaOperacoesEPatrimonioTotal()
    {
        var corretora = CriarCorretora(saldoBrl: 100000m, cotacao: 300000m);
        corretora.Comprar(15000m);
        corretora.Vender(0.01m);

        var texto = corretora.Extrato.Gerar(corretora.Carteira, corretora.Cotacao);

        Assert.Contains("Compra", texto);
        Assert.Contains("Venda", texto);
        Assert.Contains(Formatador.Moeda(15000m), texto);
        Assert.Contains(Formatador.Btc(0.05m), texto);
        Assert.Contains("Patrimônio total", texto);
        Assert.Contains(Formatador.Moeda(100000m), texto);
    }

    // ----- Formatação -----

    [Theory]
    [InlineData(75, "R$ 75,00")]
    [InlineData(0, "R$ 0,00")]
    [InlineData(1234.5, "R$ 1.234,50")]
    [InlineData(350000, "R$ 350.000,00")]
    public void Moeda_FormataNoPadraoBrasileiro(double valor, string esperado)
    {
        Assert.Equal(esperado, Formatador.Moeda((decimal)valor));
    }

    [Theory]
    [InlineData(0.05, "0,05000000 BTC")]
    [InlineData(1, "1,00000000 BTC")]
    [InlineData(0.00000001, "0,00000001 BTC")]
    public void Btc_FormataComOitoCasasDecimais(double valor, string esperado)
    {
        Assert.Equal(esperado, Formatador.Btc((decimal)valor));
    }
}