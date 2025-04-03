using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class InstructorProgram
    {
        public int id { get; set; }
        public int instructorId { get; set; }
        public Instructor instructor { get; set; }
        public int programId { get; set; }
        public Program program { get; set; }
    }
}
