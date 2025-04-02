﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class EnterpriseDto
    {
        public int Id { get; set; }
        public string Observation { get; set; }
        public string NameBoss { get; set; }
        public string NameEnterprise { get; set; }
        public short PhoneEnterprise { get; set; }
        public string Locate { get; set; }
        public string EmailBoss { get; set; }
        public short NitEnterprise { get; set; }
        public bool Active { get; set; }
        public string EmailEnterprise { get; set; }
    }
}
