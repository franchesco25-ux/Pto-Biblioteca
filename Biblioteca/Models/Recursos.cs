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

        public int? tiporecursoid { get; set; }
        public int? editorialid { get; set; }
        public int? generoid { get; set; }

        public int? ano_publicacion { get; set; }

        public string? edicion { get; set; }

        public bool eliminado { get; set; }

        public string? tipo_recurso { get; set; }

        [StringLength(20)]
        public string? isbn_issn { get; set; }
        
        [StringLength(255)]
        public string? editorial_nombre { get; set; }

        public string? descripcion { get; set; }

        [StringLength(20)]
        public string? clasificacion_ddc { get; set; }

        [StringLength(100)]
        public string? genero_nombre { get; set; }

        // @pFechaIngreso -> columna fecha_ingreso (obligatoria en el SP)
        // Se deja default hoy para no romper formularios existentes.
        [DataType(DataType.Date)]
        public DateTime fecha_ingreso { get; set; } = DateTime.Today;

        // @pEstadoGeneral -> columna estado_general (default 'activo')
        [StringLength(20)]
        public string? estado_general { get; set; } = "activo";

        public bool es_consulta_sala { get; set; } = false;

       
        public string? observaciones { get; set; }

        public string? descripcion_recurso { get; set; }
    }
}
