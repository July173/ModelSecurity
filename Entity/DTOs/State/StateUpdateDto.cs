﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.State
{
    public class StateUpdateDto
    {
        public int Id { get; set; }
        public string TypeState { get; set; }
        public string Description { get; set; }
    
    }
}
