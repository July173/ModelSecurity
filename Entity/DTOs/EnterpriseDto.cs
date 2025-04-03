using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class EnterpriseDto
    {
        public int id { get; set; }
        public string observation { get; set; }
        public string nameEnterprise { get; set; }
        public short phoneEnterprise { get; set; }
        public string locate { get; set; }
        public string nitEnterprise { get; set; }
        public bool active { get; set; }
        public string emailEnterprise { get; set; }
    }
}
