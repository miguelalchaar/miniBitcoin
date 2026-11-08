namespace MiniBitcoin.Core;

/// <summary>
/// Lançada quando uma operação tenta usar mais saldo do que a carteira possui.
/// </summary>
public class SaldoInsuficienteException : InvalidOperationException
{
    public SaldoInsuficienteException(string mensagem) : base(mensagem)
    {
    }
}