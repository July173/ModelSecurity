using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class RolForm
    {
        public int id { get; set; }
        public string permission { get; set; }
        public int rolId { get; set; }
        public Rol rol{ get; set; }
        public int formId { get; set; }
        public Form form { get; set; }

    }
}
