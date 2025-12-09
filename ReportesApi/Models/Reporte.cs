using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    [Table("reportes")] 
    public class Reporte
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("folio")]
        public string Folio { get; set; } = "";

        [Column("categoria")]
        public string Categoria { get; set; } = "";

        [Column("subcategoria")]
        public string Subcategoria { get; set; } = "";

        [Column("telefono")]
        public string Telefono { get; set; } = "";

        [Column("correo")]
        public string Correo { get; set; } = "";

        [Column("direccion")]
        public string Direccion { get; set; } = "";

        [Column("referencias")]
        public string Referencias { get; set; } = "";

        [Column("descripcion_reporte")]
        public string Descripcion_Reporte { get; set; } = "";

        [Column("fecha_creacion")]
        public DateTime Fecha_Creacion { get; set; } = DateTime.UtcNow;

        [Column("ruta_imagen")]

        [JsonPropertyName("rutaImagen")]
        public string Ruta_Imagen { get; set; } = "";

    }
}
