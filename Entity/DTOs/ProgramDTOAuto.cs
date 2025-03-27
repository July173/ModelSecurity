using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    internal class ProgramDTOAuto
    {
        public int Id { get; set; }
        public decimal CodeProgram { get; set; }
        public string Name { get; set; }
        public string TypeProgram { get; set; }
        //public DateTime CreateDate { get; set; }
        //public DateTime DeleteDate { get; set; }
        //public DateTime UpdateDate { get; set; }
        //public bool Active { get; set; }
        public string Description { get; set; }
    }
}
