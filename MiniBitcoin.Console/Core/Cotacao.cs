namespace MiniBitcoin.Core;

/// <summary>
/// Cotação atual do Bitcoin em reais (simulada e atualizável pelo sistema).
/// </summary>
public class Cotacao
{
    public decimal PrecoBrl { get; private set; }

    public Cotacao(decimal precoBrl)
    {
        ValidarPreco(precoBrl);
        PrecoBrl = precoBrl;
    }

    public void Atualizar(decimal novoPrecoBrl)
    {
        ValidarPreco(novoPrecoBrl);
        PrecoBrl = novoPrecoBrl;
    }

    private static void ValidarPreco(decimal preco)
    {
        if (preco <= 0)
            throw new ArgumentException("A cotação do Bitcoin deve ser maior que zero.", nameof(preco));
    }
}