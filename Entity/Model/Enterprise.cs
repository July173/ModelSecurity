using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model2
{
    class Enterprise
    {
        public int Id { get; set; }
        public string observation { get; set; }
        public string name_boss { get; set; }
        public string name_enterprise { get; set; }
        public string phone_enterprise { get; set; }
        public string locate { get; set; }
        public string email_boss { get; set; }
        public string nit_enterprise { get; set; }
        public string email_enterprise { get; set; }
        public bool active { get; set; }
        public DateTime create_date { get; set; }
        public DateTime delete_date { get; set; }
        public DateTime update_date { get; set; }    
    }
}
