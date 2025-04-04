using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Enterprise
    {
        public int Id { get; set; }
        public string Observation { get; set; }
        public string NameEnterprise { get; set; }
        public string PhoneEnterprise { get; set; }
        public string Locate { get; set; }
        public string NitEnterprise { get; set; }
        public string EmailEnterprise { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DeleteDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
