using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class Recursos
    {
        public int id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? titulo { get; set; }
        public string? subtitulo { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        //[DataType(DataType.Date)]public DateTime ano_publicacion { get; set; }

        public int? ano_publicacion { get; set; }

        public string? edicion { get; set; }

        public bool eliminado { get; set; }

        public string? tipo_recurso { get; set; }

    }
}
