using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apis.Domain
{
    public abstract class BaseDomain
    {
        public BaseDomain()
        {
            this.SqlConnection  = new SqlConnection(@"Data Source=PC0443\MSSQL2017;Initial Catalog=FileUpload;Integrated Security=True;MultipleActiveResultSets=True");
            this.SqlConnection.Open();
        }
        public SqlDataReader GetReader(string commandText)
        {
            this.SqlCommand = new SqlCommand(commandText, this.SqlConnection);
            return this.SqlCommand.ExecuteReader();
        }
        public void ExecuteNonQuery(string commandText)
        {
            this.SqlCommand = new SqlCommand(commandText, this.SqlConnection);
            this.SqlCommand.ExecuteReader();
        }
        public void CloseConnection()
        {
            this.SqlConnection.Close();
        }
        protected SqlCommand SqlCommand { get; set; }
        private SqlConnection SqlConnection { get; set; }

    }
}
