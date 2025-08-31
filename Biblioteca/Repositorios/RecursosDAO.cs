using Biblioteca.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Text.Json;

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
                using (SqlCommand cmd = new SqlCommand("sp_listar_recursos", cn))
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
                                descripcion_recurso = dr.IsDBNull(6) ? string.Empty : dr.GetString(6),
                                isbn_issn = dr.IsDBNull(7) ? string.Empty : dr.GetString(7),
                                descripcion = dr.IsDBNull(8) ? string.Empty : dr.GetString(8),
                                clasificacion_ddc = dr.IsDBNull(9) ? string.Empty : dr.GetString(9),
                                fecha_ingreso = dr.IsDBNull(10) ? DateTime.MinValue : dr.GetDateTime(10),
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
            using var cmd = new SqlCommand("dbo.sp_CrearRecurso", cn) { 
                CommandType = CommandType.StoredProcedure 
            };

            cmd.Parameters.AddWithValue("@titulo", reg.titulo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@subtitulo", reg.subtitulo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@isbn_issn", reg.isbn_issn ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@tipo_recurso_id", reg.tipo_recurso ?? (object)DBNull.Value); // por NOMBRE
            cmd.Parameters.AddWithValue("@editorial_id", reg.editorial_nombre ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ano_publicacion", (object?)reg.ano_publicacion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@edicion", reg.edicion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@descripcion", reg.descripcion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@clasificacion_ddc", reg.clasificacion_ddc ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@genero_id", reg.genero_nombre ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@fecha_ingreso", reg.fecha_ingreso);                  // obligatorio en SP
            cmd.Parameters.AddWithValue("@estado_general", reg.estado_general ?? "activo");
            cmd.Parameters.AddWithValue("@es_consulta_sala", reg.es_consulta_sala);
            cmd.Parameters.AddWithValue("@observaciones", reg.observaciones ?? (object)DBNull.Value);

            var ret = new SqlParameter("@nuevo_recurso_id", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
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

        public void generarMulta()
        {
            using(SqlConnection cn = new SqlConnection(_conecta.getConexion()))
            {
                using(SqlCommand cmd = new SqlCommand("sp_generar_multas_retraso", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public ResponseJSON crearRecurso(Recursos recursos)
        {
            using (SqlConnection cn = new SqlConnection(_conecta.getConexion()))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand("sp_CrearRecurso", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros de entrada
                    cmd.Parameters.AddWithValue("@titulo", recursos.titulo);
                    cmd.Parameters.AddWithValue("@subtitulo", (object)recursos.subtitulo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@isbn_issn", (object)recursos.isbn_issn ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@tipo_recurso_id", recursos.tiporecursoid);
                    cmd.Parameters.AddWithValue("@editorial_id", (object)recursos.editorialid ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ano_publicacion", (object)recursos.ano_publicacion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@edicion", (object)recursos.edicion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@descripcion", (object)recursos.descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@clasificacion_ddc", (object)recursos.clasificacion_ddc ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@genero_id", (object)recursos.generoid ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@fecha_ingreso", recursos.fecha_ingreso);
                    cmd.Parameters.AddWithValue("@estado_general", (object)recursos.estado_general ?? "activo");
                    cmd.Parameters.AddWithValue("@es_consulta_sala", recursos.es_consulta_sala);
                    cmd.Parameters.AddWithValue("@observaciones", (object)recursos.observaciones ?? DBNull.Value);

                    SqlParameter nuevoRecursoIdParam = new SqlParameter
                    {
                        ParameterName = "@nuevo_recurso_id",
                        SqlDbType = SqlDbType.SmallInt,
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(nuevoRecursoIdParam);

                    string jsonResponse;

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            jsonResponse = reader.GetString(0);
                        }
                        else
                        {
                            // Manejo de error si no se recibe respuesta
                            return new ResponseJSON { StatusCode = 500, Mensaje = "No se recibió respuesta del stored procedure." };
                        }
                    }

                    var resultado = JsonSerializer.Deserialize<ResponseJSON>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return resultado;
                }
            }
        }

        public IEnumerable<SelectView> listTipoRecursos()
        {
            List<SelectView> temporal = new List<SelectView>();

            using (SqlConnection cn = new SqlConnection(_conecta.getConexion()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_listar_tipo_recursos", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            temporal.Add(new SelectView
                            {

                                id = dr.IsDBNull(0) ? 0 : dr.GetInt16(0), // Si es nulo, asigna 0. Sino, lee el valor.
                                nombre = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
                            });
                        }
                    }
                }
            }

            return temporal;
        }

        public IEnumerable<SelectView> listEditoriales()
        {
            List<SelectView> temporal = new List<SelectView>();

            using (SqlConnection cn = new SqlConnection(_conecta.getConexion()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_listar_editoriales", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            temporal.Add(new SelectView
                            {

                                id = dr.IsDBNull(0) ? 0 : dr.GetInt16(0), // Si es nulo, asigna 0. Sino, lee el valor.
                                nombre = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
                            });
                        }
                    }
                }
            }

            return temporal;
        }

        public IEnumerable<SelectView> listGeneros()
        {
            List<SelectView> temporal = new List<SelectView>();

            using (SqlConnection cn = new SqlConnection(_conecta.getConexion()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_listar_generos", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            temporal.Add(new SelectView
                            {

                                id = dr.IsDBNull(0) ? 0 : dr.GetInt16(0), // Si es nulo, asigna 0. Sino, lee el valor.
                                nombre = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
                            });
                        }
                    }
                }
            }

            return temporal;
        }

        public ResponseJSON editarRecurso(Recursos recursos)
        {
            using (SqlConnection cn = new SqlConnection(_conecta.getConexion()))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand("sp_EditarRecurso", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros de entrada
                    cmd.Parameters.AddWithValue("@recurso_id", recursos.id);
                    cmd.Parameters.AddWithValue("@titulo", recursos.titulo);
                    cmd.Parameters.AddWithValue("@subtitulo", (object)recursos.subtitulo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@isbn_issn", (object)recursos.isbn_issn ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@tipo_recurso_id", recursos.tiporecursoid);
                    cmd.Parameters.AddWithValue("@editorial_id", (object)recursos.editorialid ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ano_publicacion", (object)recursos.ano_publicacion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@edicion", (object)recursos.edicion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@descripcion", (object)recursos.descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@clasificacion_ddc", (object)recursos.clasificacion_ddc ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@genero_id", (object)recursos.generoid ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@fecha_ingreso", recursos.fecha_ingreso);
                    cmd.Parameters.AddWithValue("@estado_general", (object)recursos.estado_general ?? "activo");
                    cmd.Parameters.AddWithValue("@es_consulta_sala", recursos.es_consulta_sala);
                    cmd.Parameters.AddWithValue("@observaciones", (object)recursos.observaciones ?? DBNull.Value);

                    string jsonResponse;

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            jsonResponse = reader.GetString(0);
                        }
                        else
                        {
                            // Manejo de error si no se recibe respuesta
                            return new ResponseJSON { StatusCode = 500, Mensaje = "No se recibió respuesta del stored procedure." };
                        }
                    }

                    var resultado = JsonSerializer.Deserialize<ResponseJSON>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return resultado;
                }
            }
        }
    }
}
