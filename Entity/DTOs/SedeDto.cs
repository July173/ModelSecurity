using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class SedeDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public long codeSede { get; set; }
        public string address { get; set; }
        public short phoneSede { get; set; }
        public string emailContacto { get; set; }
        public bool active { get; set; }
        
    }
}
