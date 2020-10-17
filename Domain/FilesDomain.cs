using apis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apis.Domain
{
   
    public class FilesDomain:BaseDomain
    {
        public List<IndexViewModel> Get()
        {
            var reader = this.GetReader($"Select * from tblFiles");
            var ViewModel = new List<IndexViewModel>();
            while(reader.Read())
            {
                var model = new IndexViewModel();
                model.Id = Convert.ToInt32(reader["Id"]);
                model.Name = reader["Name"].ToString();
                model.Path = reader["ContentType"].ToString();
                ViewModel.Add(model);
            }
            return ViewModel;
        }
    }
}
