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

Sem uma classe, para guardar uma fruta você precisaria de várias variáveis:

```csharp
int id;
string nome;
decimal preco;
int quantidade;
int idCategoria;
DateTime dataValidade;
string hashImg;
```

E para passar essa fruta para outro método, teria que passar sete parâmetros.
Com duas frutas, catorze variáveis. Isso não escala.

Uma **classe** é um molde que agrupa informações que andam juntas:

```csharp
public class Fruta
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public int Quantidade { get; set; }
    public int IdCategoria { get; set; }
    public DateTime DataValidade { get; set; }
    public string HashImg { get; set; }
}
```

As propriedades não foram inventadas por nós: elas **espelham a fruta que a API
guarda**, cujos campos são `id`, `nome`, `preco`, `quantidade`, `id_categoria`,
`data_validade` e `hash_img`. No arquivo de verdade cada propriedade ainda tem
uma etiqueta `[DataMember(Name = "...")]` ligando-a ao campo da API — isso é
explicado na etapa 10.

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
| **propriedade** | Uma informação guardada no objeto (`Nome`, `Preco`...). |
| **`{ get; set; }`** | Atalho que diz "esta propriedade pode ser lida (`get`) e escrita (`set`)". |
| **`public`** | Qualquer parte do programa pode usar. |

### A escolha de cada tipo de dado

Isto **não** é detalhe:

- `Quantidade` é `int` (número inteiro) porque não existe "meia caixa".
- `Preco` é `decimal`, **não** `double`. `double` faz arredondamentos estranhos
  com dinheiro (`0.1 + 0.2` pode dar `0.30000000000000004`). Para dinheiro,
  sempre `decimal`.
- `DataValidade` é `DateTime`, **não** `string`. Sendo `DateTime` você pode
  comparar datas (`if (fruta.DataValidade < DateTime.Today)`). Como texto, não
  conseguiria.
- `IdCategoria` é `int`: a fruta guarda o **número** da categoria, e não o texto
  `"Tropical"`. Se amanhã a categoria mudar de nome, as frutas continuam
  apontando para a categoria certa. Na tela mostramos o texto, e quem troca um
  pelo outro é a `FrutaApi` (etapas 6 e 8).
- `Id` é o número da própria fruta. Quem cria esse número é a API, na hora do
  cadastro; uma fruta que ainda não foi enviada tem `Id` igual a `0`.
- `HashImg` é `string`: é a imagem da fruta escrita como texto (Base64) — veja a
  etapa 5.

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
suspensas não ficarem vazias. Ela tem só os textos, sem o número de cada
categoria — por isso com ela dá para ver as telas, mas não para cadastrar.

As telas não leem `Fruta.Tipos` direto. As duas chamam o mesmo método:

```csharp
FrutaApi.ObterTipos()
```

Esse método é a **fonte única da verdade**. A tela de Cadastro o usa para
preencher o campo Tipo, e a tela de Estoque o usa para preencher o filtro
Categoria. Assim as duas telas mostram sempre as mesmas categorias, escritas do
mesmo jeito.

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
O Tipo é a **categoria** da fruta — a mesma que o filtro "Categoria" do Estoque
pesquisa — e a API só aceita categorias que existem. Se fosse texto livre, o
usuário poderia digitar uma categoria que não existe. Com a lista fechada, só
existem os valores que `FrutaApi.ObterTipos()` devolve.

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

A tela guarda só um texto, como `C:\Fotos\banana.png`. A imagem em si continua
no disco. É mais leve, e é desse caminho que vamos precisar para ler o arquivo
na hora de enviar a fruta para a API.

O campo fica declarado **fora** dos métodos:

```csharp
private string caminhoImagem = string.Empty;
```

Isso é importante: uma variável declarada **dentro** de um método morre quando o
método termina. Precisamos que o caminho **sobreviva** entre o clique em
"Escolher imagem" e o clique em "Cadastrar" — então ela tem que ficar fora.

### Como a imagem chega na API: Base64

A fruta é enviada para a API como um texto JSON (etapa 10), e JSON só carrega
**texto**. Uma imagem não é texto: é uma sequência de bytes. **Base64** é uma
forma de escrever esses bytes usando só letras e números. No clique em
Cadastrar fazemos:

```csharp
string imagemBase64 = Convert.ToBase64String(File.ReadAllBytes(caminhoImagem));
```

- `File.ReadAllBytes` lê o arquivo inteiro do disco.
- `Convert.ToBase64String` transforma os bytes em texto.

Esse texto é o que vai na propriedade `HashImg` da fruta. No código de verdade
essa linha fica dentro de um `try`/`catch`, porque o arquivo pode ter sido
apagado ou movido depois de escolhido.

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

Escolher não basta: a API guarda o **número** da categoria, e a lista suspensa
só tem o texto. Então trocamos um pelo outro:

```csharp
int idCategoria = FrutaApi.ObterIdCategoria(cmbTipo.SelectedItem.ToString());
if (idCategoria == 0)
{
    Avisar("Não foi possível obter as categorias da API. ...", cmbTipo);
    return;
}
```

`ObterIdCategoria` devolve `0` quando o texto não é de nenhuma categoria da API.
Isso acontece quando a API estava desligada e a lista suspensa foi preenchida
com a lista reserva, que não tem números. Sem o número não há o que enviar,
então avisamos e saímos.

### Não repetir código: o método `Avisar`

O `MessageBox` de aviso apareceria **nove vezes** igual. Em vez disso,
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
    Preco = valor,
    Quantidade = quantidade,
    IdCategoria = idCategoria,
    DataValidade = dtpValidade.Value.Date,
    HashImg = imagemBase64
};
```

Essa sintaxe com `{ }` é o **inicializador de objeto**: é um atalho para criar o
objeto e preencher as propriedades em seguida. `Trim()` remove espaços sobrando
no começo e no fim. `.Date` joga fora a hora e fica só com o dia. O
`imagemBase64` é a imagem transformada em texto (etapa 5). O `Id` não é
preenchido: quem cria esse número é a API.

### Enviando e conferindo a resposta

```csharp
if (!FrutaApi.Cadastrar(fruta))
{
    MessageBox.Show("Não foi possível cadastrar a fruta. Tente novamente.",
        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
    return;
}
```

`Cadastrar` devolve `true` quando a API confirmou e `false` quando ela recusou
ou não respondeu. O `!` significa "não": **se não cadastrou**, avisamos e saímos
com `return` — sem limpar os campos, para o usuário não perder o que digitou e
poder tentar de novo. Só depois desse `if` vem a mensagem de sucesso e a troca
de tela.

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
        linha.SubItems.Add(fruta.DataValidade.ToShortDateString()); // 3ª
        linha.SubItems.Add(fruta.Preco.ToString("C"));       // 4ª
        linha.SubItems.Add(FrutaApi.ObterDescricaoCategoria(fruta.IdCategoria)); // 5ª
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

A quinta coluna merece atenção: a fruta guarda só o **número** da categoria
(`IdCategoria`), e mostrar `2` na tela não diria nada ao usuário.
`FrutaApi.ObterDescricaoCategoria` troca o número pelo texto (`"Tropical"`). É o
caminho contrário do `ObterIdCategoria` usado no Cadastro.

### `foreach`

```csharp
foreach (Fruta fruta in frutas)
```

Leia como: "para **cada** `Fruta` (que vou chamar de `fruta`) **dentro** da
lista `frutas`, faça...". É a forma mais simples de percorrer uma lista.

### Por que `PreencherLista` é um método separado

Ele não sabe de onde as frutas vieram — só sabe desenhá-las. Isso se chama
**separar responsabilidades**. Se amanhã as frutas vierem de outro lugar, este
método **não muda nada**.

---

## Etapa 9 — Os filtros Categoria e Nome

No `Load` da tela:

```csharp
cmbCategoria.Items.Add(TodasAsCategorias);             // "(Todas)"
cmbCategoria.Items.AddRange(FrutaApi.ObterTipos());    // os mesmos tipos do Cadastro
cmbCategoria.SelectedIndex = 0;                        // começa em "(Todas)"
```

Repare de novo: **`FrutaApi.ObterTipos()`**, o mesmo método da tela de Cadastro.
É isso que garante que a categoria escolhida no cadastro seja a mesma que
aparece aqui para pesquisar.

### A opção "(Todas)"

A API de frutas filtra pelo **número** da categoria, e não pelo texto. E
`"(Todas)"` não é uma categoria de verdade — é a opção "não quero filtrar por
este campo". Combinamos então que o número **zero** significa "sem filtro":

```csharp
int idCategoria = 0;                       // 0 = não filtre por categoria

string categoria = cmbCategoria.SelectedItem.ToString();
if (categoria != TodasAsCategorias)
{
    idCategoria = FrutaApi.ObterIdCategoria(categoria);   // texto -> número
}

string nome = txtNome.Text.Trim();

List<Fruta> encontradas = FrutaApi.Pesquisar(idCategoria, nome);
```

### Pesquisar sem filtro nenhum

Com `"(Todas)"` e o Nome em branco não há filtro nenhum — e a pesquisa
**acontece do mesmo jeito**, trazendo **todas** as frutas. Não existe um `if`
impedindo a pesquisa "vazia": quem decide o que mandar para a API é o
`FrutaApi.Pesquisar` (etapa 10), que só coloca no endereço os filtros que foram
preenchidos.

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
(um servidor que responde pela rede). Ela é outro projeto, que roda separado
deste, e tem duas partes:

- `/Categoria` — a lista de categorias de fruta.
- `/Fruta` — pesquisar e cadastrar frutas.

Tudo isso fica no arquivo `FrutaApi.cs`.

### Por que existe um arquivo só para isso

As telas nunca falam HTTP direto. Elas chamam:

```csharp
FrutaApi.ObterTipos();
FrutaApi.Cadastrar(fruta);
FrutaApi.Pesquisar(idCategoria, nome);
```

Se amanhã o endereço ou o formato da API mudar, **só o `FrutaApi.cs` muda**. As
telas continuam iguais. Isso se chama **centralizar a dependência**.

### A primeira chamada: categorias

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
   suspensa mostra. A categoria completa (com o `Id`) também fica guardada: é
   com ela que `ObterIdCategoria` e `ObterDescricaoCategoria` trocam texto por
   número e número por texto.
2. Se não veio nenhuma categoria (API fora do ar), devolve a **lista reserva**
   `Fruta.Tipos`. O programa continua funcionando.
3. **Guarda o resultado** numa variável e, da segunda vez em diante, devolve o
   que guardou. Assim o Cadastro e o Estoque mostram sempre a mesma lista. Para
   buscar as categorias de novo, feche e abra o programa.

> **Se as categorias da API não aparecerem**, confira: (1) o projeto da API
> está rodando? (2) abra `https://localhost:7069/Categoria` no navegador — se
> ele reclamar do certificado, rode `dotnet dev-certs https --trust` no
> projeto da API. Enquanto o Windows não confia no certificado, a chamada falha
> e o programa usa a lista reserva. Vale o mesmo para as frutas: sem a API, a
> pesquisa volta vazia e o cadastro avisa que não conseguiu gravar.

### Pesquisar o estoque

```
GET https://localhost:7069/Fruta                             (todas as frutas)
GET https://localhost:7069/Fruta?id_categoria=2
GET https://localhost:7069/Fruta?nome=banana
GET https://localhost:7069/Fruta?id_categoria=2&nome=banana

Resposta esperada: 200 OK
[
  {
    "id": 1,
    "nome": "Banana Prata",
    "preco": 7.49,
    "quantidade": 120,
    "id_categoria": 2,
    "data_validade": "2026-12-31T00:00:00",
    "hash_img": "iVBORw0KGgo..."
  }
]
```

O endereço fica na constante `FrutaApi.UrlFrutas`. Os filtros vão depois de um
`?`, separados por `&`, e os dois são **opcionais**: só entra no endereço o
filtro que o usuário preencheu. Sem filtro nenhum o endereço fica só
`.../Fruta`, e a API devolve tudo.

```csharp
public static List<Fruta> Pesquisar(int idCategoria, string nome)
{
    List<string> filtros = new List<string>();

    if (idCategoria > 0)
    {
        filtros.Add("id_categoria=" + idCategoria);
    }

    if (!string.IsNullOrWhiteSpace(nome))
    {
        filtros.Add("nome=" + Uri.EscapeDataString(nome));
    }

    string url = UrlFrutas;

    if (filtros.Count > 0)
    {
        url = url + "?" + string.Join("&", filtros);
    }

    // ...daqui em diante é igual a ListarCategorias(): GetAsync(url),
    // IsSuccessStatusCode, DataContractJsonSerializer(typeof(List<Fruta>))
    // e try/catch devolvendo lista vazia.
}
```

- Cada filtro preenchido entra numa lista; `string.Join("&", filtros)` junta os
  itens colocando `&` **entre** eles. Com um filtro só, não sobra `&` nenhum.
- `Uri.EscapeDataString` troca espaços e acentos por código. Sem isso, um nome
  como `banana prata` quebraria o endereço.

> **Uma lista vazia não é a mesma coisa que `null`.** Uma lista vazia é uma
> lista que existe e tem zero itens: o `foreach` passa por ela sem fazer nada.
> `null` é "não existe lista nenhuma" — e tentar percorrer `null` quebra o
> programa. Por isso `Pesquisar` devolve lista vazia, e nunca `null`, tanto
> quando não há fruta nenhuma quanto quando a API não respondeu.

**Consequência prática:** com a API desligada, clicar em Pesquisar mostra
"Nenhuma fruta encontrada". Para ver o desenho da tabela mesmo assim, abra
`EstoqueForm.cs`, comente a linha
`List<Fruta> encontradas = FrutaApi.Pesquisar(idCategoria, nome);` e descomente
o bloco de frutas de exemplo logo abaixo dela. Depois desfaça.

### Como o JSON vira `Fruta`

É o mesmo mecanismo da classe `Categoria`: cada propriedade tem a etiqueta com o
nome do campo no JSON.

```csharp
[DataContract]
public class Fruta
{
    [DataMember(Name = "id", EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(Name = "nome")]
    public string Nome { get; set; }

    [DataMember(Name = "preco")]
    public decimal Preco { get; set; }

    [DataMember(Name = "quantidade")]
    public int Quantidade { get; set; }

    [DataMember(Name = "id_categoria")]
    public int IdCategoria { get; set; }

    public DateTime DataValidade { get; set; }

    [DataMember(Name = "hash_img")]
    public string HashImg { get; set; }
}
```

Aqui o `Name` é ainda mais necessário do que em `Categoria`: no JSON o campo se
chama `id_categoria` (com sublinhado), e em C# o costume é `IdCategoria`.

Dois detalhes:

- **`EmitDefaultValue = false` no `Id`.** Faz o `Id` **não** ser enviado quando
  vale `0`. Uma fruta nova ainda não tem número — quem cria é a API —, então o
  cadastro não manda um `"id": 0`.
- **`DataValidade` não tem etiqueta.** No JSON a data vem como **texto**
  (`"2026-12-31T00:00:00"`), e o leitor de JSON do .NET não sabe transformar
  esse texto em `DateTime` sozinho. Por isso existe uma segunda propriedade,
  `private`, que serve de ponte:

```csharp
[DataMember(Name = "data_validade")]
private string DataValidadeTexto
{
    get
    {
        return DataValidade.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
    }
    set
    {
        DateTime data;
        DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out data);
        DataValidade = data;
    }
}
```

  O `get` roda ao **enviar** (data → texto) e o `set` ao **receber** (texto →
  data). Repare no `TryParse`, o mesmo da etapa 6: se a data vier num formato
  inesperado, ele só devolve `false` em vez de quebrar a leitura da lista
  inteira. É `private` porque as telas não precisam dela — elas usam
  `DataValidade`.

### Cadastrar uma fruta

```
POST https://localhost:7069/Fruta
Content-Type: application/json

{
  "nome": "Banana Prata",
  "preco": 7.49,
  "quantidade": 120,
  "id_categoria": 2,
  "data_validade": "2026-12-31T00:00:00",
  "hash_img": "iVBORw0KGgo..."
}

Resposta esperada: um código de sucesso (200 OK ou 201 Created)
```

O endereço é o mesmo da pesquisa. O que muda é o **verbo**: `GET` pede dados,
`POST` envia dados novos.

```csharp
public static bool Cadastrar(Fruta fruta)
{
    try
    {
        string json;

        using (MemoryStream memoria = new MemoryStream())
        {
            DataContractJsonSerializer escritor =
                new DataContractJsonSerializer(typeof(Fruta));

            escritor.WriteObject(memoria, fruta);

            json = Encoding.UTF8.GetString(memoria.ToArray());
        }

        using (HttpClient cliente = new HttpClient())
        using (StringContent corpo = new StringContent(json, Encoding.UTF8, "application/json"))
        {
            cliente.Timeout = TimeSpan.FromSeconds(5);

            HttpResponseMessage resposta = cliente.PostAsync(UrlFrutas, corpo).Result;

            return resposta.IsSuccessStatusCode;
        }
    }
    catch (Exception)
    {
        return false;
    }
}
```

- `WriteObject` é o caminho inverso do `ReadObject`: transforma o **objeto** em
  **JSON**.
- `StringContent` leva o texto, e o `"application/json"` avisa a API de que
  aquele texto é um JSON.
- `PostAsync` faz o `POST`.
- O método devolve `true` só quando a API confirmou. Com a API desligada o
  `catch` devolve `false`, e a tela de Cadastro avisa o usuário (etapa 6).

### `.Result` e a tela congelada

As três chamadas usam `.Result`, que é o jeito mais simples de esperar uma
resposta — mas ele **congela a tela** enquanto a API não responde. O jeito
correto em programas reais é `async`/`await`. Fica como assunto para a próxima
etapa do curso.

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
   aparecem os 5 tipos da lista reserva. **Os itens 6 e 8 precisam da API
   rodando.**
2. Clicar em **Cadastrar** com tudo em branco → aviso, e **não** troca de tela.
3. Digitar `abc` em Quantidade → aviso específico daquele campo.
4. Digitar `abc` em Valor → aviso específico daquele campo.
5. **Escolher imagem** → a foto aparece inteira, sem esticar.
6. Preencher tudo certo → mensagem de sucesso → o Estoque abre. Com a API
   desligada → aviso de erro, **não** troca de tela e os campos continuam
   preenchidos.
7. No Estoque, as 5 colunas aparecem na ordem Fruta, Quantidade, Validade,
   Valor, Tipo.
8. **Pesquisar** com "(Todas)" e o Nome em branco → aparecem **todas** as
   frutas, incluindo a que você acabou de cadastrar. Escolher uma categoria ou
   digitar parte do nome → só as frutas que combinam. Com a API desligada →
   "Nenhuma fruta encontrada".
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
   *Dica: `linha.ForeColor = Color.Red;` e compare `fruta.DataValidade` com
   `DateTime.Today`.*
4. **Enter pesquisa.** Fazer a tecla Enter no campo Nome disparar a pesquisa.
   *Dica: propriedade `AcceptButton` do formulário.*
5. **Validade mínima maior.** Não aceitar fruta que vence em menos de 3 dias.
   *Dica: `DateTime.Today.AddDays(3)`.*
6. **Ordenar pelo cabeçalho.** Clicar no cabeçalho de uma coluna ordena a lista
   por ela. *Dica: evento `ColumnClick` e a propriedade `ListViewItemSorter`.*
7. **Excluir.** Botão que remove a fruta selecionada, com pergunta de
   confirmação. *Dica: `MessageBox.Show(..., MessageBoxButtons.YesNo)` e uma
   nova chamada `DELETE /Fruta/{id}` em `FrutaApi.cs`. O `{id}` é a propriedade
   `Id` da fruta — guarde-a em `linha.Tag` ao preencher a lista.*
8. **Tela de detalhes.** Duplo clique numa linha abre uma terceira tela
   mostrando a fruta com a imagem grande. *Dica: `Convert.FromBase64String`
   transforma o `HashImg` de volta em bytes.*
