using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FigureProj.NoSql.Models
{
    /// <summary>
    /// Базовий документ для фігур у MongoDB
    /// </summary>
    public class FigureDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("color")]
        public string Color { get; set; } = string.Empty;

        [BsonElement("area")]
        public double Area { get; set; }

        [BsonElement("perimeter")]
        public double Perimeter { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        // Специфічні властивості для різних типів фігур
        [BsonElement("radius")]
        [BsonIgnoreIfDefault]
        public double? Radius { get; set; }

        [BsonElement("height")]
        [BsonIgnoreIfDefault]
        public double? Height { get; set; }

        [BsonElement("width")]
        [BsonIgnoreIfDefault]
        public double? Width { get; set; }

        [BsonElement("side")]
        [BsonIgnoreIfDefault]
        public double? Side { get; set; }

        [BsonElement("side_a")]
        [BsonIgnoreIfDefault]
        public double? A { get; set; }

        [BsonElement("side_b")]
        [BsonIgnoreIfDefault]
        public double? B { get; set; }

        [BsonElement("side_c")]
        [BsonIgnoreIfDefault]
        public double? C { get; set; }

        // Додаткові поля
        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        [BsonElement("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}


