using System.Globalization;
using MiniBitcoin.Core;

const decimal SaldoInicialBrl = 10_000m;
const decimal CotacaoInicial = 350_000m;

var corretora = new Corretora(new Carteira(SaldoInicialBrl, 0m), new Cotacao(CotacaoInicial));

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Bem-vindo à MiniBitcoin!");
Console.WriteLine($"Saldo inicial: {Formatador.Moeda(SaldoInicialBrl)} | Cotação inicial: {Formatador.Moeda(CotacaoInicial)}");

while (true)
{
    ExibirMenu();
    var opcao = LerTexto("Escolha uma opção: ");

    if (opcao is null || opcao == "0")
    {
        Console.WriteLine("Até logo!");
        break;
    }

    try
    {
        switch (opcao)
        {
            case "1": ConsultarCotacao(); break;
            case "2": AtualizarCotacao(); break;
            case "3": VerSaldos(); break;
            case "4": DepositarBrl(); break;
            case "5": ComprarBitcoin(); break;
            case "6": VenderBitcoin(); break;
            case "7": ExibirExtrato(); break;
            default: Console.WriteLine("Opção inválida. Tente novamente."); break;
        }
    }
    catch (SaldoInsuficienteException ex)
    {
        Console.WriteLine($"[Saldo] {ex.Message}");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"[Dados inválidos] {ex.Message}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"[Operação inválida] {ex.Message}");
    }
}

void ExibirMenu()
{
    Console.WriteLine();
    Console.WriteLine("===== MiniBitcoin =====");
    Console.WriteLine("1 - Consultar cotação");
    Console.WriteLine("2 - Atualizar cotação (simulação de mercado)");
    Console.WriteLine("3 - Ver saldos");
    Console.WriteLine("4 - Depositar BRL");
    Console.WriteLine("5 - Comprar Bitcoin");
    Console.WriteLine("6 - Vender Bitcoin");
    Console.WriteLine("7 - Exibir extrato");
    Console.WriteLine("0 - Sair");
}

void ConsultarCotacao()
{
    Console.WriteLine($"Cotação atual do Bitcoin: {Formatador.Moeda(corretora.Cotacao.PrecoBrl)}");
}

void AtualizarCotacao()
{
    var novoPreco = LerDecimal("Nova cotação em BRL (ex.: 360000): ");
    corretora.Cotacao.Atualizar(novoPreco);
    Console.WriteLine($"Cotação atualizada para {Formatador.Moeda(corretora.Cotacao.PrecoBrl)}.");
}

void VerSaldos()
{
    var carteira = corretora.Carteira;
    Console.WriteLine($"Saldo em BRL: {Formatador.Moeda(carteira.SaldoBrl)}");
    Console.WriteLine($"Saldo em BTC: {Formatador.Btc(carteira.SaldoBtc)} ({carteira.SaldoSatoshis} satoshis)");
    Console.WriteLine($"Patrimônio total: {Formatador.Moeda(corretora.Extrato.CalcularPatrimonio(carteira, corretora.Cotacao))}");
}

void DepositarBrl()
{
    var valor = LerDecimal("Valor do depósito em BRL: ");
    corretora.Carteira.CreditarBrl(valor);
    Console.WriteLine($"Depósito realizado. Saldo em BRL: {Formatador.Moeda(corretora.Carteira.SaldoBrl)}");
}

void ComprarBitcoin()
{
    Console.WriteLine($"Cotação atual: {Formatador.Moeda(corretora.Cotacao.PrecoBrl)} | Saldo em BRL: {Formatador.Moeda(corretora.Carteira.SaldoBrl)}");
    var valor = LerDecimal("Quanto deseja gastar (BRL)? ");

    var transacao = corretora.Comprar(valor);
    ExibirResultadoOperacao("Compra", transacao);
}

void VenderBitcoin()
{
    Console.WriteLine($"Cotação atual: {Formatador.Moeda(corretora.Cotacao.PrecoBrl)} | Saldo em BTC: {Formatador.Btc(corretora.Carteira.SaldoBtc)}");
    var quantidade = LerDecimal("Quantos BTC deseja vender? ");

    var transacao = corretora.Vender(quantidade);
    ExibirResultadoOperacao("Venda", transacao);
}

void ExibirResultadoOperacao(string operacao, Transacao transacao)
{
    Console.WriteLine($"{operacao} realizada: {Formatador.Btc(transacao.QuantidadeBtc)} por {Formatador.Moeda(transacao.ValorBrl)}.");
}

void ExibirExtrato()
{
    Console.WriteLine();
    Console.WriteLine(corretora.Extrato.Gerar(corretora.Carteira, corretora.Cotacao));
}

string? LerTexto(string mensagem)
{
    Console.Write(mensagem);
    return Console.ReadLine()?.Trim();
}

decimal LerDecimal(string mensagem)
{
    while (true)
    {
        var entrada = LerTexto(mensagem);
        if (entrada is null) throw new InvalidOperationException("Entrada encerrada.");

        // aceita "49,90" e "49.90"
        if (decimal.TryParse(entrada.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var valor))
            return valor;

        Console.WriteLine("Valor inválido. Digite um número (ex.: 49,90).");
    }
}