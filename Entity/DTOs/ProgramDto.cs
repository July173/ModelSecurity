using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class ProgramDto
    {
        public int id { get; set; }
        public decimal codeProgram { get; set; }
        public string name { get; set; }
        public string typeProgram { get; set; }
       
        public bool active { get; set; }
        public string description { get; set; }
    }
}
