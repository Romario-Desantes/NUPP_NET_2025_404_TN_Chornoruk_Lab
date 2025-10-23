using FigureProj.Common.Models.Abstract;

namespace FigureProj.Common.Extensions
{
    public static class FigureExtensions
    {
        // Метод розширення
        public static string GetDetailedInfo(this Figure figure)
        {
            if (figure == null)
                throw new ArgumentNullException(nameof(figure));

            figure.CalculateArea();
            figure.CalculatePerimetr();

            return $@" ДЕТАЛЬНА ІНФОРМАЦІЯ ПРО ФІГУРУ:             
                         ID:         {figure.Id}                                      
                         Тип:        {figure.GetType().Name}                          
                         Назва:      {figure.Name}                                    
                         Колір:      {figure.Color}                                   
                         Площа:      {figure.Area:F2}                                 
                         Периметр:   {figure.Perimeter:F2}";
        }
    }
}


