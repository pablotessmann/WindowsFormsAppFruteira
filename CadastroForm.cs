using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsAppFruteira
{
    public partial class CadastroForm : Form
    {
        /// <summary>
        /// Guarda o caminho da imagem escolhida pelo usuário.
        /// Fica aqui, e não dentro de um método, porque precisa "sobreviver"
        /// entre o clique em Escolher imagem e o clique em Cadastrar.
        /// </summary>
        private string caminhoImagem = string.Empty;

        public CadastroForm()
        {
            InitializeComponent();

            // A tela abre maximizada (WindowState, no Designer), mas o usuário
            // pode restaurar e redimensionar a janela. Aqui dizemos que o
            // tamanho desenhado no Designer é o MENOR tamanho permitido: sem
            // isto daria para encolher a janela até os campos ficarem uns por
            // cima dos outros. Neste ponto this.Size ainda é o tamanho do
            // Designer, porque a janela só é maximizada quando aparece na tela.
            this.MinimumSize = this.Size;
        }

        /// <summary>
        /// Executa uma única vez, quando a tela abre.
        /// </summary>
        private void CadastroForm_Load(object sender, EventArgs e)
        {
            // Preenche a lista de tipos com as categorias que vêm da API.
            // Se a API estiver fora do ar, ObterTipos() devolve a lista
            // reserva Fruta.Tipos, então o combo nunca fica vazio.
            cmbTipo.Items.AddRange(FrutaApi.ObterTipos());

            // Não faz sentido cadastrar fruta com validade no passado.
            dtpValidade.MinDate = DateTime.Today;
            dtpValidade.Value = DateTime.Today;

            txtNome.Focus();
        }

        /// <summary>
        /// Abre a janela do Windows para o usuário escolher um arquivo de imagem
        /// e mostra essa imagem na prévia.
        /// </summary>
        private void btnEscolherImagem_Click(object sender, EventArgs e)
        {
            // using garante que o OpenFileDialog é liberado da memória no fim.
            using (OpenFileDialog dialogo = new OpenFileDialog())
            {
                dialogo.Title = "Escolha a imagem da fruta";

                // O Filter monta a caixa "Tipo de arquivo" do diálogo.
                // O formato é: Texto que aparece|máscara;máscara
                dialogo.Filter = "Imagens (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";

                // ShowDialog() devolve OK se o usuário escolheu um arquivo,
                // ou Cancel se ele desistiu. Sem este if, um "Cancelar"
                // apagaria a imagem que já estava escolhida.
                if (dialogo.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    // Se já havia uma imagem na prévia, liberamos a antiga da
                    // memória antes de colocar a nova. Sem isso o programa vai
                    // consumindo memória a cada troca de imagem.
                    if (picPrevia.Image != null)
                    {
                        picPrevia.Image.Dispose();
                        picPrevia.Image = null;
                    }

                    // Lemos o arquivo para a memória e fazemos uma CÓPIA da
                    // imagem (new Bitmap). Assim o arquivo no disco não fica
                    // travado pelo nosso programa enquanto ele está aberto.
                    using (MemoryStream memoria = new MemoryStream(File.ReadAllBytes(dialogo.FileName)))
                    using (Image original = Image.FromStream(memoria))
                    {
                        picPrevia.Image = new Bitmap(original);
                    }

                    // Só guardamos o caminho DEPOIS que a imagem carregou.
                    caminhoImagem = dialogo.FileName;
                }
                catch (Exception erro)
                {
                    // Acontece, por exemplo, se o arquivo estiver corrompido
                    // ou não for realmente uma imagem.
                    MessageBox.Show(
                        "Não foi possível abrir esta imagem.\n\n" + erro.Message,
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Valida os campos, envia a fruta para a API e vai para a tela de Estoque.
        /// </summary>
        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            // ---------- 1) VALIDAÇÃO ----------
            // A regra é sempre a mesma: se está errado, avisa, devolve o foco
            // para o campo problemático e SAI do método com return. O return
            // é o que impede o resto do código de rodar com dado inválido.

            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                Avisar("Informe o nome da fruta.", txtNome);
                return;
            }

            // int.TryParse tenta converter texto em número e devolve true/false.
            // Usamos TryParse (e não Convert.ToInt32) porque TryParse NÃO quebra
            // o programa quando o usuário digita "abc" — ele só devolve false.
            int quantidade;
            if (!int.TryParse(txtQuantidade.Text, out quantidade))
            {
                Avisar("A quantidade deve ser um número inteiro. Exemplo: 120", txtQuantidade);
                return;
            }

            if (quantidade <= 0)
            {
                Avisar("A quantidade deve ser maior que zero.", txtQuantidade);
                return;
            }

            // SelectedIndex vale -1 quando nada foi selecionado no ComboBox.
            if (cmbTipo.SelectedIndex < 0)
            {
                Avisar("Escolha o tipo da fruta.", cmbTipo);
                return;
            }

            // A API guarda o NÚMERO (Id) da categoria, e não o texto. O Id só
            // existe quando as categorias vieram da API: se ela não respondeu,
            // o combo está com a lista reserva e ObterIdCategoria devolve 0.
            int idCategoria = FrutaApi.ObterIdCategoria(cmbTipo.SelectedItem.ToString());
            if (idCategoria == 0)
            {
                Avisar("Não foi possível obter as categorias da API. "
                    + "Verifique se a API está rodando e abra o programa de novo.", cmbTipo);
                return;
            }

            decimal valor;
            if (!decimal.TryParse(txtValor.Text, out valor))
            {
                Avisar("O valor deve ser um número. Exemplo: 7,49", txtValor);
                return;
            }

            if (valor <= 0)
            {
                Avisar("O valor deve ser maior que zero.", txtValor);
                return;
            }

            if (string.IsNullOrEmpty(caminhoImagem))
            {
                Avisar("Escolha a imagem da fruta.", btnEscolherImagem);
                return;
            }

            // ---------- 2) MONTAR O OBJETO ----------
            // A imagem viaja para a API "dentro" do JSON, e JSON só carrega
            // texto. Base64 é uma forma de escrever os bytes de um arquivo
            // usando só letras e números. O try/catch existe porque o arquivo
            // pode ter sido apagado ou movido depois de escolhido.
            string imagemBase64;
            try
            {
                imagemBase64 = Convert.ToBase64String(File.ReadAllBytes(caminhoImagem));
            }
            catch (Exception)
            {
                Avisar("Não foi possível ler o arquivo da imagem. Escolha a imagem de novo.",
                    btnEscolherImagem);
                return;
            }

            // Passou por toda a validação: agora juntamos os dados em um objeto
            // Fruta. Esta sintaxe com { } é o "inicializador de objeto" — é o
            // mesmo que criar o objeto e depois atribuir cada propriedade.
            Fruta fruta = new Fruta
            {
                Nome = txtNome.Text.Trim(),
                Preco = valor,
                Quantidade = quantidade,
                IdCategoria = idCategoria,

                // .Date joga fora a hora e fica só com o dia.
                DataValidade = dtpValidade.Value.Date,
                HashImg = imagemBase64
            };

            // ---------- 3) ENVIAR PARA A API ----------
            // Cadastrar() devolve false quando a API recusou a fruta ou não
            // respondeu. Nesse caso avisamos e SAÍMOS com return, sem limpar
            // os campos: o usuário não perde o que digitou e pode tentar de novo.
            if (!FrutaApi.Cadastrar(fruta))
            {
                MessageBox.Show("Não foi possível cadastrar a fruta. Tente novamente.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ---------- 4) AVISAR E IR PARA O ESTOQUE ----------
            MessageBox.Show(
                "Fruta \"" + fruta.Nome + "\" cadastrada com sucesso!",
                "Cadastro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            LimparCampos();
            AbrirEstoque();
        }

        /// <summary>
        /// Limpa o formulário para um novo cadastro.
        /// </summary>
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparCampos();
        }

        /// <summary>
        /// Vai para a tela de Estoque sem cadastrar nada.
        /// </summary>
        private void btnEstoque_Click(object sender, EventArgs e)
        {
            AbrirEstoque();
        }

        /// <summary>
        /// Mostra uma mensagem de atenção e devolve o foco para o controle errado.
        /// Criamos este método para não repetir o mesmo MessageBox nove vezes
        /// na validação — é o princípio de não repetir código.
        /// </summary>
        private void Avisar(string mensagem, Control controle)
        {
            MessageBox.Show(mensagem, "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            controle.Focus();
        }

        /// <summary>
        /// Devolve o formulário ao estado inicial.
        /// </summary>
        private void LimparCampos()
        {
            txtNome.Clear();
            txtQuantidade.Clear();
            txtValor.Clear();
            cmbTipo.SelectedIndex = -1;
            dtpValidade.Value = DateTime.Today;

            if (picPrevia.Image != null)
            {
                picPrevia.Image.Dispose();
                picPrevia.Image = null;
            }

            caminhoImagem = string.Empty;
            txtNome.Focus();
        }

        /// <summary>
        /// Abre a tela de Estoque e, quando ela fechar, volta para o Cadastro.
        /// </summary>
        private void AbrirEstoque()
        {
            // new cria a segunda tela na memória.
            // using garante que ela é liberada quando terminarmos.
            using (EstoqueForm estoque = new EstoqueForm())
            {
                // Esconde o Cadastro para as duas telas não ficarem empilhadas.
                this.Hide();

                // ShowDialog() PARA aqui e só continua na linha seguinte quando
                // o usuário fechar o Estoque. Se usássemos Show(), o código
                // continuaria correndo na hora e o Cadastro reapareceria já.
                estoque.ShowDialog();
            }

            // O Estoque foi fechado: mostramos o Cadastro de novo.
            this.Show();
        }
    }
}
