# Guia passo a passo — Fruteira (C# / Windows Forms)

Leia na ordem. Ao final você deve conseguir refazer o projeto do zero.

---

## Sumário

1. [Como o projeto está organizado](#etapa-1--como-o-projeto-está-organizado)
2. [Criar a classe `Fruta`](#etapa-2--criar-a-classe-fruta)
3. [Nomear os controles direito](#etapa-3--nomear-os-controles-direito)
4. [Montar a tela de Cadastro](#etapa-4--montar-a-tela-de-cadastro)
5. [Upload e prévia da imagem](#etapa-5--upload-e-prévia-da-imagem)
6. [Validar antes de cadastrar](#etapa-6--validar-antes-de-cadastrar)
7. [Navegar entre as telas](#etapa-7--navegar-entre-as-telas)
8. [Montar a tela de Estoque](#etapa-8--montar-a-tela-de-estoque)
9. [Os filtros Categoria e Nome](#etapa-9--os-filtros-categoria-e-nome)
10. [Onde a API entra](#etapa-10--onde-a-api-entra)
11. [Compilar e executar](#etapa-11--compilar-e-executar)
12. [Exercícios](#etapa-12--exercícios)

---

## Etapa 1 — Como o projeto está organizado

Abra a pasta do projeto. Os arquivos que importam são estes:

| Arquivo | Para que serve |
|---|---|
| `Program.cs` | O **ponto de partida** do programa. É a primeira coisa que roda. |
| `Fruta.cs` | A **classe de modelo**: representa uma fruta. |
| `Categoria.cs` | Outra classe de modelo: uma categoria, como a API devolve. |
| `FrutaApi.cs` | As **chamadas para a API** (o servidor que guarda os dados). |
| `CadastroForm.cs` | O **seu código** da tela de cadastro. |
| `CadastroForm.Designer.cs` | O **desenho** da tela de cadastro (gerado pelo Visual Studio). |
| `EstoqueForm.cs` | O seu código da tela de estoque. |
| `EstoqueForm.Designer.cs` | O desenho da tela de estoque. |

### O par `Form.cs` × `Form.Designer.cs`

Cada tela é formada por **dois arquivos**. Isso confunde no começo, então entenda bem:

- **`CadastroForm.Designer.cs`** guarda o *visual*: quais botões existem, em que
  posição, de que tamanho, com que texto. Quando você arrasta um botão no
  Visual Studio, é **este** arquivo que o Visual Studio reescreve sozinho.
- **`CadastroForm.cs`** guarda o *comportamento*: o que acontece quando o
  usuário clica no botão.

> ⚠️ **Nunca escreva sua lógica no `.Designer.cs`.** Na próxima vez que alguém
> mexer na tela pelo Designer, o Visual Studio reescreve o arquivo e seu
> código pode ir embora.

Os dois arquivos formam **uma única classe**, dividida em dois pedaços. Isso é
possível por causa da palavra `partial`:

```csharp
// em CadastroForm.cs
public partial class CadastroForm : Form { ... }

// em CadastroForm.Designer.cs
partial class CadastroForm { ... }
```

`partial` = "parcial", ou seja, "esta classe continua em outro arquivo".
Para o compilador, os dois pedaços juntos são uma só classe.

### O que é `Program.cs`

```csharp
static void Main()
{
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.Run(new CadastroForm());   // <-- abre a tela de Cadastro
}
```

`Main` é o **método de entrada**: todo programa em C# começa por ele.
`Application.Run(new CadastroForm())` significa "crie a tela de Cadastro e
mantenha o programa vivo enquanto ela estiver aberta".

---

## Etapa 2 — Criar a classe `Fruta`

**O que fazer:** criar o arquivo `Fruta.cs`.

### Por que não usar variáveis soltas

Sem uma classe, para guardar uma fruta você precisaria de cinco variáveis:

```csharp
string nome;
int quantidade;
string tipo;
decimal valor;
DateTime validade;
```

E para passar essa fruta para outro método, teria que passar cinco parâmetros.
Com duas frutas, dez variáveis. Isso não escala.

Uma **classe** é um molde que agrupa informações que andam juntas:

```csharp
public class Fruta
{
    public string Nome { get; set; }
    public int Quantidade { get; set; }
    public string Tipo { get; set; }
    public decimal Valor { get; set; }
    public DateTime Validade { get; set; }
    public string CaminhoImagem { get; set; }
}
```

Agora uma fruta é **um** objeto:

```csharp
Fruta f = new Fruta();      // cria a fruta na memória
f.Nome = "Banana Prata";    // preenche uma informação
```

### Vocabulário desta etapa

| Termo | Significado |
|---|---|
| **classe** | O molde, a receita. `Fruta` é a classe. |
| **objeto** / **instância** | Uma fruta concreta feita a partir do molde. |
| **`new`** | O comando que cria o objeto na memória. |
| **propriedade** | Uma informação guardada no objeto (`Nome`, `Valor`...). |
| **`{ get; set; }`** | Atalho que diz "esta propriedade pode ser lida (`get`) e escrita (`set`)". |
| **`public`** | Qualquer parte do programa pode usar. |

### A escolha de cada tipo de dado

Isto **não** é detalhe:

- `Quantidade` é `int` (número inteiro) porque não existe "meia caixa".
- `Valor` é `decimal`, **não** `double`. `double` faz arredondamentos estranhos
  com dinheiro (`0.1 + 0.2` pode dar `0.30000000000000004`). Para dinheiro,
  sempre `decimal`.
- `Validade` é `DateTime`, **não** `string`. Sendo `DateTime` você pode comparar
  datas (`if (fruta.Validade < DateTime.Today)`). Como texto, não conseguiria.

### A lista de tipos

No fim da classe existe isto:

```csharp
public static readonly string[] Tipos =
{
    "Cítrica", "Tropical", "Vermelha", "Comum", "Seca"
};
```

- `static` = pertence à **classe**, não a uma fruta específica. Você escreve
  `Fruta.Tipos`, e não `minhaFruta.Tipos`.
- `readonly` = ninguém pode trocar a lista depois que o programa inicia.

Essa é a **lista reserva**. A lista de verdade vem da **API de categorias**
(etapa 10); esta só entra em cena quando a API não responde, para as listas
suspensas não ficarem vazias.

As telas não leem `Fruta.Tipos` direto. As duas chamam o mesmo método:

```csharp
FrutaApi.ObterTipos()
```

Esse método é a **fonte única da verdade**. A tela de Cadastro o usa para
preencher o campo Tipo, e a tela de Estoque o usa para preencher o filtro
Categoria. Assim é impossível cadastrar `"Acida"` (sem acento) e depois
pesquisar `"Ácida"` (com acento) e não achar nada.

---

## Etapa 3 — Nomear os controles direito

Quando você arrasta controles no Designer, o Visual Studio dá nomes automáticos:
`textBox1`, `textBox2`, `button1`, `label3`...

**Esses nomes são um problema.** Olhe este código e diga o que ele faz:

```csharp
if (textBox3.Text == "") { ... }
```

Impossível saber. Agora olhe:

```csharp
if (txtValor.Text == "") { ... }
```

Renomeamos **todos** os controles usando um **prefixo por tipo**:

| Prefixo | Tipo de controle | Exemplo |
|---|---|---|
| `lbl` | `Label` (texto fixo na tela) | `lblValor` |
| `txt` | `TextBox` (caixa de digitar) | `txtValor` |
| `btn` | `Button` (botão) | `btnCadastrar` |
| `cmb` | `ComboBox` (lista suspensa) | `cmbTipo` |
| `dtp` | `DateTimePicker` (seletor de data) | `dtpValidade` |
| `pic` | `PictureBox` (moldura de imagem) | `picPrevia` |
| `lst` | `ListView` (tabela) | `lstEstoque` |
| `grp` | `GroupBox` (moldura com título) | `grpPesquisar` |

### Como renomear sem quebrar nada

No Visual Studio: clique no controle, painel **Propriedades**, mude a
propriedade **`(Name)`**. O Visual Studio corrige todos os usos sozinho.

Se for renomear **na mão**, cuidado: cada controle aparece em **5 lugares** do
`.Designer.cs`. Todos precisam mudar:

1. A **declaração**, no fim do arquivo:
   `private System.Windows.Forms.TextBox txtValor;`
2. O **`new`**, no começo do `InitializeComponent`:
   `this.txtValor = new System.Windows.Forms.TextBox();`
3. A propriedade **`Name`**: `this.txtValor.Name = "txtValor";`
4. O **`Controls.Add`**: `this.Controls.Add(this.txtValor);`
5. A ligação do **evento**, se houver: `this.btnCadastrar.Click += ...`

---

## Etapa 4 — Montar a tela de Cadastro

A tela ficou assim:

```
 CadastroForm  (831 x 503)
 +---------------------------------------------------------------+
 | Cadastro de Frutas                                            |
 |                                                               |
 |  +-------------------------+      Nome        [____________]  |
 |  |                         |      Quantidade  [____________]  |
 |  |   picPrevia             |      Tipo        [v___________]  |
 |  |   (prévia da imagem)    |      Valor       [____________]  |
 |  |                         |      Validade    [dd/mm/aaaa v]  |
 |  +-------------------------+                                  |
 |  [ Escolher imagem ]                                          |
 |                                                               |
 |  [ Estoque ]        [ Cancelar ]          [ Cadastrar ]        |
 +---------------------------------------------------------------+
```

### Duas escolhas de controle que valem explicação

**Validade usa `DateTimePicker`, não `TextBox`.**
Com uma caixa de texto, o usuário pode digitar `31/02/2026` (31 de fevereiro
não existe) ou `banana`, e você teria que validar tudo isso na mão. O
`DateTimePicker` abre um calendário: **é impossível escolher data inválida**, e
ele já entrega um `DateTime` pronto.

**Tipo usa `ComboBox`, não `TextBox`.**
O Tipo é o campo que o filtro "Categoria" do Estoque pesquisa. Se fosse texto
livre, um erro de digitação no cadastro faria a fruta nunca aparecer na
pesquisa. Com a lista fechada, só existem os valores que
`FrutaApi.ObterTipos()` devolve.

A propriedade que fecha a lista é esta:

```csharp
cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
```

Sem ela, o `ComboBox` aceitaria digitação livre e o problema voltaria.

### O evento `Load`

Algumas coisas precisam acontecer **quando a tela abre**, antes do usuário ver
qualquer coisa. Isso vai no evento `Load`:

```csharp
private void CadastroForm_Load(object sender, EventArgs e)
{
    cmbTipo.Items.AddRange(FrutaApi.ObterTipos());   // preenche a lista suspensa
    dtpValidade.MinDate = DateTime.Today;            // proíbe data no passado
    dtpValidade.Value = DateTime.Today;
    txtNome.Focus();                                 // cursor já no primeiro campo
}
```

### Vocabulário: o que é um evento

Um **evento** é um aviso: "o usuário clicou", "a tela abriu", "o texto mudou".
Você escreve um **método** e diz ao controle "quando este evento acontecer,
chame este método". Essa ligação é feita com `+=`:

```csharp
this.btnCadastrar.Click += new System.EventHandler(this.btnCadastrar_Click);
//   ^controle      ^evento                              ^método que será chamado
```

Todo método de evento tem sempre os mesmos dois parâmetros:

```csharp
private void btnCadastrar_Click(object sender, EventArgs e)
```

- `sender` = **quem** disparou o evento (neste caso, o próprio botão).
- `e` = informações **extras** sobre o evento (numa tecla, qual tecla foi).

No começo você não vai usar nenhum dos dois — mas eles têm que estar ali,
senão a assinatura do método não bate com o que o evento espera.

---

## Etapa 5 — Upload e prévia da imagem

**O que fazer:** no clique de `btnEscolherImagem`, abrir a janela de escolha de
arquivo do Windows e mostrar a imagem escolhida em `picPrevia`.

### O código

```csharp
using (OpenFileDialog dialogo = new OpenFileDialog())
{
    dialogo.Title = "Escolha a imagem da fruta";
    dialogo.Filter = "Imagens (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";

    if (dialogo.ShowDialog() != DialogResult.OK)
    {
        return;
    }

    // ... carrega a imagem ...
    caminhoImagem = dialogo.FileName;
}
```

### Explicando pedaço por pedaço

**`OpenFileDialog`** é a janela padrão do Windows para escolher arquivo. Você
não precisa desenhá-la: ela já existe.

**`Filter`** monta a caixinha "Tipo de arquivo". O formato é estranho mas é
simples: `Texto que aparece|máscara;máscara`. O texto antes do `|` é o que o
usuário lê; depois do `|` vêm as extensões aceitas.

**`if (dialogo.ShowDialog() != DialogResult.OK) return;`** — `ShowDialog()`
devolve `OK` se o usuário escolheu um arquivo e `Cancel` se desistiu. Sem este
`if`, clicar em "Cancelar" apagaria a imagem que já estava escolhida.

**`using (...)`** garante que o objeto é liberado da memória ao final do bloco,
mesmo se der erro no meio. Leia como: "use isto aqui dentro e depois jogue fora".

### Por que guardamos o *caminho* e não a imagem

O campo `CaminhoImagem` guarda só um texto, como `C:\Fotos\banana.png`.
A imagem em si continua no disco. É mais leve, e é isso que a API vai precisar
para ler o arquivo na hora de enviar.

O campo fica declarado **fora** dos métodos:

```csharp
private string caminhoImagem = string.Empty;
```

Isso é importante: uma variável declarada **dentro** de um método morre quando o
método termina. Precisamos que o caminho **sobreviva** entre o clique em
"Escolher imagem" e o clique em "Cadastrar" — então ela tem que ficar fora.

### O detalhe do arquivo travado

O caminho óbvio seria:

```csharp
picPrevia.Image = Image.FromFile(dialogo.FileName);   // NÃO faça isso
```

O problema: `Image.FromFile` deixa o arquivo **travado** enquanto o programa
está aberto. O usuário não consegue apagar nem renomear a foto. Fazemos assim:

```csharp
using (MemoryStream memoria = new MemoryStream(File.ReadAllBytes(dialogo.FileName)))
using (Image original = Image.FromStream(memoria))
{
    picPrevia.Image = new Bitmap(original);   // uma CÓPIA em memória
}
```

`new Bitmap(original)` faz uma **cópia** independente. O arquivo no disco é
liberado na hora.

### Duas boas práticas no mesmo código

**Liberar a imagem anterior.** Cada imagem ocupa memória. Se o usuário trocar
de foto 50 vezes, o programa acumula 50 imagens:

```csharp
if (picPrevia.Image != null)
{
    picPrevia.Image.Dispose();
    picPrevia.Image = null;
}
```

**`SizeMode = Zoom`.** Sem isso, a foto aparece cortada ou esticada. `Zoom`
encaixa a imagem inteira na moldura mantendo a proporção.

### `try` / `catch`

E se o arquivo estiver corrompido, ou for um `.png` que na verdade não é imagem?
O programa fecharia com um erro feio. `try`/`catch` evita isso:

```csharp
try
{
    // código que PODE dar erro
}
catch (Exception erro)
{
    MessageBox.Show("Não foi possível abrir esta imagem.\n\n" + erro.Message);
}
```

Leia como: "**tente** fazer isto; se der erro, **pegue** o erro e avise o
usuário em vez de quebrar".

---

## Etapa 6 — Validar antes de cadastrar

Esta é a etapa mais importante do projeto. **Nunca confie no que o usuário
digitou.**

### O padrão que repetimos

Para cada campo, sempre a mesma estrutura:

```csharp
if (/* está errado */)
{
    Avisar("mensagem explicando o problema", campoComProblema);
    return;
}
```

O **`return`** é a parte essencial: ele **sai do método na hora**. Sem ele, o
código continuaria e tentaria cadastrar uma fruta com dados inválidos.

### Texto vazio

```csharp
if (string.IsNullOrWhiteSpace(txtNome.Text))
```

`IsNullOrWhiteSpace` é `true` quando o texto está vazio, é `null`, **ou contém
só espaços**. Testar apenas `txtNome.Text == ""` deixaria passar um nome com
três espaços.

### Números: `TryParse`

O usuário digita texto; você precisa de número. A conversão pode falhar.

```csharp
int quantidade;
if (!int.TryParse(txtQuantidade.Text, out quantidade))
{
    Avisar("A quantidade deve ser um número inteiro. Exemplo: 120", txtQuantidade);
    return;
}
```

**Por que `TryParse` e não `Convert.ToInt32`?**

| | Se o usuário digitar `abc` |
|---|---|
| `Convert.ToInt32("abc")` | **quebra o programa** com uma exceção |
| `int.TryParse("abc", out n)` | devolve `false` e você trata com educação |

`TryParse` faz duas coisas ao mesmo tempo: devolve `true`/`false` dizendo se
conseguiu, e coloca o número convertido na variável marcada com `out`. A
palavra `out` significa "esta variável vai ser **preenchida** pelo método".

Repare que validamos **duas** coisas por campo numérico — se é número, e depois
se faz sentido:

```csharp
if (quantidade <= 0)
{
    Avisar("A quantidade deve ser maior que zero.", txtQuantidade);
    return;
}
```

`-5` é um número válido, mas não é uma quantidade válida.

### Lista suspensa

```csharp
if (cmbTipo.SelectedIndex < 0)
```

`SelectedIndex` é a posição escolhida, começando em zero. Vale **`-1`** quando
nada foi selecionado.

### Não repetir código: o método `Avisar`

O `MessageBox` de aviso apareceria **sete vezes** igual. Em vez disso,
escrevemos o trecho uma vez:

```csharp
private void Avisar(string mensagem, Control controle)
{
    MessageBox.Show(mensagem, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    controle.Focus();
}
```

Repare no tipo do parâmetro: **`Control`**. `Control` é o "avô" de todos os
controles — `TextBox`, `Button`, `ComboBox` são todos `Control`. Por isso o
mesmo método serve para qualquer campo.

`controle.Focus()` põe o cursor no campo errado, para o usuário não ter que
procurar qual era.

### Montando o objeto

Passou por toda a validação? Agora sim:

```csharp
Fruta fruta = new Fruta
{
    Nome = txtNome.Text.Trim(),
    Quantidade = quantidade,
    Tipo = cmbTipo.SelectedItem.ToString(),
    Valor = valor,
    Validade = dtpValidade.Value,
    CaminhoImagem = caminhoImagem
};
```

Essa sintaxe com `{ }` é o **inicializador de objeto**: é um atalho para criar o
objeto e preencher as propriedades em seguida. `Trim()` remove espaços sobrando
no começo e no fim.

---

## Etapa 7 — Navegar entre as telas

Depois de cadastrar, o sistema deve ir para o Estoque.

```csharp
private void AbrirEstoque()
{
    using (EstoqueForm estoque = new EstoqueForm())
    {
        this.Hide();
        estoque.ShowDialog();
    }

    this.Show();
}
```

### `Show()` × `ShowDialog()`

Esta é a diferença que mais confunde iniciantes:

| | `Show()` | `ShowDialog()` |
|---|---|---|
| O código continua correndo na hora? | **Sim** | **Não**, espera a tela fechar |
| Dá para usar a tela de trás? | Sim | Não, fica bloqueada |

No nosso caso queremos `ShowDialog()`: o código **para** naquela linha e só
continua quando o usuário fechar o Estoque. Se usássemos `Show()`, a linha
`this.Show()` rodaria imediatamente e o Cadastro reapareceria na frente do
Estoque.

### `this`

`this` significa "**este** objeto, a tela em que estou agora". Dentro do
`CadastroForm`, `this.Hide()` esconde o Cadastro.

### Por que esconder e mostrar

`this.Hide()` esconde o Cadastro para as duas janelas não ficarem empilhadas.
Quando o Estoque fecha, `this.Show()` traz o Cadastro de volta.

E no Estoque, o botão Voltar é só isto:

```csharp
private void btnVoltar_Click(object sender, EventArgs e)
{
    this.Close();
}
```

Ele fecha o Estoque; o Cadastro reaparece sozinho, porque a execução volta para
a linha seguinte ao `ShowDialog()`.

> ⚠️ Um erro comum é usar `this.Hide()` no lugar de `this.Close()` no botão
> Voltar. A tela desaparece, mas **continua na memória** — e o programa nunca
> termina de verdade, ficando como processo fantasma no Gerenciador de Tarefas.

---

## Etapa 8 — Montar a tela de Estoque

```
 EstoqueForm  (800 x 450)
 +-------------------------------------------------------------+
 | Estoque de Frutas                                           |
 |  +-- Pesquisar ------------------------+                     |
 |  | Categoria        Nome               |    [ Pesquisar ]    |
 |  | [v__________]    [___________]      |                     |
 |  +-------------------------------------+                     |
 |  +-------------------------------------------------------+   |
 |  | Fruta        | Quantidade | Validade   | Valor | Tipo |   |
 |  |--------------|------------|------------|-------|------|   |
 |  | Banana Prata | 120        | 07/10/2026 | R$7,49| Trop.|   |
 |  +-------------------------------------------------------+   |
 |                                            [ Voltar ]        |
 +-------------------------------------------------------------+
```

### O `ListView` só funciona com duas propriedades

Este é o erro número 1 de quem usa `ListView` pela primeira vez: **arrastar o
controle e não ver tabela nenhuma**. Faltam duas coisas:

```csharp
lstEstoque.View = View.Details;   // sem isto, não vira tabela
// e as colunas precisam existir
```

Por padrão o `ListView` nasce em modo `LargeIcon` (ícones grandes), não em modo
tabela. `View.Details` é o que faz as colunas aparecerem.

As outras duas propriedades são conforto:

```csharp
lstEstoque.FullRowSelect = true;   // clicar seleciona a linha toda, não só a 1ª célula
lstEstoque.GridLines = true;       // desenha as linhas de grade
```

### As colunas

No Designer, cada coluna é um `ColumnHeader`:

| Coluna | Largura |
|---|---|
| Fruta | 220 |
| Quantidade | 110 |
| Validade | 120 |
| Valor | 110 |
| Tipo | 180 |

### Preenchendo as linhas

```csharp
private void PreencherLista(List<Fruta> frutas)
{
    lstEstoque.Items.Clear();

    if (frutas == null || frutas.Count == 0)
    {
        MessageBox.Show("Nenhuma fruta encontrada com estes filtros.");
        return;
    }

    foreach (Fruta fruta in frutas)
    {
        ListViewItem linha = new ListViewItem(fruta.Nome);   // 1ª coluna
        linha.SubItems.Add(fruta.Quantidade.ToString());     // 2ª coluna
        linha.SubItems.Add(fruta.Validade.ToShortDateString()); // 3ª
        linha.SubItems.Add(fruta.Valor.ToString("C"));       // 4ª
        linha.SubItems.Add(fruta.Tipo);                      // 5ª
        lstEstoque.Items.Add(linha);
    }
}
```

**Três detalhes que costumam dar errado:**

1. **`Items.Clear()` no começo.** Sem isso, o resultado da segunda pesquisa fica
   grudado embaixo do resultado da primeira.
2. **A primeira coluna é o texto do `ListViewItem`**; da segunda em diante são
   `SubItems`. E a ordem dos `SubItems` tem que ser **exatamente** a ordem das
   colunas na tela.
3. **`ToString()` em tudo.** O `ListView` só mostra texto. `ToShortDateString()`
   mostra `07/10/2026` em vez da data com hora; `ToString("C")` é o formato de
   moeda e mostra `R$ 7,49`.

### `foreach`

```csharp
foreach (Fruta fruta in frutas)
```

Leia como: "para **cada** `Fruta` (que vou chamar de `fruta`) **dentro** da
lista `frutas`, faça...". É a forma mais simples de percorrer uma lista.

### Por que `PreencherLista` é um método separado

Ele não sabe de onde as frutas vieram — só sabe desenhá-las. Isso se chama
**separar responsabilidades**. Quando a API entrar no lugar da lista vazia,
este método **não muda nada**.

---

## Etapa 9 — Os filtros Categoria e Nome

No `Load` da tela:

```csharp
cmbCategoria.Items.Add(TodasAsCategorias);             // "(Todas)"
cmbCategoria.Items.AddRange(FrutaApi.ObterTipos());    // os mesmos tipos do Cadastro
cmbCategoria.SelectedIndex = 0;                        // começa em "(Todas)"
```

Repare de novo: **`FrutaApi.ObterTipos()`**, o mesmo método da tela de Cadastro.
É isso que garante que o que foi cadastrado como `"Ácida"` seja encontrado ao
pesquisar `"Ácida"`.

### A opção "(Todas)"

`"(Todas)"` não é uma categoria de verdade — é a opção "não quero filtrar por
este campo". Então, antes de pesquisar, traduzimos para texto vazio:

```csharp
string categoria = cmbCategoria.SelectedItem.ToString();
if (categoria == TodasAsCategorias)
{
    categoria = string.Empty;   // para a API, vazio = não filtre
}
```

O texto `"(Todas)"` está numa **constante**:

```csharp
private const string TodasAsCategorias = "(Todas)";
```

`const` = valor fixo que nunca muda. Usamos constante porque o texto aparece em
dois lugares (ao preencher a lista e ao comparar). Se estivesse escrito na mão
duas vezes, um erro de digitação em um deles criaria um bug difícil de achar —
e o compilador não avisaria nada.

---

## Etapa 10 — Onde a API entra

Neste projeto, **os dados não ficam no programa**: quem guarda tudo é uma API
(um servidor que responde pela internet). São duas partes:

- A API de **categorias** já existe. Ela é outro projeto, que roda separado
  deste, e a chamada a ela **está ligada de verdade**.
- A API de **frutas** (cadastrar e pesquisar) **ainda não existe**, então essas
  duas chamadas estão **comentadas**.

Tudo isso fica no arquivo `FrutaApi.cs`.

### Por que existe um arquivo só para isso

As telas nunca falam HTTP direto. Elas chamam:

```csharp
FrutaApi.ObterTipos();
FrutaApi.Cadastrar(fruta);
FrutaApi.Pesquisar(categoria, nome);
```

Se amanhã o endereço ou o formato da API mudar, **só o `FrutaApi.cs` muda**. As
telas continuam iguais. Isso se chama **centralizar a dependência**.

### A chamada que já funciona: categorias

```
GET https://localhost:7069/Categoria

Resposta esperada: 200 OK
[
  { "id": 1, "descricao": "Ácida" },
  ...
]
```

O endereço fica na constante `FrutaApi.UrlCategorias`. Para o programa entender
esse JSON existe a classe `Categoria`:

```csharp
[DataContract]
public class Categoria
{
    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "descricao")]
    public string Descricao { get; set; }
}
```

- `[DataContract]` avisa o leitor de JSON de que a classe pode ser montada a
  partir de um JSON.
- `[DataMember(Name = "descricao")]` diz **qual campo do JSON** vai em cada
  propriedade. É necessário porque no JSON o nome vem em minúsculo e em C# o
  costume é começar com maiúscula.

Quem faz a chamada é `FrutaApi.ListarCategorias()`:

```csharp
try
{
    using (HttpClient cliente = new HttpClient())
    {
        cliente.Timeout = TimeSpan.FromSeconds(5);

        HttpResponseMessage resposta = cliente.GetAsync(UrlCategorias).Result;

        if (!resposta.IsSuccessStatusCode)
        {
            return new List<Categoria>();
        }

        using (Stream corpo = resposta.Content.ReadAsStreamAsync().Result)
        {
            DataContractJsonSerializer leitor =
                new DataContractJsonSerializer(typeof(List<Categoria>));

            List<Categoria> categorias = (List<Categoria>)leitor.ReadObject(corpo);
            // ...
            return categorias;
        }
    }
}
catch (Exception)
{
    return new List<Categoria>();
}
```

Pedaço por pedaço:

- `HttpClient` é quem faz o pedido pela rede. `GetAsync` faz um `GET`.
- `Timeout` de 5 segundos: sem isso ele esperaria até 100 segundos, com a tela
  congelada.
- `IsSuccessStatusCode` é `true` quando a API respondeu com um código de sucesso
  (como `200 OK`).
- `DataContractJsonSerializer` transforma o JSON em uma `List<Categoria>`. Ele
  já vem no .NET — só exige a referência `System.Runtime.Serialization`, que já
  está no projeto.
- O `try`/`catch` é **obrigatório**: se a API estiver desligada, o `HttpClient`
  lança uma exceção. Sem o `catch`, o programa fecharia na cara do usuário.

As telas não chamam `ListarCategorias()` direto. Elas chamam
`FrutaApi.ObterTipos()`, que faz três coisas:

1. Pega só o texto (`Descricao`) de cada categoria, porque é isso que a lista
   suspensa mostra.
2. Se não veio nenhuma categoria (API fora do ar), devolve a **lista reserva**
   `Fruta.Tipos`. O programa continua funcionando.
3. **Guarda o resultado** numa variável e, da segunda vez em diante, devolve o
   que guardou. Assim o Cadastro e o Estoque mostram sempre a mesma lista. Para
   buscar as categorias de novo, feche e abra o programa.

> **Se as categorias da API não aparecerem**, confira: (1) o projeto da API
> está rodando? (2) abra `https://localhost:7069/Categoria` no navegador — se
> ele reclamar do certificado, rode `dotnet dev-certs https --trust` no
> projeto da API. Enquanto o Windows não confia no certificado, a chamada falha
> e o programa usa a lista reserva.

### As duas chamadas esperadas (ainda sem API)

**Cadastrar uma fruta:**

```
POST https://localhost:5001/api/frutas
Content-Type: multipart/form-data

nome       = "Banana Prata"
quantidade = 120
tipo       = "Tropical"
valor      = 7.49
validade   = "2026-12-31"          (formato yyyy-MM-dd)
imagem     = <bytes do arquivo>

Resposta esperada: 201 Created
```

Usamos `multipart/form-data` (e não JSON) porque estamos enviando um **arquivo**
junto com os campos de texto.

**Pesquisar o estoque:**

```
GET https://localhost:5001/api/frutas?categoria=Tropical&nome=banana

Resposta esperada: 200 OK
[
  {
    "nome": "Banana Prata",
    "quantidade": 120,
    "tipo": "Tropical",
    "valor": 7.49,
    "validade": "2026-12-31",
    "caminhoImagem": "banana.png"
  }
]
```

Os dois parâmetros são opcionais: enviar vazio significa "não filtre por este
campo".

### Enquanto a API de frutas não existe

`Cadastrar` e `Pesquisar` devolvem um valor "vazio", propositalmente:

```csharp
public static bool Cadastrar(Fruta fruta)
{
    // ...código comentado...
    return false;      // nada foi gravado de verdade
}

public static List<Fruta> Pesquisar(string categoria, string nome)
{
    // ...código comentado...
    return new List<Fruta>();     // lista VAZIA
}
```

> **Uma lista vazia não é a mesma coisa que `null`.** Uma lista vazia é uma
> lista que existe e tem zero itens: o `foreach` passa por ela sem fazer nada.
> `null` é "não existe lista nenhuma" — e tentar percorrer `null` quebra o
> programa. Por isso devolvemos lista vazia, e nunca `null`.

**Consequência prática:** ao clicar em Pesquisar, você verá "Nenhuma fruta
encontrada". Isso está **certo** — não é bug.

Para ver o desenho da tabela funcionando, abra `EstoqueForm.cs`, comente a linha
`List<Fruta> encontradas = FrutaApi.Pesquisar(categoria, nome);` e descomente o
bloco de frutas de exemplo logo abaixo dela. Depois desfaça.

### Como ativar a API de frutas de verdade

1. **Trocar o endereço.** Em `FrutaApi.cs`, ajuste `BaseUrl`.
2. **Descomentar** o corpo de `Cadastrar` e de `Pesquisar`.
3. **Ler o JSON das frutas.** O código comentado de `Pesquisar` usa
   `JsonConvert`, que o projeto não tem. Escolha um caminho:
   - **`DataContractJsonSerializer`** — é o que `ListarCategorias()` já usa,
     então não precisa instalar nada. Coloque `[DataContract]` e `[DataMember]`
     na classe `Fruta`, do mesmo jeito que foi feito em `Categoria`, e troque a
     linha do `JsonConvert` pelo mesmo código de leitura.
   - **`Newtonsoft.Json`** — instale pelo NuGet (*Ferramentas → Gerenciador de
     Pacotes NuGet*). É a mais usada e a mais curta de escrever:
     `JsonConvert.DeserializeObject<List<Fruta>>(json)`.

   `System.Net.Http` (o `HttpClient`) **já está referenciado** no projeto.
4. **Tratar o retorno.** Em `CadastroForm.cs`, troque a linha
   `FrutaApi.Cadastrar(fruta);` pelo bloco `if` que já está comentado logo
   abaixo dela, para avisar o usuário quando a API recusar o cadastro.
5. **Estudar `async`/`await`.** O código (o comentado e o das categorias) usa
   `.Result`, que é o jeito mais simples de esperar uma resposta — mas ele
   **congela a tela** enquanto a API não responde. O jeito correto em programas reais é `async`/`await`. Fica
   como assunto para a próxima etapa do curso.

---

## Etapa 11 — Compilar e executar

No Visual Studio: abra `WindowsFormsAppFruteira.sln` e pressione **F5**.

Pela linha de comando do Windows:

```cmd
msbuild WindowsFormsAppFruteira.sln /p:Configuration=Debug
bin\Debug\WindowsFormsAppFruteira.exe
```

> ⚠️ **`dotnet build` NÃO funciona neste projeto.** Ele é .NET **Framework**
> 4.7.2, que roda só no Windows e se compila com **MSBuild**. `dotnet` é para
> projetos .NET Core / .NET 5 ou superior.

### O erro que mais vai acontecer com você

Este projeto usa o formato **antigo** de arquivo `.csproj`, no qual **todo
arquivo é listado um por um**. Se você criar uma classe nova e ela não estiver
listada, o compilador age como se ela não existisse — o erro aparece como
`O nome 'Fruta' não existe no contexto atual`, que não dá nenhuma pista do
problema real.

Ao criar a classe pelo Visual Studio (*Projeto → Adicionar Classe*), ele
registra sozinho. Se criar o arquivo na mão, abra o `.csproj` e adicione:

```xml
<Compile Include="Fruta.cs" />
```

### Roteiro de teste

Confira um por um:

1. A tela abre com a lista de tipos preenchida e a prévia vazia. Com a API de
   categorias rodando, aparecem as categorias dela; com a API desligada,
   aparecem os 5 tipos da lista reserva.
2. Clicar em **Cadastrar** com tudo em branco → aviso, e **não** troca de tela.
3. Digitar `abc` em Quantidade → aviso específico daquele campo.
4. Digitar `abc` em Valor → aviso específico daquele campo.
5. **Escolher imagem** → a foto aparece inteira, sem esticar.
6. Preencher tudo certo → mensagem de sucesso → o Estoque abre.
7. No Estoque, as 5 colunas aparecem na ordem Fruta, Quantidade, Validade,
   Valor, Tipo.
8. **Pesquisar** → "Nenhuma fruta encontrada" (correto, sem API).
9. **Voltar** → o Cadastro reaparece. Feche o Cadastro e confirme no
   Gerenciador de Tarefas que o processo terminou.

---

## Etapa 12 — Exercícios

Em ordem crescente de dificuldade.

1. **Contador de itens.** Mostre num `Label` do Estoque quantas frutas a
   pesquisa trouxe. *Dica: `lstEstoque.Items.Count`.*
2. **Limpar filtros.** Adicione um botão que volta a Categoria para "(Todas)",
   apaga o Nome e limpa a lista.
3. **Destacar vencidas.** Pinte de vermelho as linhas cuja validade já passou.
   *Dica: `linha.ForeColor = Color.Red;` e compare `fruta.Validade` com
   `DateTime.Today`.*
4. **Enter pesquisa.** Fazer a tecla Enter no campo Nome disparar a pesquisa.
   *Dica: propriedade `AcceptButton` do formulário.*
5. **Validade mínima maior.** Não aceitar fruta que vence em menos de 3 dias.
   *Dica: `DateTime.Today.AddDays(3)`.*
6. **Ordenar pelo cabeçalho.** Clicar no cabeçalho de uma coluna ordena a lista
   por ela. *Dica: evento `ColumnClick` e a propriedade `ListViewItemSorter`.*
7. **Excluir.** Botão que remove a fruta selecionada, com pergunta de
   confirmação. *Dica: `MessageBox.Show(..., MessageBoxButtons.YesNo)` e uma
   nova chamada `DELETE /api/frutas/{id}` em `FrutaApi.cs`. Note que para isso
   a classe `Fruta` vai precisar de uma propriedade `Id`.*
8. **Tela de detalhes.** Duplo clique numa linha abre uma terceira tela
   mostrando a fruta com a imagem grande.
