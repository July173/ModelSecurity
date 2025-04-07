using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Model;

namespace Entity.DTOautogestion
{
    public class UserDto
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int PersonId { get; set; }
        public int UserRolId { get; set; }
        public int UserSedeId { get; set; }
        public int InstructorId { get; set; }
        public int AprendizId { get; set; }
    }
}

