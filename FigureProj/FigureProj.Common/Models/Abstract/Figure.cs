namespace FigureProj.Common.Models.Abstract
{
    public abstract class Figure
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public double Area { get; protected set; }
        public double Perimeter { get; protected set; }

        public static int CounterOfFigures = 0;
        public static string DefaultColor;
        public static Dictionary<string, string> PresetColors;

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

        public abstract double CalculateArea();
        public abstract double CalculatePerimetr();
        public abstract void Draw();
    }
}
