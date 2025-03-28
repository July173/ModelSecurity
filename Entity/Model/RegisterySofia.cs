using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class RegisterySofia
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string document { get; set; }
        public bool active { get; set; }
        public DateTime create_date { get; set; }
        public DateTime delete_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
