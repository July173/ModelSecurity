using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class ProcessDto
    {
        public int Id { get; set; }
        public string StartAprendiz { get; set; }
        public string TypeProcess { get; set; }
        public string Observation { get; set; }
        public bool Active { get; set; }
    }
}
