using FigureProj.Common.Models.Abstract;

namespace FigureProj.Common.Models
{
    public class Rectangle : Figure
    {
        private double _height;
        private double _width;
        public double Height
        {
            get { return _height; }
            set
            {
                if (value <= 0) {throw new ArgumentOutOfRangeException("Сторона не може бути менше нуля!");}
                _height = value;
            }
        }
        public double Width
        {
            get { return _width; }
            set
            {
                if (value <= 0) { throw new ArgumentOutOfRangeException("Сторона не може бути менше нуля!"); }
                _width = value;
            }
        }
        public Rectangle(double a, double b, string name, string color) : base(name, color) 
        {
            Height = a;
            Width = b;
        }

        public override string ToString()
        {
            return $"Фігура створена: id:{Id}, Ім'я: {Name} , Кольор: {Color}, Висота: {Height}, Ширина: {Width}";
        }

        public override double CalculateArea()
        {
            return Area = Height * Width;
        }

        public override double CalculatePerimetr()
        {
            return Perimeter = Height * 2 + Width * 2;
        }

        public override void Draw()
        {
            for (int i = 0; i < 7; i++)
            {
                Console.Write("#");
                for (int j = 0; j < 9; j++)
                {
                    Console.Write("#");
                }
                Console.WriteLine();
            }
        }
    }
}
