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
                rdr = cnx.ExecuteCommand("SELECT * FROM TI_FUENTE WHERE ID_MODELO =" + m, CommandType.Text);


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
        public string getCampoCatalogo(string model)
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
        public string updateUsuario(string id, string nombre, string apellido_p, string apellido_m, string contrasena, string correo, Object[] fuentes)
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
                string datos = JsonConvert.SerializeObject(fuentes);
                SqlDataReader r2;
                cnx cnxDel = new cnx();
                SqlParameter[] parametro = new SqlParameter[1];
                parametro[0] = new SqlParameter() { ParameterName = "@id", Value = id };
                r2 = cnxDel.ExecuteCommand("DELETE TI_FUENTE_USUARIO WHERE ID_USUARIO=@id", CommandType.Text, parametro);
                var fuentesSelect = JsonConvert.DeserializeObject<List<fuente>>(datos);
                for (int i = 0; i < fuentesSelect.Count; i++)
                {
                    SqlDataReader r;
                    cnx cnxAux = new cnx();
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
            string init = "";
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

                        using (OleDbCommand olecmd = new OleDbCommand("SELECT * FROM [Hoja1$]", con))
                        {
                            OleDbDataReader rdr = olecmd.ExecuteReader();
                            int columns = rdr.FieldCount;
                            init = "[";
                            if (rdr.HasRows)
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


                                while (rdr.Read())
                                {
                                    init += "{" + '"' + "data" + '"' + ":[";
                                    for (int i = 0; i < columns; i++)
                                    {
                                        init += '"' + rdr[i].ToString() + '"';
                                        if (i != columns - 1)
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
                            init += "]";
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
                if (r2 != null)
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
        public void putWorkTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID_FUENTE");
            dt.Columns.Add("ID_VARIABLE_COMPUESTA");
            dt.Columns.Add("ID_VARIABLE_PADRE");
            dt.Columns.Add("ID_VARIABLE");
            dt.Columns.Add("ID_TIPO_DATO");
            dt.Columns.Add("ID_ACTIVIDAD_COMPUESTA");
            dt.Columns.Add("ID_ACTIVIDAD_PADRE");
            dt.Columns.Add("ID_ACTIVIDAD");
            dt.Columns.Add("ID_TIPO_PERIODICIDAD");
            dt.Columns.Add("ID_PERIODICIDAD");
            dt.Columns.Add("ANIO");
            dt.Columns.Add("ID_ESTATUS");
            dt.Columns.Add("ID_ENTIDAD");
            dt.Columns.Add("ID_ESTATUS_CIFRA");
            dt.Columns.Add("VALOR");
            dt.Columns.Add("VALOR_PRESENTACION");
            dt.Columns.Add("FECHA_INSERCION");
            dt.Columns.Add("REFERENCIA");
            dt.Columns.Add("VERSION");
            dt.Columns.Add("IMP");
            dt.Columns.Add("ANIO_BASE");
            dt.Columns.Add("DESGLOSE_GEOGRAFICO");
            dt.Columns.Add("UBICACION_GEOGRAFICA");
            dt.Columns.Add("PERIODO");
            dt.Columns.Add("ESTATUS_DE_LA_INFORMACION");
            dt.Columns.Add("EXCEPCION");

            DataRow row = dt.NewRow();

            row[0] = 3.ToString();
            row[1] = 1.ToString();
            row[2] = 2.ToString();
            row[3] = 2.ToString();
            row[4] = 3.ToString();
            row[5] = 0.ToString();
            row[6] = 0.ToString();
            row[7] = 0.ToString();
            row[8] = 1.ToString();
            row[9] = 1.ToString();
            row[10] = 2015.ToString();
            row[11] = 1.ToString();
            row[12] = 0.ToString();
            row[13] = 1.ToString();
            row[14] = 10000.ToString();
            row[15] = 10001.ToString();
            row[16] = null;
            row[17] = 1.ToString();
            row[18] = 1.1.ToString();
            row[19] = 1;
            row[20] = 2015.ToString();
            row[21] = null;
            row[22] = null;
            row[23] = null;
            row[24] = null;
            row[25] = null;

            dt.Rows.Add(row);

            DataRow row1 = dt.NewRow();

            row1[0] = 3.ToString();
            row1[1] = 2.ToString();
            row1[2] = 2.ToString();
            row1[3] = 2.ToString();
            row1[4] = 3.ToString();
            row1[5] = 0.ToString();
            row1[6] = 0.ToString();
            row1[7] = 0.ToString();
            row1[8] = 1.ToString();
            row1[9] = 1.ToString();
            row1[10] = 2015.ToString();
            row1[11] = 1.ToString();
            row1[12] = 1.ToString();
            row1[13] = 1.ToString();
            row1[14] = 10000.ToString();
            row1[15] = 10001.ToString();
            row1[16] = null;
            row1[17] = 1.ToString();
            row1[18] = 1.1.ToString();
            row1[19] = 1;
            row1[20] = 2015.ToString();
            row1[21] = null;
            row1[22] = null;
            row1[23] = null;
            row1[24] = null;
            row1[25] = null;
            dt.Rows.Add(row1);


            DataRow row2 = dt.NewRow();

            row2[0] = 3.ToString();
            row2[1] = 2.ToString();
            row2[2] = 2.ToString();
            row2[3] = 2.ToString();
            row2[4] = 3.ToString();
            row2[5] = 0.ToString();
            row2[6] = 0.ToString();
            row2[7] = 0.ToString();
            row2[8] = 1.ToString();
            row2[9] = 1.ToString();
            row2[10] = 2015.ToString();
            row2[11] = 1.ToString();
            row2[12] = 1.ToString();
            row2[13] = 1.ToString();
            row2[14] = 10000.ToString();
            row2[15] = 10001.ToString();
            row2[16] = null;
            row2[17] = 1.ToString();
            row2[18] = 1.1.ToString();
            row2[19] = 1;
            row2[20] = 2015.ToString();
            row2[21] = null;
            row2[22] = null;
            row2[23] = null;
            row2[24] = null;
            row2[25] = null;
            dt.Rows.Add(row2);

            DataTable tabledistinct = new DataTable();


            //string[] columns = { "ID_FUENTE", "ID_VARIABLE_COMPUESTA" };
            //tabledistinct = dt.DefaultView.ToTable(true, new string[] { "ID_FUENTE", "ID_VARIABLE_COMPUESTA" });


            DataTable dtclean = new DataTable();

            dtclean = dt.Clone();

            dtclean.Rows.Clear();

            List<string> list = new List<string>();
            foreach (DataRow irow in dt.Rows)
            {
                string key = "";
                key += irow[0].ToString();
                key += irow[1].ToString();
                key += irow[2].ToString();


                DataRow xrow = dtclean.NewRow();
                xrow.ItemArray= ((DataRow)irow).ItemArray;
                int i = list.IndexOf(key);
                if(i==-1)
                {
                    list.Add(key);
                    dtclean.Rows.Add(xrow);
                }
                
            }



            //DataTable dtDistinct = new DataTable();

            
            try
            {

  
                        cnx cnxAux = new cnx("conexion1");
                        SqlParameter[] _params = new SqlParameter[1]{
                            new SqlParameter{
                                ParameterName = "@TABLE",
                                Value = dtclean,
                                SqlDbType = SqlDbType.Structured
                            }
                        };


                       int rows= cnxAux.ExecuteTransaction("PR_TRANSAC_AUTOMATIZACION", CommandType.StoredProcedure,_params);
               
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
                rdr = cnx.ExecuteCommand("SELECT * FROM TI_ER WHERE ID_MODELO =  " + id_modelo + " AND TABLA_DE_TRABAJO = 'true'", CommandType.Text);
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        SqlDataReader r;
                        cnx cnxAux = new cnx();
                        r = cnxAux.ExecuteCommand("SELECT * FROM TR_TABLA WHERE ID_MODELO =" + id_modelo + " AND ID_CATALOGO = " + rdr["ID_CATALOGO"].ToString(), CommandType.Text);
                        if (r.HasRows)
                        {
                            while (r.Read())
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
        public void getUsuarios()
        {
            try
            {

                cnx = new cnx();

                List<usuarios> usuariosList = new List<usuarios>();
                rdr = cnx.ExecuteCommand("SELECT * FROM TC_USUARIO WHERE ID_PERFIL=2", CommandType.Text);
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        SqlDataReader r;
                        cnx cnxAux = new cnx();
                        r = cnxAux.ExecuteCommand("SELECT fu.ID_MODELO,fu.ID_FUENTE,tf.NOMBRE FROM TI_FUENTE_USUARIO fu  inner join TI_fuente tf on fu.ID_FUENTE=tf.ID_FUENTE and fu.ID_MODELO=tf.ID_MODELO and fu.ID_USUARIO='" + rdr["ID_USUARIO"].ToString() + "'", CommandType.Text);
                        usuarios user = new usuarios()
                        {
                            id = rdr["ID_USUARIO"].ToString(),
                            nombre = rdr["NOMBRE"].ToString(),
                            apellido_p = rdr["APELLIDO_P"].ToString(),
                            apellido_m = rdr["APELLIDO_M"].ToString(),
                            correo = rdr["CORREO"].ToString(),
                            fecha_na = rdr["FECHA_NACIMIENTO"].ToString(),
                            contrasena = rdr["PASSWORD"].ToString()
                        };


                        if (r != null)
                        {
                            user.fuentes = new List<fuenteUsuario>();
                            while (r.Read())
                            {
                                fuenteUsuario f = new fuenteUsuario()
                                {
                                    id = r["ID_FUENTE"].ToString(),
                                    id_modelo = r["ID_MODELO"].ToString(),
                                    nombre = r["NOMBRE"].ToString()
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
                rdr = cnx.ExecuteCommand("select t2.NOMBRE as TABLA_TRABAJO, t1.ID_CATALOGO as ID_CATALOGO, t1.NOMBRE as CATALOGO from(SELECT ER.ID_CATALOGO,C.NOMBRE from TC_CATALOGO C INNER JOIN TI_ER ER ON C.ID_CATALOGO=ER.ID_CATALOGO WHERE ID_MODELO=@modelo)t1 left join (select NOMBRE from TI_ER E INNER JOIN TC_CATALOGO C ON E.ID_CATALOGO=C.ID_CATALOGO WHERE E.ID_MODELO=@modelo AND E.TABLA_DE_TRABAJO=1)t2 on (t2.NOMBRE = t1.NOMBRE)", CommandType.Text, parameters);

                List<catalogos> list = new List<catalogos>();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        catalogos c = new catalogos()
                        {
                            nombre = rdr["CATALOGO"].ToString(),
                            id_catalogo = rdr["ID_CATALOGO"].ToString(),
                            T_trabajo = rdr["TABLA_TRABAJO"].ToString()
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
        
        [WebMethod]
        public string validaCargaVista(string vista, string tabla)
        {
            try
            {
                cnx = new cnx();

                rdr = cnx.ExecuteCommand("SELECT C1.CONT AS CONT, C2.EXISTENTROWS AS EXISTENTROWS FROM (SELECT COUNT(VECES) AS CONT FROM (select COUNT(*) AS VECES from  "+vista+"  group by ID_FUENTE ,ID_VARIABLE_COMPUESTA,ID_VARIABLE,ID_TIPO_DATO,ID_ACTIVIDAD_COMPUESTA,ID_ACTIVIDAD_PADRE,ID_ACTIVIDAD,ID_TIPO_PERIODICIDAD,ID_PERIODICIDAD,ANIO,ID_ESTATUS,ID_ENTIDAD,ID_ESTATUS_CIFRA) as t WHERE VECES > 1) C1 LEFT JOIN (SELECT COUNT(*) AS EXISTENTROWS FROM "+vista+" A INNER JOIN "+tabla+" B ON A.ID_VARIABLE_COMPUESTA = B.ID_VARIABLE_COMPUESTA AND A.ID_VARIABLE_PADRE = B.ID_VARIABLE_PADRE AND A.ID_VARIABLE = B.ID_VARIABLE AND A.ID_TIPO_DATO = B.ID_TIPO_DATO AND A.ID_ACTIVIDAD_COMPUESTA = B.ID_ACTIVIDAD_COMPUESTA AND A.ID_ACTIVIDAD_PADRE = B.ID_ACTIVIDAD_PADRE AND A.ID_ACTIVIDAD = B.ID_ACTIVIDAD AND A.ID_TIPO_PERIODICIDAD = B.ID_TIPO_PERIODICIDAD AND A.ID_PERIODICIDAD = B.ID_PERIODICIDAD AND A.ANIO = B.ANIO AND A.ID_ESTATUS = B.ID_ESTATUS AND A.ID_ENTIDAD = B.ID_ENTIDAD AND A.ID_ESTATUS_CIFRA = B.ID_ESTATUS_CIFRA AND A.ID_FUENTE = B.ID_FUENTE) C2 ON C1.CONT >= 0 and C2.EXISTENTROWS >= 0", CommandType.Text);

                List<comp_vista> list = new List<comp_vista>();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        comp_vista f = new comp_vista()
                        {
                            cont = rdr["CONT"].ToString(),
                            existentrows = rdr["EXISTENTROWS"].ToString()
                        };
                        list.Add(f);
                    }
                    rdr.Close();
                    rdr = null;
                    string data = JsonConvert.SerializeObject(list);

                    return data;
                }
            }
            catch (Exception )
            {
                return "nomms";
                //throw ex;
            }
            return "";
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
public class fuentes
{
    public string nombre { set; get; }
    public string id_fuente { set; get; }
    public string id_modelo { set; get; }
}

public class campo
{
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
public class catalogos
{
    public string id_catalogo { set; get; }
    public string nombre { set; get; }
    public string T_trabajo { set; get; }
}
public class comp_vista
{
    public string cont { set; get; }
    public string existentrows { set; get; }
}
