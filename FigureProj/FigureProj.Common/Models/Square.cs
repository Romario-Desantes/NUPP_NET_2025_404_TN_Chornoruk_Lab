using FigureProj.Common.Models.Abstract;

namespace FigureProj.Common.Models
{
    public class Square : Figure
    {
        private double _side;
        public double Side
        {
            get { return _side; }
            set
            {
                if (value <= 0) { throw new ArgumentOutOfRangeException("Сторона не може бути менше нуля!"); }
                _side = value;
                RaisePropertyChanged($"Сторона змінена на {value}");
            }
        }

        // Конструктор без параметрів для JSON десеріалізації
        public Square() : base()
        {
        }

        // Конструктор
        public Square(double side, string name, string color) : base(name, color)
        {
            Side = side;
        }

        // Метод
        public override string ToString()
        {
            return $"Фігура створена: id:{Id}, Ім'я: {Name} , Кольор: {Color}, Сторона: {Side}";
        }

        // Метод
        public override double CalculateArea()
        {
            return Area = Side * Side;
        }

        // Метод
        public override double CalculatePerimetr()
        {
            return Perimeter = 4 * Side;
        }

        // Метод
        public override void Draw()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Console.Write("#");
                }
                Console.WriteLine();
            }
        }
    }
}

