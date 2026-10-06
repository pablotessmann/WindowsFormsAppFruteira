using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;

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
    /// A API é outro projeto, que roda separado deste. Ela tem duas partes,
    /// e as duas estão ligadas de verdade aqui:
    ///
    ///   /Categoria -> ListarCategorias()
    ///   /Fruta     -> Pesquisar(...) e Cadastrar(...)
    ///
    /// Os detalhes de cada chamada estão na etapa 10 do GUIA.md.
    /// </summary>
    public static class FrutaApi
    {
        /// <summary>Endereço da API de categorias.</summary>
        public const string UrlCategorias = "https://localhost:7069/Categoria";

        /// <summary>
        /// Endereço da API de frutas. O mesmo endereço serve para pesquisar
        /// (GET) e para cadastrar (POST): o que muda é o verbo da chamada.
        /// </summary>
        public const string UrlFrutas = "https://localhost:7069/Fruta";

        /// <summary>
        /// Guarda os tipos já carregados, para o programa perguntar à API uma
        /// vez só. Começa null, que aqui significa "ainda não carregamos".
        /// </summary>
        private static string[] tiposCarregados;

        /// <summary>
        /// Guarda as categorias completas (Id + Descricao) que vieram da API.
        /// Os combos mostram só o texto, mas a API de frutas trabalha com o
        /// Id — é esta lista que permite trocar um pelo outro. Fica vazia
        /// quando a API não respondeu e os combos usam a lista reserva.
        /// </summary>
        private static List<Categoria> categoriasCarregadas = new List<Categoria>();

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
            // cada categoria e ignoramos as que vierem sem texto. A categoria
            // completa também é guardada, para sabermos o Id de cada texto.
            List<string> descricoes = new List<string>();
            List<Categoria> categorias = new List<Categoria>();

            foreach (Categoria categoria in ListarCategorias())
            {
                if (categoria != null && !string.IsNullOrWhiteSpace(categoria.Descricao))
                {
                    categoria.Descricao = categoria.Descricao.Trim();

                    descricoes.Add(categoria.Descricao);
                    categorias.Add(categoria);
                }
            }

            categoriasCarregadas = categorias;

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
        /// Descobre o Id de uma categoria a partir do texto escolhido no combo.
        /// </summary>
        /// <returns>
        /// O Id da categoria. Zero se o texto não for de nenhuma categoria da
        /// API — é o que acontece quando os combos estão com a lista reserva.
        /// </returns>
        public static int ObterIdCategoria(string descricao)
        {
            // Garante que as categorias já foram buscadas na API.
            ObterTipos();

            foreach (Categoria categoria in categoriasCarregadas)
            {
                if (categoria.Descricao == descricao)
                {
                    return categoria.Id;
                }
            }

            return 0;
        }

        /// <summary>
        /// Faz o caminho contrário: a partir do Id que vem em cada fruta,
        /// descobre o texto da categoria para mostrar na tela.
        /// </summary>
        /// <returns>O texto da categoria. String vazia se o Id não existir.</returns>
        public static string ObterDescricaoCategoria(int idCategoria)
        {
            ObterTipos();

            foreach (Categoria categoria in categoriasCarregadas)
            {
                if (categoria.Id == idCategoria)
                {
                    return categoria.Descricao;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Envia uma fruta nova para a API.
        /// </summary>
        /// <returns>true se a API confirmou o cadastro.</returns>
        public static bool Cadastrar(Fruta fruta)
        {
            // ----------------------------------------------------------------
            // CHAMADA
            //
            //   POST https://localhost:7069/Fruta
            //   Content-Type: application/json
            //
            //   {
            //     "nome": "Banana Prata",
            //     "preco": 7.49,
            //     "quantidade": 120,
            //     "id_categoria": 2,
            //     "data_validade": "2026-12-31T00:00:00",
            //     "hash_img": "iVBORw0KGgo..."     (a imagem em Base64)
            //   }
            //
            //   O "id" não é enviado: quem cria o número é a API.
            //
            //   Resposta esperada: um código de sucesso (200 OK ou 201 Created)
            // ----------------------------------------------------------------

            try
            {
                // 1) Transformar o objeto Fruta em texto JSON. É o caminho
                //    inverso do que ListarCategorias() faz: lá usamos
                //    ReadObject (JSON -> objeto), aqui WriteObject
                //    (objeto -> JSON).
                string json;

                using (MemoryStream memoria = new MemoryStream())
                {
                    DataContractJsonSerializer escritor =
                        new DataContractJsonSerializer(typeof(Fruta));

                    escritor.WriteObject(memoria, fruta);

                    json = Encoding.UTF8.GetString(memoria.ToArray());
                }

                // 2) Enviar. O StringContent leva o texto e avisa a API, pelo
                //    "application/json", de que aquele texto é um JSON.
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
                // API desligada, demorou demais ou certificado não confiável:
                // devolvemos false para a tela avisar que NADA foi gravado.
                return false;
            }
        }

        /// <summary>
        /// Busca as frutas do estoque aplicando os filtros da tela.
        /// Sem nenhum filtro, traz TODAS as frutas.
        /// </summary>
        /// <param name="idCategoria">Id da categoria. Zero = todas.</param>
        /// <param name="nome">Parte do nome da fruta. String vazia = todos.</param>
        /// <returns>
        /// As frutas encontradas. Lista vazia se não houver nenhuma, ou se a
        /// API estiver fora do ar.
        /// </returns>
        public static List<Fruta> Pesquisar(int idCategoria, string nome)
        {
            // ----------------------------------------------------------------
            // CHAMADA
            //
            //   GET https://localhost:7069/Fruta                          (tudo)
            //   GET https://localhost:7069/Fruta?id_categoria=2
            //   GET https://localhost:7069/Fruta?nome=banana
            //   GET https://localhost:7069/Fruta?id_categoria=2&nome=banana
            //
            //   Os dois filtros são opcionais: só entra no endereço o filtro
            //   que o usuário preencheu.
            //
            //   Resposta esperada: 200 OK, com um vetor JSON
            //   [
            //     {
            //       "id": 1,
            //       "nome": "Banana Prata",
            //       "preco": 7.49,
            //       "quantidade": 120,
            //       "id_categoria": 2,
            //       "data_validade": "2026-12-31T00:00:00",
            //       "hash_img": "iVBORw0KGgo..."
            //     }
            //   ]
            // ----------------------------------------------------------------

            // 1) Montar o endereço. Os filtros vão depois de um "?", separados
            //    por "&". Guardamos cada filtro preenchido em uma lista e no
            //    fim juntamos tudo com string.Join.
            List<string> filtros = new List<string>();

            if (idCategoria > 0)
            {
                filtros.Add("id_categoria=" + idCategoria);
            }

            if (!string.IsNullOrWhiteSpace(nome))
            {
                // Uri.EscapeDataString troca espaços e acentos por código,
                // senão um nome como "banana prata" quebra o endereço.
                filtros.Add("nome=" + Uri.EscapeDataString(nome));
            }

            string url = UrlFrutas;

            // Sem filtro nenhum o endereço fica só ".../Fruta", e a API
            // devolve todas as frutas.
            if (filtros.Count > 0)
            {
                url = url + "?" + string.Join("&", filtros);
            }

            // 2) Chamar a API e ler a resposta, do mesmo jeito que em
            //    ListarCategorias().
            try
            {
                using (HttpClient cliente = new HttpClient())
                {
                    cliente.Timeout = TimeSpan.FromSeconds(5);

                    HttpResponseMessage resposta = cliente.GetAsync(url).Result;

                    if (!resposta.IsSuccessStatusCode)
                    {
                        return new List<Fruta>();
                    }

                    using (Stream corpo = resposta.Content.ReadAsStreamAsync().Result)
                    {
                        DataContractJsonSerializer leitor =
                            new DataContractJsonSerializer(typeof(List<Fruta>));

                        List<Fruta> frutas = (List<Fruta>)leitor.ReadObject(corpo);

                        if (frutas == null)
                        {
                            return new List<Fruta>();
                        }

                        return frutas;
                    }
                }
            }
            catch (Exception)
            {
                // Em caso de falha devolvemos uma lista VAZIA.
                // Uma lista vazia é diferente de null: a tela pode percorrê-la
                // sem estourar erro.
                return new List<Fruta>();
            }
        }
    }
}
