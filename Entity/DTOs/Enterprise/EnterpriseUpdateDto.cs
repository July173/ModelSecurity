using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.Enterprise
{
    public class EnterpriseUpdateDTO
    {
        public int Id { get; set; }
        public string Observation { get; set; }
        public string NameEnterprise { get; set; }
        public string PhoneEnterprise { get; set; }
        public string Locate { get; set; }
        public string EmailEnterprise { get; set; }
        
    }
}
