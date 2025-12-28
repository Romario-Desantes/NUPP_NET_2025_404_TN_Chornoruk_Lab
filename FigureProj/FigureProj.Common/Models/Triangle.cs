using FigureProj.Common.Models.Abstract;

namespace FigureProj.Common.Models
{
    public class Triangle : Figure
    {
        private double _a;
        private double _b;
        private double _c;

        public double A
        {
            get { return _a; }
            set
            {
                if (value <= 0) { throw new ArgumentOutOfRangeException("Сторона не може бути менше нуля!"); }
                _a = value;
                RaisePropertyChanged($"Сторона A змінена на {value}");
            }
        }
        public double B
        {
            get { return _b; }
            set
            {
                if (value <= 0) { throw new ArgumentOutOfRangeException("Сторона не може бути менше нуля!"); }
                _b = value;
                RaisePropertyChanged($"Сторона B змінена на {value}");
            }
        }
        public double C
        {
            get { return _c; }
            set
            {
                if (value <= 0) { throw new ArgumentOutOfRangeException("Сторона не може бути менше нуля!"); }
                _c = value;
                RaisePropertyChanged($"Сторона C змінена на {value}");
            }
        }

        // Конструктор
        public Triangle(double a, double b, double c, string name, string color) : base(name, color)
        {
            if (a <= 0 || b <= 0 || c <= 0)
                throw new ArgumentOutOfRangeException("Усі сторони повинні бути більші за нуль!");

            if (!IsValidTriangle(a, b, c))
                throw new ArgumentException("Сторони не утворюють трикутник!");

            A = a;
            B = b;
            C = c;
        }

        // Статичний метод для створення нового об'єкта із згенерованими даними
        public static Triangle CreateNew()
        {
            var random = Random.Shared;
            double a, b, c;
            
            // Генеруємо коректний трикутник
            do
            {
                a = random.Next(1, 51) + random.NextDouble();
                b = random.Next(1, 51) + random.NextDouble();
                c = random.Next(1, 51) + random.NextDouble();
            } while (!IsValidTriangle(a, b, c));
            
            string[] colorKeys = PresetColors.Keys.ToArray();
            string color = colorKeys[random.Next(colorKeys.Length)];
            string name = $"Трикутник-{random.Next(1000, 9999)}";
            
            return new Triangle(a, b, c, name, color);
        }

        // Метод
        public override string ToString()
        {
            return $"Фігура створена: id:{Id}, Ім'я: {Name} , Кольор: {Color}, Перша сторона: {A}, Друга сторона: {B}, Третя сторона: {C}";
        }

        // Статичний метод
        private static bool IsValidTriangle(double a, double b, double c)
        {
            return a + b > c && a + c > b && b + c > a;
        }

        // Метод
        public override double CalculateArea()
        {
            double halfP = (A + B + C) / 2;
            return Area = Math.Sqrt(halfP * (halfP - A) * (halfP - B) * (halfP - C));
        }

        // Метод
        public override double CalculatePerimetr()
        {
            return Perimeter = A + B + C;
        }

        // Метод
        public override void Draw()
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6 - i - 1; j++)
                {
                    Console.Write(" ");
                }

                for (int j = 0; j < 2 * i + 1; j++)
                {
                    Console.Write("#");
                }

                Console.WriteLine();
            }
        }
    }
}
