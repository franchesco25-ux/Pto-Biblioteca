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
                using (SqlCommand cmd = new SqlCommand("usp_listar", cn))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            temporal.Add(new Recursos
                            {
                                id = dr.GetInt32(0),
                                titulo = dr.GetString(1),
                                subtitulo = dr.GetString(2),
                                ano_publicacion = dr.GetDateTime(3),
                                edicion = dr.GetString(4)
                            });
                        }
                    }
                }
            }

            return temporal;
        }
    }
}
