﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class RegistrerySofiaDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string document { get; set; }
        public bool active { get; set; }
    }
}
