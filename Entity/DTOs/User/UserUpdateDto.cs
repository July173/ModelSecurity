using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.User
{
   public class UserUpdateDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
 
    }
}
