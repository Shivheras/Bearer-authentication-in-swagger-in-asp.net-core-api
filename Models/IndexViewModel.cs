using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apis.Models
{
    public class IndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string Path { get; set; }
        public byte[] Data { get; set; }
    }
}
