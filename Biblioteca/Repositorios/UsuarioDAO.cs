using Biblioteca.Models;
using Microsoft.Data.SqlClient;
using System.Data;
namespace Biblioteca.Repositorios
{
    public class UsuarioDAO : IRepositorioUsuario, IUsuarioRepository<Usuario>
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

        //=============Implementación by Jesus============

        private static bool HasColumn(IDataRecord dr, string name)
        {
            for (int i = 0; i < dr.FieldCount; i++)
                if (string.Equals(dr.GetName(i), name, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private static T GetValue<T>(IDataRecord dr, string name, T def = default!)
        {
            if (!HasColumn(dr, name)) return def;
            var v = dr[name];
            if (v == DBNull.Value || v is null) return def;
            return (T)Convert.ChangeType(v, typeof(T));
        }

        // Tus SP devuelven nombre_completo; derivamos Nombre/Apellido
        private static (string Nombre, string Apellido) SplitNombreCompleto(string? nc)
        {
            if (string.IsNullOrWhiteSpace(nc)) return (string.Empty, string.Empty);
            var s = nc.Trim();
            var p = s.LastIndexOf(' ');
            if (p <= 0) return (s, string.Empty);
            return (s[..p], s[(p + 1)..]);
        }

        // Mapper compatible con lo que devuelven tus SP de lectura
        private Usuario MapearDesdeSpLectura(IDataRecord dr)
        {
            var nc = GetValue<string>(dr, "nombre_completo", string.Empty);
            var (nombre, apellido) = SplitNombreCompleto(nc);

            return new Usuario
            {
                UsuarioId = GetValue<short>(dr, "usuario_id"),
                Rol = GetValue<string>(dr, "rol", string.Empty),
                Identidad = GetValue<string>(dr, "identidad", string.Empty),
                DocumentoIdent = GetValue<string>(dr, "documento_ident", string.Empty),
                Nombre = nombre,
                Apellido = apellido,
                UsuarioLogin = GetValue<string>(dr, "usuario", string.Empty),

                // Estos campos NO vienen en tus SP de lectura → default
                PasswordHash = string.Empty,
                Telefono = GetValue<string>(dr, "telefono", null),
                Direccion = GetValue<string>(dr, "direccion", null),
                Mail = GetValue<string>(dr, "mail", null),
                FechaRegistro = GetValue<DateTime>(dr, "fecha_registro", DateTime.MinValue),
                PasswordActiva = false,
                CuentaBloqueada = false,
                Eliminado = false
            };
        }
        public short crear(Usuario u)
        {
            using SqlConnection cn = new SqlConnection(_Conexion.getConexion());
            using var cmd = new SqlCommand("sp_CrearUsuario", cn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@rol", u.Rol);
            cmd.Parameters.AddWithValue("@identidad", u.Identidad);
            cmd.Parameters.AddWithValue("@documento_ident", u.DocumentoIdent);
            cmd.Parameters.AddWithValue("@nombre", u.Nombre);
            cmd.Parameters.AddWithValue("@apellido", u.Apellido);
            cmd.Parameters.AddWithValue("@usuario", u.UsuarioLogin);
            cmd.Parameters.AddWithValue("@password", u.PasswordHash); // hash desde app
            cmd.Parameters.AddWithValue("@telefono", (object?)u.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", (object?)u.Direccion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mail", (object?)u.Mail ?? DBNull.Value);

            var pOut = new SqlParameter("@nuevo_usuario_id", SqlDbType.SmallInt) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pOut);

            cn.Open();
            cmd.ExecuteNonQuery();

            return pOut.Value is short id ? id : (short)0;
        }

        public int actualizar(Usuario u)
        {
            using SqlConnection cn = new SqlConnection(_Conexion.getConexion());
            using var cmd = new SqlCommand("sp_EditarUsuario", cn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@usuario_id", u.UsuarioId);

            cmd.Parameters.AddWithValue("@rol", DBNull.Value);
            cmd.Parameters.AddWithValue("@identidad", DBNull.Value);
            cmd.Parameters.AddWithValue("@documento_ident", DBNull.Value);

            cmd.Parameters.AddWithValue("@nombre", u.Nombre);
            cmd.Parameters.AddWithValue("@apellido", u.Apellido);
            cmd.Parameters.AddWithValue("@usuario", u.UsuarioLogin);

            cmd.Parameters.AddWithValue("@telefono", DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", DBNull.Value);
            cmd.Parameters.AddWithValue("@mail", DBNull.Value);

            cn.Open();
            cmd.ExecuteNonQuery(); // El SP hace SELECT de StatusCode; no devuelve rows affected
            return 1; // consideramos éxito si no lanza excepción
        }

        public int eliminarLogico(short id)
        {
            using SqlConnection cn = new SqlConnection(_Conexion.getConexion());
            using var cmd = new SqlCommand("sp_EliminarUsuario", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@usuario_id", id);

            cn.Open();
            cmd.ExecuteNonQuery(); // SP hace UPDATE y luego SELECT (StatusCode)
            return 1;
        }

        public Usuario? buscarPorId(short id)
        {
            using SqlConnection cn = new SqlConnection(_Conexion.getConexion());
            using var cmd = new SqlCommand("sp_obtener_usuario_por_id", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@usuario_id", id);

            cn.Open();
            using var dr = cmd.ExecuteReader();
            if (dr.Read())
                return MapearDesdeSpLectura(dr);
            // Si no hay fila, ignoramos el 2º resultset (StatusCode/Mensaje)
            return null;
        }

        public IEnumerable<Usuario> listarVigentes()
        {
            var lista = new List<Usuario>();
            using SqlConnection cn = new SqlConnection(_Conexion.getConexion());
            using var cmd = new SqlCommand("sp_listar_usuarios_vigentes", cn) { CommandType = CommandType.StoredProcedure };
            cn.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                lista.Add(MapearDesdeSpLectura(dr));
            return lista;
        }

        public int bloquear(short id, bool bloquear)
        {
            using SqlConnection cn = new SqlConnection(_Conexion.getConexion());
            using var cmd = new SqlCommand("sp_BloquearUsuario", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@usuario_id", id);
            cmd.Parameters.AddWithValue("@bloquear", bloquear);

            cn.Open();
            cmd.ExecuteNonQuery(); // SP hace UPDATE y luego SELECT (StatusCode)
            return 1;
        }
    }
}
