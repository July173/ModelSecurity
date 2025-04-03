using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class UserRol
    {
        public int id { get; set; }
        public int userId { get; set; }
        public User name { get; set; }
        public int rolId { get; set; }
        public Rol rol { get; set; }
    }
}
