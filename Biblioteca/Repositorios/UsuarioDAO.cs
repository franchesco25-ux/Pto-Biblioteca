using Biblioteca.Models;
using Microsoft.Data.SqlClient;
using System.Data;
namespace Biblioteca.Repositorios
{
    public class UsuarioDAO : IRepositorioUsuario
    {
        private readonly IConexion _Conexion;

        public UsuarioDAO(IConexion conexion)
        {
            _Conexion = conexion;
        }

        public async Task<Usuario?> ObtenerPorUsuarioActivoAsync(string usuarioLogin)
        {
            Usuario usuario = null;
            
            using (SqlConnection cn = new SqlConnection(_Conexion.getConexion()))
            {
                
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerUsuarioActivo", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@usuario", usuarioLogin);

                    await cn.OpenAsync();

                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (!await dr.ReadAsync())
                        {
                            return null;
                        }

                        usuario = new Usuario()
                        {
                            UsuarioId = dr.GetInt16(dr.GetOrdinal("usuario_id")),
                            Rol = dr.GetString(dr.GetOrdinal("rol")),
                            Identidad = dr.GetString(dr.GetOrdinal("identidad")),
                            DocumentoIdent = dr.GetString(dr.GetOrdinal("documento_ident")),
                            Nombre = dr.GetString(dr.GetOrdinal("nombre")),
                            Apellido = dr.GetString(dr.GetOrdinal("apellido")),
                            UsuarioLogin = dr.GetString(dr.GetOrdinal("usuario")),
                            PasswordHash = dr.GetString(dr.GetOrdinal("password")),
                            Telefono = dr.IsDBNull(dr.GetOrdinal("telefono")) ? null : dr.GetString(dr.GetOrdinal("telefono")),
                            Direccion = dr.IsDBNull(dr.GetOrdinal("direccion")) ? null : dr.GetString(dr.GetOrdinal("direccion")),
                            Mail = dr.IsDBNull(dr.GetOrdinal("mail")) ? null : dr.GetString(dr.GetOrdinal("mail")),
                            FechaRegistro = dr.GetDateTime(dr.GetOrdinal("fecha_registro")),
                            PasswordActiva = dr.GetBoolean(dr.GetOrdinal("password_activa")),
                            CuentaBloqueada = dr.GetBoolean(dr.GetOrdinal("cuenta_bloqueada")),
                            Eliminado = dr.GetBoolean(dr.GetOrdinal("eliminado"))
                        };
                    }
                }
            }
            return usuario;
        }



        public IEnumerable<Usuario> ListUsuarios()
        {
            List<Usuario> temporal = new List<Usuario>();

            using (SqlConnection cn = new SqlConnection(_Conexion.getConexion()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_listar_usuarios", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            temporal.Add(new Usuario
                            {

                                //id = dr.IsDBNull(0) ? 0 : dr.GetInt16(0), // Si es nulo, asigna 0. Sino, lee el valor.
                                //titulo = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),

                                UsuarioId = (short)(dr.IsDBNull(0) ? 0 : dr.GetInt16(0)),
                                Rol = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
                                Identidad = dr.IsDBNull(2) ? string.Empty : dr.GetString(2),
                                DocumentoIdent = dr.IsDBNull(3) ? string.Empty : dr.GetString(3),
                                Nombre = dr.IsDBNull(4) ? string.Empty : dr.GetString(4),
                                Apellido = dr.IsDBNull(5) ? string.Empty : dr.GetString(5),
                                UsuarioLogin = dr.IsDBNull(6) ? string.Empty : dr.GetString(6),
                                Telefono = dr.IsDBNull(7) ? string.Empty : dr.GetString(7),
                                Direccion = dr.IsDBNull(8) ? string.Empty : dr.GetString(8),
                                Mail = dr.IsDBNull(9) ? string.Empty : dr.GetString(9),
                                FechaRegistro = dr.IsDBNull(10) ? DateTime.MinValue : dr.GetDateTime(10),
                                PasswordActiva = dr.IsDBNull(11) ? false : dr.GetBoolean(11),

                            });
                        }
                    }
                }
            }

            return temporal;
        }




    }
}
