using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    internal class EnterpriseDTOAuto
    {
        public int Id { get; set; }
        //public DateTime CreateDate { get; set; }
        //public DateTime DeleteDate { get; set; }
        //public DateTime UpdateDelete { get; set; }
        public string Observation { get; set; }
        public string NameBoss { get; set; }
        public string NameEnterprise { get; set; }
        public short PhoneEnterprise { get; set; }
        public string Locate { get; set; }
        public string EmailBoss { get; set; }
        public short NitEnterprise { get; set; }
        //public bool Active { get; set; }
        public string EmailEnterprise { get; set; }
    }
}
