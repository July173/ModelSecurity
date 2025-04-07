﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class CenterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CodeCenter { get; set; }
        public bool Active { get; set; }
        public int RegionalId { get; set; }
        public string Address { get; set; }
        public int SedeId { get; set; }
    }
}
