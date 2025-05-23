﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.Center
{
    public class CenterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CodeCenter { get; set; }
        public bool Active { get; set; }
        public int RegionalId { get; set; }
        public string Address { get; set; }
    }
}
