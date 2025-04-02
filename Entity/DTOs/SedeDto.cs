using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class SedeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long CodeSede { get; set; }
        public string Address { get; set; }
        public short PhoneSede { get; set; }
        public string EmailContacto { get; set; }
        //public bool Active { get; set; }
        //public DateTime CreateDate { get; set; }
        //public DateTime DeleteDate { get; set; }
        //public DateTime UpdateDate { get; set; }
    }
}
