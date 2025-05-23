﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Instructor
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public ICollection<InstructorProgram> InstructorProgram { get; set; }
        public ICollection<AprendizProcessInstructor> AprendizProcessInstructor { get; set; }


    }
}
