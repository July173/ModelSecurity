using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Process
    {
        public int Id { get; set; }
        public string start_aprendiz { get; set; }
        public string observation { get; set; }
        public string type_procces { get; set; }
        public bool active { get; set; }
        public DateTime create_date { get; set; }
        public DateTime delete_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
