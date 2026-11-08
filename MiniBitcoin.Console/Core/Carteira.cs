namespace MiniBitcoin.Core;

public class Carteira
{
    private const decimal SatoshisPorBitcoin = 100_000_000m;

    public decimal SaldoBrl { get; private set; }
    public decimal SaldoBtc { get; private set; }

    public long SaldoSatoshis => (long)(SaldoBtc * SatoshisPorBitcoin);

    public Carteira(decimal saldoBrl = 0m, decimal saldoBtc = 0m)
    {
        if (saldoBrl < 0)
            throw new ArgumentException("O saldo inicial em BRL não pode ser negativo.", nameof(saldoBrl));

        if (saldoBtc < 0)
            throw new ArgumentException("O saldo inicial em BTC não pode ser negativo.", nameof(saldoBtc));

        SaldoBrl = saldoBrl;
        SaldoBtc = saldoBtc;
    }

    public void CreditarBrl(decimal valor)
    {
        ValidarPositivo(valor, nameof(valor));
        SaldoBrl += valor;
    }

    public void CreditarBtc(decimal quantidade)
    {
        ValidarPositivo(quantidade, nameof(quantidade));
        SaldoBtc += quantidade;
    }

    public void DebitarBrl(decimal valor)
    {
        ValidarPositivo(valor, nameof(valor));

        if (valor > SaldoBrl)
            throw new SaldoInsuficienteException(
                $"Saldo em BRL insuficiente: solicitado {Formatador.Moeda(valor)}, disponível {Formatador.Moeda(SaldoBrl)}.");

        SaldoBrl -= valor;
    }

    public void DebitarBtc(decimal quantidade)
    {
        ValidarPositivo(quantidade, nameof(quantidade));

        if (quantidade > SaldoBtc)
            throw new SaldoInsuficienteException(
                $"Saldo em BTC insuficiente: solicitado {Formatador.Btc(quantidade)}, disponível {Formatador.Btc(SaldoBtc)}.");

        SaldoBtc -= quantidade;
    }

    private static void ValidarPositivo(decimal valor, string nomeParametro)
    {
        if (valor <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.", nomeParametro);
    }
}