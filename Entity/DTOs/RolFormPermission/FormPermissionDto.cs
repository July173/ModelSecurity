﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.RolFormPermission
{
       public  class FormPermissionDto
    {
        public int FormId { get; set; }
        public List<int> PermissionIds { get; set; }
    }
}
