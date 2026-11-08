# MiniBitcoin

![C#](https://img.shields.io/badge/C%23-.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-Testes%20Unit%C3%A1rios-5E2D91?style=for-the-badge&logo=dotnet&logoColor=white)
![Git](https://img.shields.io/badge/Git-Versionamento-orange?style=for-the-badge&logo=git&logoColor=white)
![GitHub](https://img.shields.io/badge/GitHub-Reposit%C3%B3rio-black?style=for-the-badge&logo=github&logoColor=white)
![Status](https://img.shields.io/badge/Status-Conclu%C3%ADda-green?style=for-the-badge)

Projeto desenvolvido para a disciplina de **Garantia da Qualidade de Software**, com foco na aplicação prática de **testes unitários com xUnit**, utilizando uma solução **.NET** e ferramentas de versionamento de código.

A atividade consiste no desenvolvimento de uma **carteira de Bitcoin simulada em console**, com funcionalidades para gestão de saldos em reais (BRL) e Bitcoin, cotação de mercado, compra e venda de Bitcoin e extrato de transações com patrimônio consolidado.

Além da implementação das funcionalidades, o projeto tem como objetivo compreender a importância dos testes automatizados na validação das regras de negócio e utilizar o Git com o padrão **Conventional Commits** para registrar a evolução do software.

## ▸ Conceito da atividade

A atividade consiste na criação de um repositório chamado `MiniBitcoin` e na implementação de uma solução composta por dois projetos:

- `MiniBitcoin.Console`: aplicação de console que contém as entidades, as regras de negócio e a interface de interação.
- `MiniBitcoin.Tests`: projeto de testes unitários responsável por verificar o comportamento das funcionalidades implementadas.

O projeto de aplicação concentra as regras relacionadas à carteira e às operações, enquanto o projeto de testes valida essas regras de forma automatizada.

Essa separação permite testar a lógica do sistema independentemente da interação pelo terminal, facilitando a identificação de erros e a manutenção do código.

## ▸ Camadas da solução

### `MiniBitcoin.Console` — Código de Produção

O projeto `MiniBitcoin.Console` contém a aplicação principal e as regras de negócio da carteira. As regras ficam na pasta `Core` (namespace `MiniBitcoin.Core`) e não dependem de entrada ou saída de dados.

Neste projeto são implementados:

- A classe `Carteira`, responsável pelos saldos em BRL e BTC e pelas validações de créditos e débitos.
- A classe `Cotacao`, responsável pelo preço atual do Bitcoin (simulado e atualizável).
- A classe `Corretora`, responsável por executar as ordens de compra e venda usando a cotação atual.
- O registro `Transacao` e o enum `TipoTransacao`, que representam cada operação realizada.
- A classe `Extrato`, responsável pelo histórico de operações e pelo cálculo do patrimônio total.
- A classe `Formatador`, responsável por apresentar valores em reais (`R$ 1.234,50`) e em Bitcoin (`0,05000000 BTC`).
- A exceção `SaldoInsuficienteException`, usada quando a operação exige mais saldo do que o disponível.
- O arquivo `Program.cs`, responsável pelo menu interativo.

### `MiniBitcoin.Tests` — Testes Unitários

O projeto `MiniBitcoin.Tests` representa a camada de validação automatizada.

Neste projeto são implementados:

- Testes com os atributos `[Fact]` e `[Theory]` com `[InlineData]`.
- Verificações com `Assert.Equal`, `Assert.Single`, `Assert.Empty`, `Assert.Same`, `Assert.Contains` e `Assert.Throws`.
- Testes para impedir saldos iniciais negativos e débitos inválidos ou acima do saldo.
- Testes para validar a compra de frações de Bitcoin e a venda com saldo suficiente.
- Testes para garantir que operações inválidas não alterem os saldos.
- Testes para conferir o registro das operações no extrato e o cálculo do patrimônio.
- Testes para validar a formatação de valores em reais e em Bitcoin.

Os testes permitem verificar se o comportamento do sistema corresponde às regras estabelecidas, inclusive após futuras alterações no código.

## ▸ Funcionalidades da carteira

A aplicação disponibiliza as seguintes funcionalidades:

| Funcionalidade         | Descrição                                                                          |
| ---------------------- | ---------------------------------------------------------------------------------- |
| Gestão de carteira     | Controla o saldo em BRL e em Bitcoin (também exibido em satoshis).                 |
| Validação de saldos    | Impede saldos iniciais negativos, valores inválidos e débitos acima do saldo.      |
| Cotação de mercado     | Mantém e permite consultar e atualizar o preço atual do Bitcoin em BRL.            |
| Compra de Bitcoin      | Converte o valor em reais em frações de BTC pela cotação atual, validando o saldo. |
| Venda de Bitcoin       | Converte BTC em reais pela cotação atual, validando o saldo em Bitcoin.            |
| Operações atômicas     | Se uma operação é inválida, nenhum saldo é alterado e nada é registrado.           |
| Extrato de transações  | Exibe o histórico de compras e vendas e os totais movimentados.                    |
| Patrimônio consolidado | Calcula o saldo em BRL somado ao saldo em BTC convertido pela cotação atual.       |

## ▸ Regras de negócio

As classes `Carteira`, `Cotacao`, `Corretora` e `Extrato` centralizam as principais regras da aplicação.

| Classe      | Método                                           | Retorno     | Regra                                                                                  |
| ----------- | ------------------------------------------------ | ----------- | -------------------------------------------------------------------------------------- |
| `Carteira`  | `Carteira(saldoBrl, saldoBtc)`                   | —           | Cria a carteira com saldos iniciais; valores negativos são rejeitados.                 |
| `Carteira`  | `CreditarBrl(valor)` / `CreditarBtc(quantidade)` | `void`      | Aumenta o saldo; valores menores ou iguais a zero são rejeitados.                      |
| `Carteira`  | `DebitarBrl(valor)` / `DebitarBtc(quantidade)`   | `void`      | Diminui o saldo; lança exceção se o valor for inválido ou maior que o saldo.           |
| `Carteira`  | `SaldoSatoshis`                                  | `long`      | Converte o saldo em Bitcoin para satoshis (1 BTC = 100.000.000 satoshis).              |
| `Cotacao`   | `Atualizar(novoPrecoBrl)`                        | `void`      | Altera o preço atual do Bitcoin; preços menores ou iguais a zero são rejeitados.       |
| `Corretora` | `Comprar(valorBrl)`                              | `Transacao` | Debita o BRL, credita o BTC (arredondado para baixo em 8 casas) e registra no extrato. |
| `Corretora` | `Vender(quantidadeBtc)`                          | `Transacao` | Debita o BTC, credita o BRL (arredondado para baixo em 2 casas) e registra no extrato. |
| `Extrato`   | `Registrar(transacao)`                           | `void`      | Adiciona a operação ao histórico, mantendo a ordem de registro.                        |
| `Extrato`   | `CalcularPatrimonio(carteira, cotacao)`          | `decimal`   | Soma o saldo em BRL ao saldo em BTC convertido pela cotação atual.                     |
| `Extrato`   | `Gerar(carteira, cotacao)`                       | `string`    | Gera o extrato com as operações, os totais, os saldos e o patrimônio.                  |

### Exemplo de funcionamento

Considere uma carteira com R$ 100.000,00 e a cotação do Bitcoin em R$ 300.000,00:

```text
Compra de R$ 15.000,00  ->  0,05000000 BTC
Saldo em BRL: R$ 85.000,00
Saldo em BTC: 0,05000000 BTC
Patrimônio total: R$ 100.000,00

A cotação sobe para R$ 400.000,00.
Patrimônio total: R$ 85.000,00 + 0,05 BTC x R$ 400.000,00 = R$ 105.000,00
```

O extrato esperado será semelhante a:

```text
=== Extrato de Transações ===
08/11/2026 21:16 | Compra | R$ 15.000,00 | 0,05000000 BTC | cotação R$ 300.000,00
Total comprado: R$ 15.000,00
Total vendido: R$ 0,00
--------------------------------
Saldo em BRL: R$ 85.000,00
Saldo em BTC: 0,05000000 BTC
Cotação atual: R$ 400.000,00
Patrimônio total: R$ 105.000,00
```

O exemplo é ilustrativo e representa o resultado esperado para os dados apresentados.

## ▸ Testes unitários realizados

Os testes estão organizados em classes de acordo com as funcionalidades verificadas.

| Classe de teste  | Cenários validados                                                                                                         | Objetivo                                      |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------- |
| `CarteiraTests`  | Saldos iniciais, saldos negativos, satoshis, créditos e débitos válidos, inválidos e acima do saldo                        | Validar as regras da carteira.                |
| `CotacaoTests`   | Preço válido, preço inválido e atualização da cotação                                                                      | Validar a manutenção da cotação.              |
| `CorretoraTests` | Compra e venda com sucesso, arredondamento, cotação atualizada, saldo insuficiente, valores inválidos e saldos preservados | Validar as operações de compra e venda.       |
| `ExtratoTests`   | Registro e ordem das transações, totais, patrimônio, integração com a corretora, relatório e formatação                    | Validar o extrato e o patrimônio consolidado. |

### Testes da carteira

Verificam se a carteira aceita saldos válidos, rejeita valores negativos e impede débitos acima do saldo, sem alterar os valores em caso de erro.

### Testes de compra e venda

Verificam a conversão entre reais e Bitcoin pela cotação atual, o arredondamento das frações, a rejeição de valores inválidos e a preservação dos saldos quando a operação falha.

### Testes do extrato

Verificam se cada operação é registrada na ordem correta, se operações inválidas não aparecem no histórico e se o patrimônio reflete a cotação atual.

### Execução dos testes

Para executar todos os testes automatizados, utilize:

```bash
dotnet test
```

O resultado esperado é que todos os testes implementados sejam executados com sucesso, sem falhas.

## ▸ Versionamento e qualidade de software

O projeto utiliza o Git para acompanhar as alterações e o padrão **Conventional Commits** para padronizar as mensagens de commit.

| Conceito                  | Aplicação no projeto                                                 |
| ------------------------- | -------------------------------------------------------------------- |
| **Solução (`.sln`)**      | Agrupa os projetos de aplicação e de testes.                         |
| **Teste unitário**        | Valida individualmente as regras de negócio.                         |
| **xUnit**                 | Framework utilizado para implementar e executar os testes.           |
| **`[Fact]` e `[Theory]`** | Identificam testes simples e testes com vários conjuntos de dados.   |
| **`Assert`**              | Compara o comportamento obtido com o esperado.                       |
| **TDD básico**            | Cada etapa começa com o commit dos testes, seguido da implementação. |
| **Repositório Git**       | Mantém o histórico de alterações do código.                          |
| **Conventional Commits**  | Padroniza as mensagens de commit.                                    |
| **`git add`**             | Seleciona as alterações que serão registradas.                       |
| **`git commit`**          | Registra um conjunto de alterações no histórico.                     |
| **`git push`**            | Envia os commits para o repositório remoto.                          |
| **`.gitignore`**          | Evita versionar arquivos gerados, como `bin/` e `obj/`.              |
| **README.md**             | Documenta o projeto e as instruções de execução.                     |

### Padrão de commits

Os commits seguem a estrutura:

```text
tipo(escopo): descrição da alteração
```

Exemplos utilizados no desenvolvimento:

```bash
git commit -m "chore: initial project structure with console and xunit apps"

git commit -m "test(carteira): add unit tests for wallet balance validation"

git commit -m "feat(carteira): implement wallet entity and initial balance rules"

git commit -m "test(transacao): add unit tests for bitcoin buy and sell operations"

git commit -m "feat(transacao): implement bitcoin purchase and sale logic with quote validation"

git commit -m "test(extrato): add unit tests for transaction ledger and portfolio valuation"

git commit -m "feat(extrato): implement transaction history and portfolio summary"

git commit -m "feat(console): implement interactive menu for bitcoin wallet application"

git commit -m "refactor(core): improve code readability and organize namespaces"

git commit -m "docs: add comprehensive instructions in README.md"
```

Os commits devem ser realizados após as respectivas etapas, com as alterações correspondentes já implementadas e verificadas.

## ▸ Configuração via .NET CLI

Os comandos abaixo demonstram como criar a solução e os projetos utilizando a linha de comando.

```bash
# 1. Cria a solução
dotnet new sln -n MiniBitcoin

# 2. Cria o projeto de console
dotnet new console -n MiniBitcoin.Console

# 3. Cria o projeto de testes com xUnit
dotnet new xunit -n MiniBitcoin.Tests

# 4. Adiciona os projetos à solução
dotnet sln add MiniBitcoin.Console/MiniBitcoin.Console.csproj
dotnet sln add MiniBitcoin.Tests/MiniBitcoin.Tests.csproj

# 5. Adiciona a referência da aplicação no projeto de testes
dotnet add MiniBitcoin.Tests/MiniBitcoin.Tests.csproj reference MiniBitcoin.Console/MiniBitcoin.Console.csproj
```

**Observação:** os comandos de criação são destinados à configuração inicial. Não é necessário executá-los novamente se a solução e os projetos já tiverem sido criados.

## ▸ Como executar

**Pré-requisito:** possuir o [SDK do .NET](https://dotnet.microsoft.com/download) instalado na máquina, na versão configurada nos projetos (`TargetFramework` dos arquivos `.csproj`).

### 1. Clonar o repositório

```bash
git clone https://github.com/miguelalchaar/MiniBitcoin
```

### 2. Acessar a pasta do projeto

```bash
cd MiniBitcoin
```

### 3. Restaurar as dependências

```bash
dotnet restore
```

### 4. Executar os testes unitários

```bash
dotnet test
```

### 5. Executar a aplicação

```bash
dotnet run --project MiniBitcoin.Console/MiniBitcoin.Console.csproj
```

### 6. Conferir o histórico de commits

```bash
git log --oneline
```

Para verificar se existem alterações locais pendentes:

```bash
git status
```

### Menu da aplicação

A carteira inicia com R$ 10.000,00 em BRL, nenhum Bitcoin e cotação de R$ 350.000,00.

| Opção | Ação                                     |
| ----- | ---------------------------------------- |
| 1     | Consultar cotação                        |
| 2     | Atualizar cotação (simulação de mercado) |
| 3     | Ver saldos                               |
| 4     | Depositar BRL                            |
| 5     | Comprar Bitcoin                          |
| 6     | Vender Bitcoin                           |
| 7     | Exibir extrato                           |
| 0     | Sair                                     |

## ▸ Estrutura do projeto

```text
MiniBitcoin/
├── MiniBitcoin.sln
├── MiniBitcoin.Console/
│   ├── Program.cs
│   └── Core/
│       ├── Carteira.cs
│       ├── Cotacao.cs
│       ├── Corretora.cs
│       ├── Transacao.cs
│       ├── TipoTransacao.cs
│       ├── Extrato.cs
│       ├── Formatador.cs
│       └── SaldoInsuficienteException.cs
└── MiniBitcoin.Tests/
    ├── CarteiraTests.cs
    ├── CotacaoTests.cs
    ├── CorretoraTests.cs
    └── ExtratoTests.cs
```

## ▸ Licença

Este projeto está destinado a fins acadêmicos e educacionais.

## ▸ Autor

**Miguel Alchaar**

Desenvolvido como atividade acadêmica da disciplina de **Garantia da Qualidade de Software**, com foco em testes unitários, regras de negócio e versionamento de código.
