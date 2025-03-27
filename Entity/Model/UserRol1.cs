using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model2
{
    class UserRol1
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User1 Name { get; set; }
        public int RolId { get; set; }
        public Rol1 Rol { get; set; }
    }
}
