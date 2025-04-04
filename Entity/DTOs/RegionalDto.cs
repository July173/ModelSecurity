using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class RegionalDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CodeRegional { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
    }
}
