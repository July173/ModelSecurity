using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model2
{
    class FormModule1
    {
        public int Id { get; set; }
        public string status_procedure { get; set; }
        public int FormId { get; set; }
        public Form1 Form1 { get; set; }
        public int ModuleId { get; set; }
        public Module1 Module1 { get; set; }
    }
}
