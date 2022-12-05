using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EM.Repository
{

    public static class ConexaoFirebird
    {

        static FbConnection _conexaoFB;


        public static FbConnection conexao
        {
            get
            {
                return _conexaoFB;
            }
        }

        public static bool Conectar()
        {
            string sql = "User=SYSDBA;" +
                                  "Password=masterkey;" +
                                  "Database=C:\\Users\\luisp\\OneDrive\\Documentos\\BancoDados\\BANCOESTUDO.FDB;" +
                                  "DataSource=localhost;" +
                                  "Port=3050;" +
                                  "Dialect=3;" +
                                  "Charset=NONE;" +
                                  "Role=;" +
                                  "Connection lifetime=15;" +
                                  "Pooling=true;" +
                                  "MinPoolSize=0;" +
                                  "MaxPoolSize=50;" +
                                  "Packet Size=4096;" +
                                  "ServerType = 0;"; 

            _conexaoFB = new FbConnection(sql);
            _conexaoFB.Open();


            return true;
        }

        public static bool Desconectar()
        {
            _conexaoFB.Close();
            return false;
        }
    }
















    //public class ConexaoFirebird
    //{
    //    FbConnection fb = new FbConnection();

    //    public ConexaoFirebird()
    //    {
    //        fb.ConnectionString = "User=SYSDBA;" +
    //                              "Password=masterkey;" +
    //                              "Database=C:\\Users\\luisp\\OneDrive\\Documentos\\BancoDados\\BANCOESTUDO.FDB;" +
    //                              "DataSource=localhost;" +
    //                              "Port=3050;" +
    //                              "Dialect=3;" +
    //                              "Charset=NONE;" +
    //                              "Role=;" +
    //                              "Connection lifetime=15;" +
    //                              "Pooling=true;" +
    //                              "MinPoolSize=0;" +
    //                              "MaxPoolSize=50;" +
    //                              "Packet Size=4096;" +
    //                              "ServerType = 0;";
    //    }

    //    public FbConnection Conectar()
    //    {
    //        if (fb.State == System.Data.ConnectionState.Closed)
    //        {
    //            fb.Open();
    //        }
    //        return fb;
    //    }

    //    public void desconectar()
    //    {
    //        if (fb.State == System.Data.ConnectionState.Open)
    //        {
    //            fb.Close();
    //        }
    //    }

    //}
}
