using FigureProj.Common.Models.Abstract;

namespace FigureProj.Common.Models
{
    public class Circle : Figure
    {
        private double _radius;
        public double Radius
        {
            get { return _radius; }
            set
            {
                if (value <= 0) { throw new ArgumentOutOfRangeException("Радіус не може бути менше нуля!"); }
                _radius = value;
                RaisePropertyChanged($"Радіус змінено на {value}");
            }
        }

        // Конструктор
        public Circle(double radius, string name, string color) : base(name, color)
        {
            Radius = radius;
        }

        // Статичний метод для створення нового об'єкта із згенерованими даними
        public static Circle CreateNew()
        {
            var random = Random.Shared;
            double radius = random.Next(1, 51) + random.NextDouble();
            string[] colorKeys = PresetColors.Keys.ToArray();
            string color = colorKeys[random.Next(colorKeys.Length)];
            string name = $"Коло-{random.Next(1000, 9999)}";
            
            return new Circle(radius, name, color);
        }

        // Метод
        public override string ToString()
        {
            return $"Фігура створена: id:{Id}, Ім'я: {Name} , Кольор: {Color}, Радіус: {Radius}";
        }

        // Метод
        public override double CalculateArea()
        {
            return Area = Math.PI * Math.Pow(Radius, 2);
        }

        // Метод
        public override double CalculatePerimetr()
        {
            return Perimeter = 2 * Math.PI * Radius;
        }

        // Метод
        public override void Draw()
        {
            int radius = 5;

            for (int y = radius; y >= -radius; y--)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        Console.Write("#");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
