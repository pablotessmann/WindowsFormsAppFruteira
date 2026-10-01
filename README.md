# Fruteira — Cadastro e Estoque de Frutas

Aplicação desktop em **C# / Windows Forms** para o controle de uma fruteira:
cadastro de frutas com foto e consulta do estoque com filtros.

Projeto desenvolvido no **Senac** como material de curso técnico. Além de
funcionar, o código é propositalmente comentado em detalhe e acompanha um guia
didático passo a passo — o repositório serve tanto como sistema quanto como
material de aula para quem está no primeiro contato com programação.

---

## As duas telas

### Cadastro de Frutas

Tela inicial do sistema. Recebe os dados da fruta e a imagem:

- **Nome**, **Quantidade**, **Tipo**, **Valor** e **Validade**
- Botão de **upload de imagem** com prévia ao lado, em tamanho grande
- Validação de todos os campos antes de cadastrar, com mensagem específica e
  foco devolvido ao campo com problema
- Ao concluir, o sistema leva o usuário direto para a tela de Estoque

### Estoque de Frutas

Consulta do que está cadastrado:

- Dois filtros de pesquisa: **Categoria** e **Nome**
- Resultado montado em uma tabela com as colunas
  **Fruta · Quantidade · Validade · Valor · Tipo**
- Botão **Voltar** para retornar ao cadastro

---

## Tecnologias

| | |
|---|---|
| Linguagem | C# |
| Interface | Windows Forms |
| Plataforma | .NET Framework 4.7.2 |
| IDE | Visual Studio 2022 |

---

## Estrutura do projeto

| Arquivo | Responsabilidade |
|---|---|
| `Program.cs` | Ponto de entrada; abre a tela de Cadastro |
| `Fruta.cs` | Modelo da fruta e a lista reserva de tipos |
| `Categoria.cs` | Modelo da categoria devolvida pela API |
| `FrutaApi.cs` | Único ponto de comunicação com a API |
| `CadastroForm.cs` | Regras da tela de cadastro |
| `CadastroForm.Designer.cs` | Layout da tela de cadastro (gerado pelo Designer) |
| `EstoqueForm.cs` | Regras da tela de estoque |
| `EstoqueForm.Designer.cs` | Layout da tela de estoque (gerado pelo Designer) |
| `GUIA.md` | Guia didático em 12 etapas |

---

## Como executar

Abra `WindowsFormsAppFruteira.sln` no Visual Studio e pressione **F5**.

Pela linha de comando do Windows:

```cmd
msbuild WindowsFormsAppFruteira.sln /p:Configuration=Debug
bin\Debug\WindowsFormsAppFruteira.exe
```

> `dotnet build` **não** funciona neste projeto: ele é .NET **Framework** 4.7.2,
> que roda apenas no Windows e se compila com MSBuild.

---

## Estado atual

Todas as operações de dados são feitas por **API**.

As **categorias** (que preenchem o campo Tipo do Cadastro e o filtro Categoria
do Estoque) já vêm de uma API real, mantida em outro projeto:

```
GET  https://localhost:7069/Categoria         lista as categorias
```

Se essa API estiver fora do ar, o sistema usa a lista reserva `Fruta.Tipos`.

A API de **frutas** **ainda não está implementada**. Em `FrutaApi.cs`, cada
chamada esperada está documentada e **comentada**, pronta para ser ativada:

```
POST /api/frutas                              cadastra uma fruta (multipart/form-data)
GET  /api/frutas?categoria=&nome=             pesquisa o estoque
```

Por isso, ao executar o sistema hoje: o cadastro valida os dados e navega para o
Estoque normalmente, mas a pesquisa responde *"Nenhuma fruta encontrada"* —
esse é o comportamento esperado enquanto não há servidor de frutas.

O passo a passo para ligar a API de verdade está na **etapa 10** do guia.

---

## Documentação

O **[GUIA.md](GUIA.md)** explica a construção do projeto em 12 etapas, cada uma
no formato *o que fazer → o código → por que funciona*, com o vocabulário
explicado no primeiro uso:

1. Como o projeto está organizado
2. Criar a classe `Fruta`
3. Nomear os controles direito
4. Montar a tela de Cadastro
5. Upload e prévia da imagem
6. Validar antes de cadastrar
7. Navegar entre as telas
8. Montar a tela de Estoque
9. Os filtros Categoria e Nome
10. Onde a API entra
11. Compilar e executar
12. Exercícios propostos
