using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.OleDb;

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
        public string getEncuestas(string modelo)
        {
            string m = modelo;
            try
            {
                cnx = new cnx();
                rdr = cnx.ExecuteCommand("SELECT * FROM TI_FUENTE WHERE ID_MODELO ="+m, CommandType.Text);


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
      
                      //Context.Response.Write(data);
                    return data;
                }
                return "";
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        [WebMethod]
        public void getModelos()
        {
            try
            {
                cnx = new cnx();
                rdr = cnx.ExecuteCommand("SELECT * FROM TC_MODELO", CommandType.Text);


                List<modelo> list = new List<modelo>();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        modelo f = new modelo()
                        {
                            id_modelo = rdr["ID_MODELO"].ToString(),
                            nombre = rdr["NOMBRE"].ToString()
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
        public string getTable(string file)
        {
            string init="";
            if (file != "")
            {
                try
                {
                    OleDbConnectionStringBuilder csb = new OleDbConnectionStringBuilder();
                    csb["Provider"] = "Microsoft.ACE.OLEDB.12.0";
                    csb["Data Source"] = file;
                    csb["Extended Properties"] = "Excel 12.0 Xml";

                    using (OleDbConnection con = new System.Data.OleDb.OleDbConnection(csb.ToString()))
                    {
                        con.Open();

                        using(OleDbCommand olecmd = new OleDbCommand("SELECT * FROM [Hoja1$]", con))
                        {
                            OleDbDataReader rdr = olecmd.ExecuteReader();
                            int columns = rdr.FieldCount;
                             init = "[";
                            if(rdr.HasRows)
                            {

                                init += "{" + '"' + "data" + '"' + ":[";
                                for (int i = 0; i < columns; i++)
                                {
                                    init += '"' + rdr.GetName(i).ToString() + '"';
                                    if (i != columns - 1)
                                    {
                                        init += ",";
                                    }
                                }
                                init += "]},";


                                while(rdr.Read())
                                {
                                    init += "{"+'"'+"data"+'"'+":[";
                                    for(int i = 0 ;i<columns;i++)
                                    {
                                        init += '"' + rdr[i].ToString() + '"';
                                        if(i != columns-1)
                                        {
                                            init += ",";
                                        }
                                    }
                                    init += "]},";
                                }
                            }
                            init = init.Substring(0, init.Length - 1);
                            rdr.Close();
                            rdr.Dispose();
                            init+="]";
                        }

                    }

                    return init;
                }
                catch (Exception ex)
                {
                    return init;
                }

            }
            else
            {
                return init;
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
        public string getWorkTable(string id_modelo)
        {
            try
            {

                cnx = new cnx();

                List<campo> list = new List<campo>();
                rdr = cnx.ExecuteCommand("SELECT * FROM TI_ER WHERE ID_MODELO =  "+id_modelo+" AND TABLA_DE_TRABAJO = 'true'", CommandType.Text);
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        SqlDataReader r;
                        cnx cnxAux = new cnx();
                        r = cnxAux.ExecuteCommand("SELECT * FROM TR_TABLA WHERE ID_MODELO =" + id_modelo + " AND ID_CATALOGO = " + rdr["ID_CATALOGO"].ToString(), CommandType.Text);
                        if(r.HasRows)
                        {
                            while(r.Read())
                            {
                                campo f = new campo()
                                {
                                    id_campo = rdr["ID_CAMPO"].ToString(),
                                    tipo = rdr["TIPO"].ToString(),
                                    nombre = rdr["DESCRIPCION"].ToString()
                                };
                                list.Add(f);
                            }
                        }
                        r.Close();
                        r.Dispose();
                    }
                    rdr.Close();
                    rdr.Dispose();
                    string data = JsonConvert.SerializeObject(list);
                    return data;
                    //return data;
                }
            }
            catch (Exception ex)
            {               
                throw ex;
            }
            return "";
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

        [WebMethod]
        public int updateModeloCatalogo(string idmodelo, string[] idcatalogos)
        {
            try
            {

                DataTable dt = new DataTable();
                dt.Columns.Add("ID_MODELO");
                dt.Columns.Add("ID_CATALOGO");

                for (int i = 0; i < idcatalogos.Length; i++)
                {
                    DataRow row = dt.NewRow();
                    row["ID_MODELO"] = idmodelo;
                    row["ID_CATALOGO"] = idcatalogos[i];

                    dt.Rows.Add(row);
                }


                cnx = new cnx();
                SqlParameter[] parameters = new SqlParameter[2]{

                    new SqlParameter {ParameterName="@MODELO",Value=idmodelo},
                    new SqlParameter {ParameterName="@TABLE",Value=dt,SqlDbType=SqlDbType.Structured}
                };
                int rows = cnx.ExecuteTransaction("PR_UPDATE_MODELO_CATALOGO", CommandType.StoredProcedure, parameters);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return 0;
        }


        [WebMethod]
        public string getCatalogosModelo(string modelo)
        {
            try
            {
                cnx = new cnx();
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter() { ParameterName = "@modelo", Value = modelo };
                rdr = cnx.ExecuteCommand("SELECT ER.ID_CATALOGO,C.NOMBRE from TC_CATALOGO C INNER JOIN TI_ER ER ON C.ID_CATALOGO=ER.ID_CATALOGO WHERE ID_MODELO=@modelo", CommandType.Text, parameters);

                List<catalogo> list = new List<catalogo>();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        catalogo c = new catalogo()
                        {
                            nombre = rdr["NOMBRE"].ToString(),
                            id_catalogo = rdr["ID_CATALOGO"].ToString()
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
        public void getCatalogos()
        {
            try
            {
                cnx = new cnx();
                rdr = cnx.ExecuteCommand("SELECT ID_CATALOGO,NOMBRE from TC_CATALOGO", CommandType.Text);

                List<catalogo> list = new List<catalogo>();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        catalogo c = new catalogo()
                        {
                            nombre = rdr["NOMBRE"].ToString(),
                            id_catalogo = rdr["ID_CATALOGO"].ToString()
                        };
                        list.Add(c);
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
            //return "";
        }


        [WebMethod]
        public string getFuentesModelo(int modelo)
        {
            try
            {
                cnx = new cnx();
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter() { ParameterName = "@modelo", Value = modelo };

                rdr = cnx.ExecuteCommand("SELECT * FROM TI_FUENTE WHERE ID_MODELO=@modelo", CommandType.Text, parameters);


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
        public void insCampo(string nombre, int catalogo, string modelo, string fuente)
        {
            try
            {
                cnx = new cnx();
                SqlParameter[] parameters = new SqlParameter[3];
                parameters[0] = new SqlParameter() { ParameterName = "@modelo", Value = modelo };
                parameters[1] = new SqlParameter() { ParameterName = "@nombre", Value = nombre };
                parameters[2] = new SqlParameter() { ParameterName = "@fuente", Value = fuente };
                switch (catalogo)
                {
                    case 1:
                        rdr = cnx.ExecuteCommand("INSERT INTO TC_CAMPO (NOMBRE) VALUES (@nombre)", CommandType.Text, parameters);
                        break;
                    case 2:
                        rdr = cnx.ExecuteCommand("INSERT INTO TC_CATALOGO (NOMBRE) VALUES (@nombre)", CommandType.Text, parameters);
                        break;
                    case 3:
                        rdr = cnx.ExecuteCommand("INSERT INTO TI_FUENTE (NOMBRE,ID_MODELO,ID_FUENTE) VALUES (@nombre,@modelo,@fuente)", CommandType.Text, parameters);
                        break;
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
    public string tipo { set; get; }
}
public class modelo
{
    public string nombre { set; get; }
    public string id_modelo { set; get; }
}
public class catalogo
{
    public string id_catalogo { set; get; }
    public string nombre { set; get; }
}

