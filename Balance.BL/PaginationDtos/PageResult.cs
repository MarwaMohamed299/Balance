using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balance.BL.PaginationDtos
{
    public class PageResult<T>
    {
        public int TotalCount { get; set; }
        public List<T>? Data { get; set; }
    }
}
