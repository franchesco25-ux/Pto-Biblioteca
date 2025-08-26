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
                                descripcion_recurso = dr.IsDBNull(6) ? string.Empty : dr.GetString(6),
                            });
                        }
                    }
                }
            }

            return temporal;
        }
    }
}
