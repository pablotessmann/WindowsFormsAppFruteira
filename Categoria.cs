using System.Runtime.Serialization;

namespace WindowsFormsAppFruteira
{
    /// <summary>
    /// Representa uma categoria de fruta, do jeito que a API de categorias
    /// devolve. Cada item do JSON tem este formato:
    ///
    ///   { "id": 1, "descricao": "Ácida" }
    ///
    /// [DataContract] avisa o leitor de JSON de que esta classe pode ser
    /// montada a partir de um JSON.
    /// [DataMember(Name = "...")] diz qual campo do JSON vai em cada
    /// propriedade. Ele é necessário porque no JSON o nome vem em minúsculo
    /// ("descricao") e em C# o costume é começar com maiúscula (Descricao).
    /// </summary>
    [DataContract]
    public class Categoria
    {
        /// <summary>Número que identifica a categoria na API.</summary>
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>Texto que aparece para o usuário. Ex.: "Ácida".</summary>
        [DataMember(Name = "descricao")]
        public string Descricao { get; set; }
    }
}
