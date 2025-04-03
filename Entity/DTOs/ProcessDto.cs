using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class ProcessDto
    {
        public int id { get; set; }
        public string typeProcess { get; set; }
        public bool active { get; set; }
        public DateTime startAprendiz { get; set; }
        public string observation { get; set; }
    }
}
