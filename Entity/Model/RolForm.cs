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
        public Rol rol1 { get; set; }
        public int form1Id { get; set; }
        public Form form1 { get; set; }

    }
}
