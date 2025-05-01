using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.RolFormPermission
{
    public class AssignPermissionsDto
    {
        public int RolId { get; set; }
        public List<FormPermissionDto> FormPermissions { get; set; }
    }
}
