using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public int PersonId { get; set; }
        public Person Person { get; set; }

        public int UserRolId { get; set; }
        public ICollection<UserRol> UserRol { get; set; }
        public int UserSedeId { get; set; }
        public ICollection<UserSede> UserSede { get; set; }
        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; }
        public int AprendizId { get; set; }
        public Aprendiz Aprendiz { get; set; }

    }
}