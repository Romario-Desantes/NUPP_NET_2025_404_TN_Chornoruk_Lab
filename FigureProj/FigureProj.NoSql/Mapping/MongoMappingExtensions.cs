using FigureProj.Common.Models;
using FigureProj.Common.Models.Abstract;
using FigureProj.NoSql.Models;

namespace FigureProj.NoSql.Mapping
{
    public static class MongoMappingExtensions
    {
        // Domain -> MongoDB Document
        public static FigureDocument ToDocument(this Figure domainFigure)
        {
            var document = new FigureDocument
            {
                Name = domainFigure.Name,
                Color = domainFigure.Color,
                Area = domainFigure.Area,
                Perimeter = domainFigure.Perimeter,
                CreatedAt = DateTime.UtcNow
            };

            switch (domainFigure)
            {
                case Circle circle:
                    document.Type = "Circle";
                    document.Radius = circle.Radius;
                    break;
                case Rectangle rectangle:
                    document.Type = "Rectangle";
                    document.Height = rectangle.Height;
                    document.Width = rectangle.Width;
                    break;
                case Square square:
                    document.Type = "Square";
                    document.Side = square.Side;
                    break;
                case Triangle triangle:
                    document.Type = "Triangle";
                    document.A = triangle.A;
                    document.B = triangle.B;
                    document.C = triangle.C;
                    break;
            }

            return document;
        }

        // MongoDB Document -> Domain
        public static Figure? ToDomainModel(this FigureDocument document)
        {
            Figure? figure = document.Type switch
            {
                "Circle" when document.Radius.HasValue => 
                    new Circle(document.Radius.Value, document.Name, document.Color),
                "Rectangle" when document.Height.HasValue && document.Width.HasValue => 
                    new Rectangle(document.Height.Value, document.Width.Value, document.Name, document.Color),
                "Square" when document.Side.HasValue => 
                    new Square(document.Side.Value, document.Name, document.Color),
                "Triangle" when document.A.HasValue && document.B.HasValue && document.C.HasValue => 
                    new Triangle(document.A.Value, document.B.Value, document.C.Value, document.Name, document.Color),
                _ => null
            };

            if (figure != null)
            {
                // Recalculate Area and Perimeter (they are calculated properties)
                figure.CalculateArea();
                figure.CalculatePerimetr();
            }

            return figure;
        }
    }
}

