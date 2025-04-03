using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public  class RolDTO
    {
        public int id { get; set; }
        public string typeRol { get; set; }
        public string description { get; set; }
        public bool active { get; set; }
    }
}
