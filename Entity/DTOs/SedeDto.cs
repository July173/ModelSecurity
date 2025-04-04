using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class SedeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CodeSede { get; set; }
        public string Addres { get; set; }
        public string PhoneSede { get; set; }
        public string EmailContact { get; set; }
        public int CenterId { get; set; }
        public bool Active { get; set; }
    }
}
