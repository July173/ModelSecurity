﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.Verification
{
    public class VerificationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Observation { get; set; }
        public bool Active { get; set; }

    }
}
