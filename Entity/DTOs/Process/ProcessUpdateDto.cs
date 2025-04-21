using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.Process
{
    public class ProcessUpdateDto
    {
        public int Id { get; set; }
        public string TypeProcess { get; set; }
        public string Observation { get; set; }
       
    }
}
