using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model2
{
    class User1
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        public int PersonId { get; set; }
        public Person1 Person1 { get; set; }
    }
}