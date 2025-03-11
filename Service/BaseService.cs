using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BaseService
    {
        public IQueryable<T> GetItemsByDateRangeAsync<T>(IQueryable<T> items, DateTime? startDate, DateTime? endDate) where T : class
        {
            if (startDate.HasValue)
            {
                var property = typeof(T).GetProperty("CreatedDate");
                if (property != null)
                {
                    items = items.Where(item => (DateTime)property.GetValue(item) >= startDate.Value);
                }
            }
            if (endDate.HasValue)
            {
                var property = typeof(T).GetProperty("CreatedDate");
                if (property != null)
                {
                    items = items.Where(item => (DateTime)property.GetValue(item) <= endDate.Value.AddDays(1).AddTicks(-1)); //Lấy cuối ngày End date
                }
            }
            return items;
        }
    }

}
