using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace WindowsFormsAppFruteira
{
    /// <summary>
    /// Representa uma fruta do estoque, do jeito que a API de frutas devolve
    /// e espera receber. Cada fruta do JSON tem este formato:
    ///
    ///   {
    ///     "id": 1,
    ///     "nome": "Banana Prata",
    ///     "preco": 7.49,
    ///     "quantidade": 120,
    ///     "id_categoria": 2,
    ///     "data_validade": "2026-12-31T00:00:00",
    ///     "hash_img": "iVBORw0KGgo..."
    ///   }
    ///
    /// Uma classe de modelo serve para agrupar, em um único lugar, todas as
    /// informações que andam juntas. Em vez de carregar várias variáveis soltas
    /// (nome, quantidade, preço...), carregamos UM objeto Fruta.
    ///
    /// [DataContract] e [DataMember(Name = "...")] funcionam igual à classe
    /// Categoria: dizem ao leitor de JSON qual campo vai em cada propriedade.
    /// </summary>
    [DataContract]
    public class Fruta
    {
        /// <summary>
        /// Número que identifica a fruta na API. Quem cria este número é a
        /// API, na hora do cadastro — por isso uma fruta nova tem Id = 0.
        ///
        /// EmitDefaultValue = false faz o Id NÃO ser enviado quando vale 0,
        /// assim o cadastro não manda um "id": 0 para a API.
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int Id { get; set; }

        /// <summary>Nome da fruta. Ex.: "Banana Prata".</summary>
        [DataMember(Name = "nome")]
        public string Nome { get; set; }

        /// <summary>Preço da fruta. É decimal (e não double) porque decimal é o tipo correto para dinheiro.</summary>
        [DataMember(Name = "preco")]
        public decimal Preco { get; set; }

        /// <summary>Quantidade em estoque. É int porque não existe "meia caixa".</summary>
        [DataMember(Name = "quantidade")]
        public int Quantidade { get; set; }

        /// <summary>
        /// Número (Id) da categoria da fruta. Guardamos o NÚMERO, e não o
        /// texto "Tropical": se amanhã a categoria mudar de nome, as frutas
        /// continuam apontando para a categoria certa.
        ///
        /// Para trocar número por texto (e vice-versa) as telas usam
        /// FrutaApi.ObterDescricaoCategoria(...) e FrutaApi.ObterIdCategoria(...).
        /// </summary>
        [DataMember(Name = "id_categoria")]
        public int IdCategoria { get; set; }

        /// <summary>
        /// Data de validade. Repare que ela NÃO tem [DataMember]: quem conversa
        /// com o JSON é a propriedade DataValidadeTexto, logo abaixo.
        /// </summary>
        public DateTime DataValidade { get; set; }

        /// <summary>
        /// "Ponte" entre a data do JSON e a propriedade DataValidade.
        ///
        /// No JSON a data vem como TEXTO ("2026-12-31T00:00:00") e o leitor
        /// de JSON do .NET não sabe transformar esse texto em DateTime
        /// sozinho. Então ele lê e escreve esta propriedade de texto, e nós
        /// fazemos a conversão:
        ///
        ///   get = DateTime -> texto (usado ao ENVIAR a fruta para a API)
        ///   set = texto -> DateTime (usado ao RECEBER a fruta da API)
        ///
        /// É private porque as telas não precisam dela: elas usam DataValidade.
        /// </summary>
        [DataMember(Name = "data_validade")]
        private string DataValidadeTexto
        {
            get
            {
                return DataValidade.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
            }
            set
            {
                // TryParse (e não Parse) pelo mesmo motivo da tela de Cadastro:
                // se a data vier em um formato inesperado, ele só devolve false
                // em vez de quebrar a leitura da lista inteira. Nesse caso a
                // DataValidade fica com DateTime.MinValue (01/01/0001).
                //
                // RoundtripKind mantém a data exatamente como veio escrita,
                // sem converter fuso horário (o que poderia mudar o dia).
                DateTime data;
                DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out data);
                DataValidade = data;
            }
        }

        /// <summary>
        /// Imagem da fruta em Base64: os bytes do arquivo de imagem
        /// transformados em texto, que é a forma de mandar um arquivo
        /// "dentro" de um JSON.
        /// </summary>
        [DataMember(Name = "hash_img")]
        public string HashImg { get; set; }

        /// <summary>
        /// Lista RESERVA de tipos de fruta.
        ///
        /// A lista de verdade vem da API de categorias. Esta aqui só é usada
        /// quando a API não responde, para os combos não ficarem vazios.
        /// Como ela tem só os textos (sem o Id de cada categoria), com ela dá
        /// para ver as telas, mas não dá para cadastrar nem filtrar.
        ///
        /// As telas NÃO leem esta lista direto: tanto o cmbTipo (Cadastro)
        /// quanto o cmbCategoria (Estoque) chamam FrutaApi.ObterTipos(), que
        /// é a "fonte única da verdade". Assim é impossível cadastrar com uma
        /// lista e pesquisar com outra.
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
