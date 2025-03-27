using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model2
{
    class UserSede
    {
        public int Id { get; set; }
        public string status_procedure { get; set; }
        public int UserId { get; set; }
        public User1 User1 { get; set; }
        public int SedeId { get; set; }
        public Sede Sede { get; set; }

    }
}
