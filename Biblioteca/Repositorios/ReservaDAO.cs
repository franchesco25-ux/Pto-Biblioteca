using Biblioteca.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace Biblioteca.Repositorios
{
    public class ReservaDAO : IReservas
    {
        IConexion _conecta;
        public ReservaDAO(IConexion conecta)
        {
            _conecta = conecta;
        }

        public IEnumerable<SelectView> listaRecursos()
        {
            List<SelectView> temporal = new List<SelectView>();

            using (SqlConnection cn = new SqlConnection(_conecta.getConexion()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_listar_select_recursos", cn))
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

        public IEnumerable<Reserva> listarReservas(int id)
        {
            List<Reserva> temporal = new List<Reserva>();

            using (SqlConnection cn = new SqlConnection(_conecta.getConexion()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_listar_reservas", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    if (id != 0)
                        cmd.Parameters.AddWithValue("@id_usuario", id);
                    else
                        cmd.Parameters.AddWithValue("@id_usuario", DBNull.Value);
                    
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            temporal.Add(new Reserva
                            {
                                id = dr.IsDBNull(0) ? 0 : dr.GetInt16(0),
                                recurso_titulo = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
                                UsuarioId = dr.IsDBNull(2) ? 0 : dr.GetInt16(2),
                                usuario_nombre = dr.IsDBNull(3) ? string.Empty : dr.GetString(3),
                                fechaReserva = dr.IsDBNull(4) ? DateTime.MinValue : dr.GetDateTime(4),
                                fechaExpiracion = dr.IsDBNull(5) ? DateOnly.MinValue : DateOnly.FromDateTime(dr.GetDateTime(5)),
                                estado = dr.IsDBNull(6) ? string.Empty : dr.GetString(6),

                            });
                        }
                    }
                }
            }

            return temporal;
        }

        public IEnumerable<SelectView> listaUsuarios()
        {
            List<SelectView> temporal = new List<SelectView>();

            using (SqlConnection cn = new SqlConnection(_conecta.getConexion()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_listar_select_clientes", cn))
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

        public ResponseJSON registrarReserva(Reserva reg)
        {
            ResponseJSON response = null;

            using (SqlConnection cn = new SqlConnection(_conecta.getConexion()))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("sp_registrar_reserva", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Añadir parámetros de entrada explícitamente para mayor seguridad y claridad
                        cmd.Parameters.Add("@recurso_id", SqlDbType.SmallInt).Value = reg.RecursoId;
                        cmd.Parameters.Add("@usuario_id", SqlDbType.SmallInt).Value = reg.UsuarioId;
                        cmd.Parameters.Add("@creado_por", SqlDbType.SmallInt).Value = reg.CreadoPor;
                        cmd.Parameters.Add("@fecha_expiracion", SqlDbType.Date).Value = reg.fechaExpiracion;

                        SqlParameter nuevoRecursoIdParam = new SqlParameter
                        {
                            ParameterName = "@nueva_reserva_id",
                            SqlDbType = SqlDbType.SmallInt,
                            Direction = ParameterDirection.Output
                        };

                        cmd.Parameters.Add(nuevoRecursoIdParam);


                        cn.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string jsonResponse = reader.GetString(0);
                                response = JsonSerializer.Deserialize<ResponseJSON>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            }
                            else
                            {
                                response = new ResponseJSON { StatusCode = 500, Mensaje = "No se recibió respuesta del stored procedure." };
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Manejo de errores específicos de SQL
                    response = new ResponseJSON { StatusCode = 500, Mensaje = $"Error de base de datos: {ex.Message}" };
                }
                catch (Exception ex)
                {
                    // Manejo de errores generales
                    response = new ResponseJSON { StatusCode = 500, Mensaje = $"Error inesperado: {ex.Message}" };
                }
            }

            return response;
        }
    }
}
