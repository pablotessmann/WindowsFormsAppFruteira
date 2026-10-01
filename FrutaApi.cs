using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;

namespace WindowsFormsAppFruteira
{
    /// <summary>
    /// Ponto ÚNICO de comunicação com a API da fruteira.
    ///
    /// As telas nunca falam HTTP direto: elas chamam FrutaApi.ObterTipos(),
    /// FrutaApi.Cadastrar(...) e FrutaApi.Pesquisar(...). Se amanhã o endereço
    /// ou o formato da API mudar, só este arquivo muda — as telas continuam
    /// iguais.
    ///
    /// ATENÇÃO, ALUNO: a API de FRUTAS ainda NÃO existe. Por isso o corpo de
    /// Cadastrar e de Pesquisar está comentado e devolvendo um valor "vazio".
    /// O passo a passo para ativar de verdade está na etapa 10 do GUIA.md.
    ///
    /// Já a API de CATEGORIAS existe (é outro projeto, que roda separado
    /// deste) e a chamada a ela está ligada de verdade em ListarCategorias().
    /// </summary>
    public static class FrutaApi
    {
        /// <summary>Endereço base da API. Trocar quando a API estiver publicada.</summary>
        public const string BaseUrl = "https://localhost:5001";

        /// <summary>
        /// Endereço da API de categorias. Ela fica em outro projeto e em outra
        /// porta, por isso tem um endereço só dela.
        /// </summary>
        public const string UrlCategorias = "https://localhost:7069/Categoria";

        /// <summary>
        /// Guarda os tipos já carregados, para o programa perguntar à API uma
        /// vez só. Começa null, que aqui significa "ainda não carregamos".
        /// </summary>
        private static string[] tiposCarregados;

        /// <summary>
        /// Busca na API a lista de categorias de fruta.
        /// </summary>
        /// <returns>
        /// As categorias encontradas. Lista vazia se a API estiver fora do ar
        /// ou responder com erro.
        /// </returns>
        public static List<Categoria> ListarCategorias()
        {
            // ----------------------------------------------------------------
            // CHAMADA
            //
            //   GET https://localhost:7069/Categoria
            //
            //   Resposta esperada: 200 OK, com um vetor JSON
            //   [
            //     { "id": 1, "descricao": "Ácida" },
            //     ...
            //   ]
            // ----------------------------------------------------------------

            // O try/catch é obrigatório aqui: se a API estiver desligada, o
            // HttpClient lança uma exceção. Sem o catch o programa fecharia
            // na cara do usuário só porque o servidor não respondeu.
            try
            {
                using (HttpClient cliente = new HttpClient())
                {
                    // Sem isto o HttpClient espera até 100 segundos por uma
                    // resposta, com a tela congelada. 5 segundos bastam para
                    // uma API que está na própria máquina.
                    cliente.Timeout = TimeSpan.FromSeconds(5);

                    HttpResponseMessage resposta = cliente.GetAsync(UrlCategorias).Result;

                    if (!resposta.IsSuccessStatusCode)
                    {
                        return new List<Categoria>();
                    }

                    // O DataContractJsonSerializer transforma o JSON em objetos.
                    // Ele já vem no .NET (referência System.Runtime.Serialization)
                    // e usa os [DataMember] da classe Categoria para saber qual
                    // campo do JSON vai em cada propriedade.
                    using (Stream corpo = resposta.Content.ReadAsStreamAsync().Result)
                    {
                        DataContractJsonSerializer leitor =
                            new DataContractJsonSerializer(typeof(List<Categoria>));

                        List<Categoria> categorias = (List<Categoria>)leitor.ReadObject(corpo);

                        // Se a API responder "null" no lugar do vetor, o leitor
                        // devolve null. Trocamos por lista vazia para quem
                        // chamou poder usar o foreach sem medo.
                        if (categorias == null)
                        {
                            return new List<Categoria>();
                        }

                        return categorias;
                    }
                }
            }
            catch (Exception)
            {
                // API desligada, demorou demais, certificado não confiável ou
                // JSON em formato inesperado: em todos os casos devolvemos
                // lista vazia, e ObterTipos() decide o que fazer.
                return new List<Categoria>();
            }
        }

        /// <summary>
        /// Devolve os textos que preenchem o cmbTipo (Cadastro) e o
        /// cmbCategoria (Estoque).
        ///
        /// Vem da API de categorias. Se ela não responder, usamos a lista
        /// reserva <see cref="Fruta.Tipos"/>, para o programa continuar
        /// funcionando mesmo com a API desligada.
        /// </summary>
        public static string[] ObterTipos()
        {
            // Já carregamos antes? Então devolvemos o que está guardado.
            // Além de evitar uma chamada a cada tela aberta, isto garante que
            // o Cadastro e o Estoque mostrem SEMPRE a mesma lista. Para buscar
            // as categorias de novo, basta fechar e abrir o programa.
            if (tiposCarregados != null)
            {
                return tiposCarregados;
            }

            // Os combos só precisam do texto, então tiramos a Descricao de
            // cada categoria e ignoramos as que vierem sem texto.
            List<string> descricoes = new List<string>();

            foreach (Categoria categoria in ListarCategorias())
            {
                if (categoria != null && !string.IsNullOrWhiteSpace(categoria.Descricao))
                {
                    descricoes.Add(categoria.Descricao.Trim());
                }
            }

            if (descricoes.Count == 0)
            {
                // A API não respondeu (ou não tem categoria nenhuma).
                tiposCarregados = Fruta.Tipos;
            }
            else
            {
                tiposCarregados = descricoes.ToArray();
            }

            return tiposCarregados;
        }

        /// <summary>
        /// Envia uma fruta nova para a API.
        /// </summary>
        /// <returns>true se a API confirmou o cadastro.</returns>
        public static bool Cadastrar(Fruta fruta)
        {
            // ----------------------------------------------------------------
            // CHAMADA ESPERADA
            //
            //   POST {BaseUrl}/api/frutas
            //   Content-Type: multipart/form-data
            //
            //   nome      = "Banana Prata"
            //   quantidade= 120
            //   tipo      = "Tropical"
            //   valor     = 7.49
            //   validade  = "2026-12-31"        (formato yyyy-MM-dd)
            //   imagem    = <bytes do arquivo>  (o arquivo em fruta.CaminhoImagem)
            //
            //   Resposta esperada: 201 Created
            //
            // Usamos multipart/form-data (e não JSON) porque estamos enviando
            // um ARQUIVO junto com os campos de texto.
            // ----------------------------------------------------------------

            // using (HttpClient cliente = new HttpClient())
            // using (MultipartFormDataContent corpo = new MultipartFormDataContent())
            // {
            //     corpo.Add(new StringContent(fruta.Nome), "nome");
            //     corpo.Add(new StringContent(fruta.Quantidade.ToString()), "quantidade");
            //     corpo.Add(new StringContent(fruta.Tipo), "tipo");
            //
            //     // CultureInfo.InvariantCulture faz o decimal virar "7.49" (ponto)
            //     // e não "7,49" (vírgula), que é o que a API espera.
            //     corpo.Add(new StringContent(
            //         fruta.Valor.ToString(CultureInfo.InvariantCulture)), "valor");
            //
            //     corpo.Add(new StringContent(
            //         fruta.Validade.ToString("yyyy-MM-dd")), "validade");
            //
            //     byte[] bytesDaImagem = File.ReadAllBytes(fruta.CaminhoImagem);
            //     ByteArrayContent arquivo = new ByteArrayContent(bytesDaImagem);
            //     corpo.Add(arquivo, "imagem", Path.GetFileName(fruta.CaminhoImagem));
            //
            //     HttpResponseMessage resposta =
            //         cliente.PostAsync(BaseUrl + "/api/frutas", corpo).Result;
            //
            //     return resposta.IsSuccessStatusCode;
            // }

            // Enquanto não há API, devolvemos false para deixar claro que
            // NADA foi gravado de verdade.
            return false;
        }

        /// <summary>
        /// Busca as frutas do estoque aplicando os filtros da tela.
        /// </summary>
        /// <param name="categoria">Tipo da fruta. String vazia = todas.</param>
        /// <param name="nome">Parte do nome da fruta. String vazia = todos.</param>
        /// <returns>As frutas encontradas. Lista vazia se não houver nenhuma.</returns>
        public static List<Fruta> Pesquisar(string categoria, string nome)
        {
            // ----------------------------------------------------------------
            // CHAMADA ESPERADA
            //
            //   GET {BaseUrl}/api/frutas?categoria=Tropical&nome=banana
            //
            //   Os dois parâmetros são opcionais: mandar vazio significa
            //   "não filtrar por este campo".
            //
            //   Resposta esperada: 200 OK, com um vetor JSON
            //   [
            //     {
            //       "nome": "Banana Prata",
            //       "quantidade": 120,
            //       "tipo": "Tropical",
            //       "valor": 7.49,
            //       "validade": "2026-12-31",
            //       "caminhoImagem": "banana.png"
            //     }
            //   ]
            // ----------------------------------------------------------------

            // using (HttpClient cliente = new HttpClient())
            // {
            //     // Uri.EscapeDataString troca espaços e acentos por código,
            //     // senão um nome como "banana prata" quebra a URL.
            //     string url = BaseUrl + "/api/frutas"
            //         + "?categoria=" + Uri.EscapeDataString(categoria)
            //         + "&nome=" + Uri.EscapeDataString(nome);
            //
            //     HttpResponseMessage resposta = cliente.GetAsync(url).Result;
            //
            //     if (!resposta.IsSuccessStatusCode)
            //     {
            //         return new List<Fruta>();
            //     }
            //
            //     string json = resposta.Content.ReadAsStringAsync().Result;
            //
            //     // Para transformar o JSON em List<Fruta> é preciso adicionar
            //     // uma biblioteca de desserialização — veja a etapa 10 do GUIA.md.
            //     return JsonConvert.DeserializeObject<List<Fruta>>(json);
            // }

            // Enquanto não há API, devolvemos uma lista VAZIA.
            // Uma lista vazia é diferente de null: a tela pode percorrê-la
            // sem estourar erro.
            return new List<Fruta>();
        }
    }
}
