using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model2
{
    class Sede
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string code_sede { get; set; }
        public string addres { get; set; }
        public string phone_sede { get; set; }
        public string email_contact { get; set; }
        public bool active { get; set; }
        public DateTime create_date { get; set; }
        public DateTime delete_date { get; set; }
        public DateTime update_date { get; set; }
        public int CenterId { get; set; }
        public Center Center { get; set; }
    }
}
