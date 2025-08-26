using System.ComponentModel.DataAnnotations;
using System.Data;
using Biblioteca.Models;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Repositorios
{
    public class RecursosDAO : IRecursos
    {
        IConexion _conecta;

        public RecursosDAO(IConexion conecta)
        {
            _conecta = conecta;
        }

        public void deleteResources(int ? id)
        {
            using(SqlConnection cn = new SqlConnection(_conecta.getConexion()))
            {
                using(SqlCommand cmd = new SqlCommand("sp_eliminar", cn))
                {
                    cmd.CommandType=CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Recursos> ListResources()
        {
            List<Recursos> temporal = new List<Recursos>();

            using (SqlConnection cn = new SqlConnection(_conecta.getConexion()))
            {
                using (SqlCommand cmd = new SqlCommand("usp_listar_recursos", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            temporal.Add(new Recursos
                            {

                                id = dr.IsDBNull(0) ? 0 : dr.GetInt16(0), // Si es nulo, asigna 0. Sino, lee el valor.
                                titulo = dr.IsDBNull(1) ? string.Empty : dr.GetString(1), // Si es nulo, asigna un string vacío.
                                subtitulo = dr.IsDBNull(2) ? string.Empty : dr.GetString(2),
                                ano_publicacion = dr.IsDBNull(3) ? 0 : dr.GetInt16(3),
                                edicion = dr.IsDBNull(4) ? string.Empty : dr.GetString(4),
                                tipo_recurso = dr.IsDBNull(5) ? string.Empty : dr.GetString(5),
                            });
                        }
                    }
                }
            }

            return temporal;
        }
        public int insertResource(Recursos reg)
        {
            using var cn = new SqlConnection(_conecta.getConexion());
            using var cmd = new SqlCommand("dbo.sp_InsertarRecurso", cn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@pTitulo", reg.titulo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@pSubtitulo", reg.subtitulo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@pIsbnIssn", reg.isbn_issn ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@pTipoRecursoNombre", reg.tipo_recurso ?? (object)DBNull.Value); // por NOMBRE
            cmd.Parameters.AddWithValue("@pEditorialNombre", reg.editorial_nombre ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@pAnoPublicacion", (object?)reg.ano_publicacion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@pEdicion", reg.edicion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@pDescripcion", reg.descripcion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@pClasificacionDdc", reg.clasificacion_ddc ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@pGeneroNombre", reg.genero_nombre ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@pFechaIngreso", reg.fecha_ingreso);                  // obligatorio en SP
            cmd.Parameters.AddWithValue("@pEstadoGeneral", reg.estado_general ?? "activo");
            cmd.Parameters.AddWithValue("@pEsConsultaSala", reg.es_consulta_sala);
            cmd.Parameters.AddWithValue("@pObservaciones", reg.observaciones ?? (object)DBNull.Value);

            var ret = new SqlParameter("@RETURN_VALUE", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            cmd.Parameters.Add(ret);

            cn.Open();
            cmd.ExecuteNonQuery();

            return (int)(ret.Value ?? 0); // >0 = ID creado; 0 = error
        }

        public int asociarAutorARecurso(int recursoId, string autorNombre, string? autorApellido, string tipoAutor = "principal")
        {
            using var cn = new SqlConnection(_conecta.getConexion());
            using var cmd = new SqlCommand("dbo.sp_AsociarAutorARecurso", cn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@pRecursoId", recursoId);
            cmd.Parameters.AddWithValue("@pAutorNombre", autorNombre);
            cmd.Parameters.AddWithValue("@pAutorApellido", (object?)autorApellido ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@pTipoAutor", tipoAutor);

            var ret = new SqlParameter("@RETURN_VALUE", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
            cmd.Parameters.Add(ret);

            cn.Open();
            cmd.ExecuteNonQuery();

            return (int)(ret.Value ?? 0); // >0 = relación creada; 0 = error
        }

    }
}
