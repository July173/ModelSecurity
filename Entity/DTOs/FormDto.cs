using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class FormDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string cuestion { get; set; }
        public string typeCuestion { get; set; }
        public string answer { get; set; }
        public bool active { get; set; }
        
    }
}
