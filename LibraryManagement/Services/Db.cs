using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Services
{
    public static class Db
    {
        private const string ConnStr =
    "Server=DESKTOP-EJ40GA0\\SQLEXPRESS;Database=LibraryDB2;Trusted_Connection=True;TrustServerCertificate=True;";


        public static object? Scalar(string sql, params SqlParameter[] parameters)
        {
            using var con = new SqlConnection(ConnStr);
            con.Open();

            using var cmd = new SqlCommand(sql, con);
            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            return cmd.ExecuteScalar();
        }
        public static int NonQuery(string sql, params SqlParameter[] parameters)
        {
            using var con = new SqlConnection(ConnStr);
            con.Open();

            using var cmd = new SqlCommand(sql, con);
            if (parameters != null && parameters.Length > 0)
                cmd.Parameters.AddRange(parameters);

            return cmd.ExecuteNonQuery();
        }

    }
}
