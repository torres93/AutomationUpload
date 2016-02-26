using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AutomationUpload
{
    /// <summary>
    /// Descripción breve de wsApp
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class wsApp : System.Web.Services.WebService
    {
        cnx cnx;
        SqlDataReader rdr;

        [WebMethod]
        public string login(string username, string password)
        {
            try
            {
                cnx = new cnx();
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter() { ParameterName = "@username", Value = username };
                rdr = cnx.ExecuteCommand("SELECT * FROM TC_USUARIO WHERE CORREO=@username", CommandType.Text, parameters);
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        if (username == rdr["CORREO"].ToString() && password == rdr["PASSWORD"].ToString())
                        {
                            if (rdr["ID_PERFIL"].ToString() == "1")
                            {
                                return "admin";
                            }
                            else
                            {
                                return "userNormal";
                            }

                        }
                        else
                        {
                            return "errorUsername";
                        }
                    }
                    rdr.Close();
                    rdr = null;
                }
                else
                {
                    return "errorUsername";
                }
            }
            catch (Exception)
            {

                throw;
            }
            return "";
        }


        [WebMethod]
        public void getFuentes()
        {
            try
            {
                cnx = new cnx();
                rdr = cnx.ExecuteCommand("SELECT TC_MODELO.ID_MODELO, TC_MODELO.NOMBRE AS MODELO, TI_FUENTE.ID_FUENTE, TI_FUENTE.NOMBRE AS FUENTE FROM TC_MODELO INNER JOIN TI_FUENTE  ON TC_MODELO.ID_MODELO = TI_FUENTE.ID_MODELO ORDER BY TC_MODELO.ID_MODELO, TI_FUENTE.ID_FUENTE", CommandType.Text);


                List<fuente> list = new List<fuente>();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        fuente f = new fuente()
                        {
                            nombre = rdr["FUENTE"].ToString(),
                            nombre_modelo = rdr["MODELO"].ToString(),
                            id = rdr["ID_FUENTE"].ToString(),
                            id_modelo = rdr["ID_MODELO"].ToString(),
                        };
                        list.Add(f);
                    }
                    rdr.Close();
                    rdr = null;
                    string data = JsonConvert.SerializeObject(list);
                    Context.Response.Write(data);
                    //return data;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [WebMethod]
        public void getUsuarios() {
            try
            {
                SqlDataReader r;
                cnx = new cnx();
                cnx cnxAux = new cnx();
                List<usuarios> usuariosList = new List<usuarios>();
                rdr = cnx.ExecuteCommand("SELECT * FROM TC_USUARIO WHERE ID_PERFIL=2",CommandType.Text);
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        r = cnxAux.ExecuteCommand("SELECT * FROM TI_FUENTE_USUARIO WHERE CORREO='" + rdr["CORREO"].ToString() + "'", CommandType.Text);
                        usuarios user = new usuarios()
                        {
                            nombre = rdr["NOMBRE"].ToString(),
                            apellido_p = rdr["APELLIDO_P"].ToString(),
                            apellido_m = rdr["APELLIDO_M"].ToString(),
                            correo = rdr["CORREO"].ToString(),
                            fecha_na = rdr["FECHA_NACIMIENTO"].ToString(),
                            contrasena = rdr["PASSWORD"].ToString()
                        };

                        user.fuentes = new List<fuenteUsuario>();
                        while (r.Read())
                        {
                            fuenteUsuario f = new fuenteUsuario()
                            {
                                id = r["ID_FUENTE"].ToString(),
                                id_modelo = r["ID_MODELO"].ToString(),
                                correo = r["CORREO"].ToString()
                            };
                            user.fuentes.Add(f);
                        }

                        usuariosList.Add(user);
                    }
                    rdr.Close();
                    rdr = null;
                    string data = JsonConvert.SerializeObject(usuariosList);
                    Context.Response.Write(data);
                    //return data;
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

        }
    }
}

public class usuarios
{
    public string nombre { set; get; }
    public string apellido_p { set; get; }
    public string apellido_m { set; get; }
    public string correo { set; get; }
    public string fecha_na { set; get; }
    public string contrasena { set; get; }
    public List<fuenteUsuario> fuentes { set; get; }

}
public class fuenteUsuario
{
    public string id { set; get; }
    public string id_modelo { set; get; }
    public string correo { set; get; }

}
public class fuente
{
    public string nombre { set; get; }
    public string nombre_modelo { set; get; }
    public string id { set; get; }
    public string id_modelo { set; get; }

}
