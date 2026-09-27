using System;
using System.Collections.Generic;

namespace WindowsFormsAppFruteira
{
    /// <summary>
    /// Ponto ÚNICO de comunicação com a API da fruteira.
    ///
    /// As telas nunca falam HTTP direto: elas chamam FrutaApi.Cadastrar(...) e
    /// FrutaApi.Pesquisar(...). Se amanhã o endereço ou o formato da API mudar,
    /// só este arquivo muda — as telas continuam iguais.
    ///
    /// ATENÇÃO, ALUNO: a API ainda NÃO existe. Por isso o corpo de cada método
    /// está comentado e devolvendo um valor "vazio". O passo a passo para
    /// ativar de verdade está na etapa 10 do GUIA.md.
    /// </summary>
    public static class FrutaApi
    {
        /// <summary>Endereço base da API. Trocar quando a API estiver publicada.</summary>
        public const string BaseUrl = "https://localhost:5001";

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
