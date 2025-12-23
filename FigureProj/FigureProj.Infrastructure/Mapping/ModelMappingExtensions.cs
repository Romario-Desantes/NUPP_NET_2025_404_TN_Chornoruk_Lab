using FigureProj.Common.Models;
using FigureProj.Common.Models.Abstract;
using FigureProj.Infrastructure.Models;

namespace FigureProj.Infrastructure.Mapping
{
    public static class ModelMappingExtensions
    {
        // Domain -> Database
        public static FigureModel ToDbModel(this Figure domainFigure)
        {
            return domainFigure switch
            {
                Circle circle => new CircleModel
                {
                    Name = circle.Name,
                    Color = circle.Color,
                    Area = circle.Area,
                    Perimeter = circle.Perimeter,
                    CreatedAt = DateTime.UtcNow,
                    Radius = circle.Radius
                },
                Rectangle rectangle => new RectangleModel
                {
                    Name = rectangle.Name,
                    Color = rectangle.Color,
                    Area = rectangle.Area,
                    Perimeter = rectangle.Perimeter,
                    CreatedAt = DateTime.UtcNow,
                    Height = rectangle.Height,
                    Width = rectangle.Width
                },
                Square square => new SquareModel
                {
                    Name = square.Name,
                    Color = square.Color,
                    Area = square.Area,
                    Perimeter = square.Perimeter,
                    CreatedAt = DateTime.UtcNow,
                    Side = square.Side
                },
                Triangle triangle => new TriangleModel
                {
                    Name = triangle.Name,
                    Color = triangle.Color,
                    Area = triangle.Area,
                    Perimeter = triangle.Perimeter,
                    CreatedAt = DateTime.UtcNow,
                    A = triangle.A,
                    B = triangle.B,
                    C = triangle.C
                },
                _ => throw new ArgumentException($"Unknown figure type: {domainFigure.GetType().Name}")
            };
        }

        // Database -> Domain
        public static Figure ToDomainModel(this FigureModel dbModel)
        {
            Figure figure = dbModel switch
            {
                CircleModel circle => new Circle(circle.Radius, circle.Name, circle.Color),
                RectangleModel rectangle => new Rectangle(rectangle.Height, rectangle.Width, rectangle.Name, rectangle.Color),
                SquareModel square => new Square(square.Side, square.Name, square.Color),
                TriangleModel triangle => new Triangle(triangle.A, triangle.B, triangle.C, triangle.Name, triangle.Color),
                _ => throw new ArgumentException($"Unknown figure model type: {dbModel.GetType().Name}")
            };
            
            // Recalculate Area and Perimeter (they are calculated properties)
            figure.CalculateArea();
            figure.CalculatePerimetr();
            
            return figure;
        }

        // Update existing database model from domain model
        public static void UpdateFromDomain(this FigureModel dbModel, Figure domainFigure)
        {
            dbModel.Name = domainFigure.Name;
            dbModel.Color = domainFigure.Color;
            dbModel.Area = domainFigure.Area;
            dbModel.Perimeter = domainFigure.Perimeter;

            switch (dbModel, domainFigure)
            {
                case (CircleModel circleDb, Circle circleDomain):
                    circleDb.Radius = circleDomain.Radius;
                    break;
                case (RectangleModel rectangleDb, Rectangle rectangleDomain):
                    rectangleDb.Height = rectangleDomain.Height;
                    rectangleDb.Width = rectangleDomain.Width;
                    break;
                case (SquareModel squareDb, Square squareDomain):
                    squareDb.Side = squareDomain.Side;
                    break;
                case (TriangleModel triangleDb, Triangle triangleDomain):
                    triangleDb.A = triangleDomain.A;
                    triangleDb.B = triangleDomain.B;
                    triangleDb.C = triangleDomain.C;
                    break;
            }
        }
    }
}

