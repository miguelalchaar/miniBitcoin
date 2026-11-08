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
        if (valor <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.", nameof(valor));

        SaldoBrl += valor;
    }

    public void CreditarBtc(decimal quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));

        SaldoBtc += quantidade;
    }

    public void DebitarBrl(decimal valor)
    {
        if (valor <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.", nameof(valor));

        if (valor > SaldoBrl)
            throw new SaldoInsuficienteException($"Saldo em BRL insuficiente: solicitado {valor}, disponível {SaldoBrl}.");

        SaldoBrl -= valor;
    }

    public void DebitarBtc(decimal quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));

        if (quantidade > SaldoBtc)
            throw new SaldoInsuficienteException($"Saldo em BTC insuficiente: solicitado {quantidade}, disponível {SaldoBtc}.");

        SaldoBtc -= quantidade;
    }
}