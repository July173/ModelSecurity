using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class FormModule
    {
        public int id { get; set; }
        public string status_procedure { get; set; }
        public int formId { get; set; }
        public Form form1 { get; set; }
        public int moduleId { get; set; }
        public Module module1 { get; set; }
    }
}
