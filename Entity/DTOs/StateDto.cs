﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class StateDto
    {
        public int id { get; set; }
        
        public bool active { get; set; }
        public string typeState { get; set; }
        public string description { get; set; }
    }
}
