using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Program
    {
        public int Id { get; set; }
        public decimal code_program { get; set; }
        public string Name { get; set; }
        public string type_program { get; set; }
        public DateTime create_date { get; set; }
        public DateTime delete_date { get; set; }
        public DateTime update_date { get; set; }
        public bool active { get; set; }
        public string Description { get; set; }
    }
}
