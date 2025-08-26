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








    }
}
