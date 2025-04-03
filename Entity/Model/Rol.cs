using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Rol
    {
        public int id { get; set; }
        public string type_rol { get; set; }
        public string description { get; set; }
        public bool active { get; set; }
    }
}
