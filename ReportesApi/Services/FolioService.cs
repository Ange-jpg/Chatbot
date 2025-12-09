using System.Text.RegularExpressions;

namespace Backend.Services
{
    public class FolioService
    {
        public string GenerarFolio(string categoria, string subcategoria)
        {
            // Primeras 4 letras de la categoría
            string cat4 = categoria.Length >= 4
                ? categoria.Substring(0, 4).ToUpper()
                : categoria.ToUpper().PadRight(4, 'X');

            // Extraer solo el número de la subcategoría
            string subNum = ExtraerNumeroSubcategoria(subcategoria);

            // Fecha ddMMyy (UTC recomendado)
            string fecha = DateTime.UtcNow.ToString("ddMMyy");

            return $"{cat4}{subNum}{fecha}";
        }

        private string ExtraerNumeroSubcategoria(string subcategoria)
        {
            
            var match = Regex.Match(subcategoria, @"^\s*(\d+)");
            return match.Success ? match.Groups[1].Value : "0";
        }
    }
}
