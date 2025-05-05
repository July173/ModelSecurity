using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Rol
    {
        public int Id { get; set; }
        public string TypeRol { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; }


        public ICollection<RolFormPermission> RolFormPermissions { get; set; }

        [JsonIgnore]
        public ICollection<UserRol> UserRol  { get; set; }

    }
}
