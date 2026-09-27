using System;

namespace WindowsFormsAppFruteira
{
    /// <summary>
    /// Representa uma fruta do estoque.
    /// Uma classe de modelo serve para agrupar, em um único lugar, todas as
    /// informações que andam juntas. Em vez de carregar cinco variáveis soltas
    /// (nome, quantidade, tipo...), carregamos UM objeto Fruta.
    /// </summary>
    public class Fruta
    {
        /// <summary>Nome da fruta. Ex.: "Banana Prata".</summary>
        public string Nome { get; set; }

        /// <summary>Quantidade em estoque. É int porque não existe "meia caixa".</summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Tipo da fruta. É também o campo pesquisado pelo filtro
        /// "Categoria" da tela de Estoque, por isso os valores possíveis
        /// ficam fixos em <see cref="Tipos"/>.
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>Preço da fruta. É decimal (e não double) porque decimal é o tipo correto para dinheiro.</summary>
        public decimal Valor { get; set; }

        /// <summary>Data de validade.</summary>
        public DateTime Validade { get; set; }

        /// <summary>Caminho do arquivo de imagem escolhido no disco. Ex.: "C:\Fotos\banana.png".</summary>
        public string CaminhoImagem { get; set; }

        /// <summary>
        /// Tipos de fruta aceitos pelo sistema.
        ///
        /// Esta lista é a "fonte única da verdade": a tela de Cadastro a usa
        /// para preencher o cmbTipo e a tela de Estoque a usa para preencher o
        /// cmbCategoria. Assim é impossível cadastrar "Citrica" e pesquisar
        /// "Cítrica" e não achar nada.
        ///
        /// static  = pertence à classe Fruta, não a uma fruta específica.
        /// readonly = ninguém pode trocar a lista depois que o programa inicia.
        /// </summary>
        public static readonly string[] Tipos =
        {
            "Cítrica",
            "Tropical",
            "Vermelha",
            "Comum",
            "Seca"
        };
    }
}
