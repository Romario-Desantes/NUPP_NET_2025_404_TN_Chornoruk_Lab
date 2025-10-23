namespace FigureProj.Common.Models.Abstract
{
    // Делегат
    public delegate void FigureEventHandler(Figure figure, string message);

    public abstract class Figure
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public double Area { get; protected set; }
        public double Perimeter { get; protected set; }

        // Подія
        public event FigureEventHandler? OnPropertyChanged;

        // Статичне поле
        public static int CounterOfFigures = 0;
        // Статичне поле
        public static string DefaultColor;
        // Статичне поле
        public static Dictionary<string, string> PresetColors;

        // Статичний конструктор
        static Figure()
        {
            DefaultColor = "Зелений";
            PresetColors = new Dictionary<string, string>
            {
                { "чорний", "#000000" },
                { "фіолетовий", "#800080" },
                { "синій", "#0000FF" },
                { "жовтий", "#FFFF00" },
                { "червоний", "#FF0000" },
                { "білий", "#FFFFFF" },
                { "коричневий", "#8B4513" }
            };

        }

        // Конструктор
        public Figure(string name, string color)
        {
            Id = Guid.NewGuid();
            Name = name;

            if (!string.IsNullOrWhiteSpace(color) && PresetColors.ContainsKey(color.Trim().ToLowerInvariant()))
                Color = color.Trim();
            else
                Color = DefaultColor;

            CounterOfFigures++;
        }

        // Метод для виклику події
        protected void RaisePropertyChanged(string message)
        {
            OnPropertyChanged?.Invoke(this, message);
        }

        // Метод
        public abstract double CalculateArea();
        // Метод
        public abstract double CalculatePerimetr();
        // Метод
        public abstract void Draw();
    }
}
