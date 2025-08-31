using Biblioteca.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Biblioteca.Repositorios
{
    public class ReservaDAO : IReservas
    {
        IConexion _conecta;
        public ReservaDAO(IConexion conecta)
        {
            _conecta = conecta;
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
    }
}
