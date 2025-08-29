namespace Biblioteca.Repositorios
{
    public class ConexionDAO : IConexion
    {

        readonly IConfiguration _iconfig;


        public ConexionDAO(IConfiguration iconfig)
        {
            _iconfig = iconfig;
        }

        public string getConexion()
        {
            return _iconfig.GetConnectionString("cadena2");
        }
    }


}
