﻿using System;
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


                List<fuenteUsuario> list = new List<fuenteUsuario>();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        fuenteUsuario f = new fuenteUsuario()
                        {
                            nombre = rdr["FUENTE"].ToString(),
                            //nombre_modelo = rdr["MODELO"].ToString(),
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
        public void getEncuestas()
        {
            try
            {
                cnx = new cnx();
                rdr = cnx.ExecuteCommand("SELECT * FROM TI_FUENTE", CommandType.Text);


                List<fuentes> list = new List<fuentes>();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        fuentes f = new fuentes()
                        {
                            nombre = rdr["NOMBRE"].ToString(),
                            id_fuente = rdr["ID_FUENTE"].ToString(),
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
        public string getCampos(string model)
        {
            try
            {
                cnx = new cnx();
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter() { ParameterName = "@modelo", Value = model };
                rdr = cnx.ExecuteCommand("SELECT tc.ID_CAMPO,NOMBRE from TC_CAMPO tc INNER JOIN TR_TABLA tr ON tc.ID_CAMPO=tr.ID_CAMPO and tr.ID_MODELO=@modelo", CommandType.Text, parameters);

                List<campo> list = new List<campo>();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        campo c = new campo()
                        {
                            nombre = rdr["NOMBRE"].ToString(),
                            id_campo = rdr["ID_CAMPO"].ToString()
                        };
                        list.Add(c);
                    }
                    rdr.Close();
                    rdr = null;
                    string data = JsonConvert.SerializeObject(list);
                    //Context.Response.Write(data);
                    return data;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return "";
        }

        [WebMethod]
        public string updateUsuario(string id,string nombre, string apellido_p, string apellido_m,string contrasena,string correo, Object[] fuentes)
        {
            try
            {
                cnx = new cnx();
                SqlParameter[] parameters = new SqlParameter[6];
                parameters[0] = new SqlParameter() { ParameterName = "@nombre", Value = nombre };
                parameters[1] = new SqlParameter() { ParameterName = "@apellido_m", Value = apellido_m };
                parameters[2] = new SqlParameter() { ParameterName = "@apellido_p", Value = apellido_p };
                parameters[3] = new SqlParameter() { ParameterName = "@contra", Value = contrasena };
                parameters[4] = new SqlParameter() { ParameterName = "@correo", Value = correo };
                parameters[5] = new SqlParameter() { ParameterName = "@id", Value = id };
                rdr = cnx.ExecuteCommand("UPDATE TC_USUARIO SET NOMBRE=@nombre,APELLIDO_M=@apellido_m,APELLIDO_P=@apellido_p,CORREO=@correo,PASSWORD=@contra WHERE ID_USUARIO=@id ", CommandType.Text, parameters);
                    string datos=JsonConvert.SerializeObject(fuentes);
                    SqlDataReader r2;
                    cnx cnxDel = new cnx();
                    SqlParameter[] parametro = new SqlParameter[1];
                    parametro[0] = new SqlParameter() { ParameterName = "@id", Value = id };
                    r2 = cnxDel.ExecuteCommand("DELETE TI_FUENTE_USUARIO WHERE ID_USUARIO=@id",CommandType.Text,parametro);
                    var fuentesSelect = JsonConvert.DeserializeObject<List<fuente>>(datos);
                    for (int i = 0; i < fuentesSelect.Count; i++)
                    {
                        SqlDataReader r;
                        cnx cnxAux=new cnx();
                        SqlParameter[] param = new SqlParameter[3];
                        param[0] = new SqlParameter() { ParameterName = "id_modelo", Value = fuentesSelect[i].id_modelo };
                        param[1] = new SqlParameter() { ParameterName = "id_fuente", Value = fuentesSelect[i].id };
                        param[2] = new SqlParameter() { ParameterName = "id_usuario", Value = id };
                        r = cnxAux.ExecuteCommand("INSERT INTO  TI_FUENTE_USUARIO (ID_MODELO,ID_FUENTE,ID_USUARIO) VALUES (@id_modelo,@id_fuente,@id_usuario)", CommandType.Text, param);
                    }
                    
                return "correcto";
                    
                    
            }
            catch (Exception ex)
            {

                throw ex;
            }
       
        }


        [WebMethod]
        public string createUsuario(string nombre, string apellido_p, string apellido_m, string contrasena, string correo, Object[] fuentes)
        {
            try
            {
                cnx = new cnx();
                cnx cnxAux = new cnx();
               
                SqlParameter[] parameters = new SqlParameter[5];
                parameters[0] = new SqlParameter() { ParameterName = "@nombre", Value = nombre };
                parameters[1] = new SqlParameter() { ParameterName = "@apellido_m", Value = apellido_m };
                parameters[2] = new SqlParameter() { ParameterName = "@apellido_p", Value = apellido_p };
                parameters[3] = new SqlParameter() { ParameterName = "@contra", Value = contrasena };
                parameters[4] = new SqlParameter() { ParameterName = "@correo", Value = correo };
                rdr = cnx.ExecuteCommand("INSERT INTO TC_USUARIO (CORREO,ID_PERFIL,NOMBRE,APELLIDO_M,APELLIDO_P,PASSWORD) VALUES(@correo,'2',@nombre,@apellido_m,@apellido_p,@contra)", CommandType.Text, parameters);
                string datos = JsonConvert.SerializeObject(fuentes);
                SqlDataReader r2;
                r2 = cnxAux.ExecuteCommand("SELECT MAX(ID_USUARIO) as ID from TC_USUARIO", CommandType.Text);
                if (r2!=null)
                {
                    while (r2.Read())
                    {
                               var fuentesSelect = JsonConvert.DeserializeObject<List<fuente>>(datos);
                    for (int i = 0; i < fuentesSelect.Count; i++)
                    {
                        cnx cnxAux2 = new cnx();
                        SqlDataReader r;
                        SqlParameter[] param = new SqlParameter[3];
                        param[0] = new SqlParameter() { ParameterName = "id_modelo", Value = fuentesSelect[i].id_modelo };
                        param[1] = new SqlParameter() { ParameterName = "id_fuente", Value = fuentesSelect[i].id };
                        param[2] = new SqlParameter() { ParameterName = "id_usuario", Value = r2["ID"].ToString() };
                        r = cnxAux2.ExecuteCommand("INSERT INTO TI_FUENTE_USUARIO (ID_MODELO,ID_FUENTE,ID_USUARIO) VALUES(@id_modelo,@id_fuente,@id_usuario)", CommandType.Text, param);
                    }
                    }
                  
                    
                   
                }
         

                return "correcto";


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
                
                cnx = new cnx();
                
                List<usuarios> usuariosList = new List<usuarios>();
                rdr = cnx.ExecuteCommand("SELECT * FROM TC_USUARIO WHERE ID_PERFIL=2",CommandType.Text);
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        SqlDataReader r;
                        cnx cnxAux = new cnx();
                        r = cnxAux.ExecuteCommand("SELECT fu.ID_MODELO,fu.ID_FUENTE,tf.NOMBRE FROM TI_FUENTE_USUARIO fu  inner join TI_fuente tf on fu.ID_FUENTE=tf.ID_FUENTE and fu.ID_MODELO=tf.ID_MODELO and fu.ID_USUARIO='" + rdr["ID_USUARIO"].ToString() + "'", CommandType.Text);
                        usuarios user = new usuarios()
                        {
                            id=rdr["ID_USUARIO"].ToString(),
                            nombre = rdr["NOMBRE"].ToString(),
                            apellido_p = rdr["APELLIDO_P"].ToString(),
                            apellido_m = rdr["APELLIDO_M"].ToString(),
                            correo = rdr["CORREO"].ToString(),
                            fecha_na = rdr["FECHA_NACIMIENTO"].ToString(),
                            contrasena = rdr["PASSWORD"].ToString()
                        };

                        
                        if (r!=null)
                        {
                            user.fuentes = new List<fuenteUsuario>();
                             while (r.Read())
                                {
                                    fuenteUsuario f = new fuenteUsuario()
                                    {
                                        id = r["ID_FUENTE"].ToString(),
                                        id_modelo = r["ID_MODELO"].ToString(),
                                        nombre=r["NOMBRE"].ToString()
                                        //id_usuario = r["ID_USUARIO"].ToString()
                                    };
                                    user.fuentes.Add(f);
                                }
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
    public string id { set; get; }
    public List<fuenteUsuario> fuentes { set; get; }

}
public class fuenteUsuario
{
    public string id { set; get; }
    public string id_modelo { set; get; }
    //public string id_usuario { set; get; }
    public string nombre { set; get; }

}
public class fuente
{
    public string nombre { set; get; }
    public string nombre_modelo { set; get; }
    public string id { set; get; }
    public string id_modelo { set; get; }

}
//encuestas
public class fuentes {
    public string nombre { set; get; }
    public string id_fuente { set; get; }
    public string id_modelo { set; get; }
}

public class campo {
    public string id_campo { set; get; }
    public string nombre { set; get; }
}
