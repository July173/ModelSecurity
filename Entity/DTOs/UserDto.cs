using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class UserDto
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public bool active { get; set; }
    }
}
