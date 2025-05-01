using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.UserRol
{
    public class UserRolAssignDto
    {
        public int UserId { get; set; }
        public List<int> RolIds { get; set; } = new();
    }
}
