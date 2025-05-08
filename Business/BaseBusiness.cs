using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Business
{
    public class BaseBusiness<T> : IBaseBusiness<T> where T : class
    {
        protected readonly IBaseData<T> _data;

        public BaseBusiness(IBaseData<T> data)
        {
            _data = data;
        }

        public virtual async Task<List<T>> GetAllAsync() => await _data.GetAllAsync();
        public virtual async Task<T> GetByIdAsync(int id) => await _data.GetByIdAsync(id);
        public virtual async Task<T> CreateAsync(T entity) => await _data.CreateAsync(entity);

        public virtual async Task UpdateAsync(T entity) => await _data.UpdateAsync(entity);
        public virtual async Task DeleteAsync(int id) => await _data.DeleteAsync(id);

        // Fix for CS0535: Implementing the missing CreateAsync method
       public virtual async Task<T> CreateAsync() => await Task.FromResult(default(T));
    }

}
