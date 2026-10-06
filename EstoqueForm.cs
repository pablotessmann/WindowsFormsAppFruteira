using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsAppFruteira
{
    public partial class EstoqueForm : Form
    {
        /// <summary>
        /// Texto da primeira opção do filtro de categoria.
        /// É uma constante para o texto não ficar espalhado (e escrito de
        /// formas diferentes) em vários pontos do código.
        /// </summary>
        private const string TodasAsCategorias = "(Todas)";

        public EstoqueForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Executa uma única vez, quando a tela abre.
        /// </summary>
        private void EstoqueForm_Load(object sender, EventArgs e)
        {
            // Primeiro a opção "não filtrar"...
            cmbCategoria.Items.Add(TodasAsCategorias);

            // ...e depois os mesmos tipos usados na tela de Cadastro.
            // É por isso que os dois combos chamam FrutaApi.ObterTipos(): a
            // categoria escolhida no cadastro é a mesma que aparece aqui.
            cmbCategoria.Items.AddRange(FrutaApi.ObterTipos());

            // Começa em "(Todas)" para a primeira pesquisa trazer tudo.
            cmbCategoria.SelectedIndex = 0;
        }

        /// <summary>
        /// Lê os dois filtros, busca na API e monta a lista.
        /// </summary>
        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            // A API de frutas filtra pelo NÚMERO (Id) da categoria, e não pelo
            // texto. O zero é o nosso combinado para "não filtre por este
            // campo" — é o que vale para "(Todas)", que não é uma categoria
            // de verdade.
            int idCategoria = 0;

            string categoria = cmbCategoria.SelectedItem.ToString();
            if (categoria != TodasAsCategorias)
            {
                // Troca o texto escolhido no combo pelo Id da categoria.
                idCategoria = FrutaApi.ObterIdCategoria(categoria);
            }

            // Trim() remove espaços sobrando no começo e no fim do que o
            // usuário digitou.
            string nome = txtNome.Text.Trim();

            // Com "(Todas)" e o nome em branco não há filtro nenhum: a
            // pesquisa acontece do mesmo jeito e traz TODAS as frutas.
            List<Fruta> encontradas = FrutaApi.Pesquisar(idCategoria, nome);

            // ---------------------------------------------------------------
            // SÓ PARA VISUALIZAR O LAYOUT DA TABELA
            //
            // Se a API estiver desligada, a lista acima volta vazia e nada
            // aparece na tela. Para ver como as linhas ficam, comente a linha
            // "List<Fruta> encontradas = FrutaApi.Pesquisar(...)" e descomente
            // o bloco abaixo. Depois desfaça — isto NÃO é o cadastro de verdade.
            //
            // List<Fruta> encontradas = new List<Fruta>
            // {
            //     new Fruta { Nome = "Banana Prata", Quantidade = 120, IdCategoria = 1,
            //                 Preco = 7.49m, DataValidade = DateTime.Today.AddDays(10) },
            //     new Fruta { Nome = "Laranja Pera", Quantidade = 80, IdCategoria = 2,
            //                 Preco = 4.90m, DataValidade = DateTime.Today.AddDays(20) },
            //     new Fruta { Nome = "Morango", Quantidade = 35, IdCategoria = 3,
            //                 Preco = 12.00m, DataValidade = DateTime.Today.AddDays(5) }
            // };
            // ---------------------------------------------------------------

            PreencherLista(encontradas);
        }

        /// <summary>
        /// Transforma uma lista de objetos Fruta nas linhas do ListView.
        ///
        /// Este método é o "encaixe" entre os dados e a tela: não importa se as
        /// frutas vieram da API ou de outro lugar, ele só sabe desenhá-las.
        /// </summary>
        private void PreencherLista(List<Fruta> frutas)
        {
            // Sempre limpar antes de preencher, senão o resultado da pesquisa
            // nova ficaria grudado embaixo do resultado da pesquisa anterior.
            lstEstoque.Items.Clear();

            if (frutas == null || frutas.Count == 0)
            {
                MessageBox.Show(
                    "Nenhuma fruta encontrada com estes filtros.",
                    "Pesquisar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // foreach percorre a lista de fruta em fruta.
            foreach (Fruta fruta in frutas)
            {
                // Em um ListView com View = Details, cada linha é um
                // ListViewItem. O texto do ListViewItem é a PRIMEIRA coluna;
                // as colunas seguintes são adicionadas como SubItems, sempre
                // NA MESMA ORDEM em que as colunas aparecem na tela:
                // Fruta | Quantidade | Validade | Valor | Tipo
                ListViewItem linha = new ListViewItem(fruta.Nome);

                linha.SubItems.Add(fruta.Quantidade.ToString());

                // ToShortDateString() mostra 31/12/2026 em vez da data com hora.
                linha.SubItems.Add(fruta.DataValidade.ToShortDateString());

                // "C" é o formato de moeda: 7.49 aparece como R$ 7,49.
                linha.SubItems.Add(fruta.Preco.ToString("C"));

                // A fruta guarda só o Id da categoria; para o usuário
                // mostramos o texto ("Tropical"), que a FrutaApi sabe achar.
                linha.SubItems.Add(FrutaApi.ObterDescricaoCategoria(fruta.IdCategoria));

                lstEstoque.Items.Add(linha);
            }
        }

        /// <summary>
        /// Fecha o Estoque. Quem abriu esta tela (o Cadastro) reaparece sozinho.
        /// </summary>
        private void btnVoltar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
