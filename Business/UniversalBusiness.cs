using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Business
{
    public class UniversalBusiness<T> : BaseBusiness<T> where T : class
    {
        public UniversalBusiness(IBaseData<T> data) : base(data) { }
    }

}
