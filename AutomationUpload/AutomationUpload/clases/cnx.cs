using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace AutomationUpload
{
    public class cnx
    {
        SqlConnection conn;
        SqlDataAdapter sda;
        SqlDataReader rdr;
        SqlCommand cmd;

        public cnx()
        {
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conexionsql"].ToString());
            conn.Open();
        }

        private SqlCommand PrepareCommand(string cmdtext, CommandType cmdtype, SqlParameter[] paramcollection = null)
        {
            cmd = new SqlCommand(cmdtext);
            cmd.Connection = conn;
            cmd.CommandType = cmdtype;

            if (paramcollection != null)
            {
                for (int i = 0; i < paramcollection.Length; i++)
                {
                    cmd.Parameters.Add(paramcollection[i]);
                }
            }


            return cmd;

        }

        public SqlDataReader ExecuteCommand(string cmdtext, CommandType cmdtype, SqlParameter[] paramcollection = null)
        {
            try
            {
                cmd = PrepareCommand(cmdtext, cmdtype, paramcollection);
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Dispose();
                return rdr;

            }
            catch
            {
                return rdr;
            }
        }

        public SqlDataReader ExecuteCommand(string cmdtext, CommandType cmdtype)
        {
            try
            {
                cmd = PrepareCommand(cmdtext, cmdtype);
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Dispose();
                return rdr;

            }
            catch
            {
                return rdr;
            }
        }


    }
}