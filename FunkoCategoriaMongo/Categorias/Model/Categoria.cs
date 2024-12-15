using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FunkoCategoriaMongo.Categorias.Model;

public class Categoria
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [JsonPropertyName("nombre")] 
    public string Nombre { get; set; }

    [JsonPropertyName("precio")] 
    public double Precio { get; set; } = 0.0;
    
    [JsonPropertyName("tipo")]
    public string Tipo { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    
}