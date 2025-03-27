using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model2
{
    class RolForm1
    {
        public int Id { get; set; }
        public string permission { get; set; }
        public int RolId { get; set; }
        public Rol1 Rol1 { get; set; }
        public int Form1Id { get; set; }
        public Form1 Form1 { get; set; }

    }
}
