using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
   public class UserSede
    {
        public int id { get; set; }
        public string status_procedure { get; set; }
        public int userId { get; set; }
        public User user { get; set; }
        public int sedeId { get; set; }
        public Sede sede { get; set; }

    }
}
