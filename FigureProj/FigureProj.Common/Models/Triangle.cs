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
            }
        }
        public double B
        {
            get { return _b; }
            set
            {
                if (value <= 0) { throw new ArgumentOutOfRangeException("Сторона не може бути менше нуля!"); }
                _b = value;
            }
        }
        public double C
        {
            get { return _c; }
            set
            {
                if (value <= 0) { throw new ArgumentOutOfRangeException("Сторона не може бути менше нуля!"); }
                _c = value;
            }
        }

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

        public override string ToString()
        {
            return $"Фігура створена: id:{Id}, Ім'я: {Name} , Кольор: {Color}, Перша сторона: {A}, Друга сторона: {B}, Третя сторона: {C}";
        }

        private static bool IsValidTriangle(double a, double b, double c)
        {
            return a + b > c && a + c > b && b + c > a;
        }

        public override double CalculateArea()
        {
            double halfP = (A + B + C) / 2;
            return Area = Math.Sqrt(halfP * (halfP - A) * (halfP - B) * (halfP - C));
        }

        public override double CalculatePerimetr()
        {
            return Perimeter = A + B + C;
        }

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
