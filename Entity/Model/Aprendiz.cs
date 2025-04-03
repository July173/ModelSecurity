using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Aprendiz
    {
        public int id { get; set; }
        public string previousProgram { get; set; }
        public bool active { get; set; }
        public int userId { get; set; }
        public User user { get; set; }

    }
}
