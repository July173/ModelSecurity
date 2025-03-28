using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    class FormModule
    {
        public int Id { get; set; }
        public string status_procedure { get; set; }
        public int FormId { get; set; }
        public Form Form1 { get; set; }
        public int ModuleId { get; set; }
        public Module Module1 { get; set; }
    }
}
