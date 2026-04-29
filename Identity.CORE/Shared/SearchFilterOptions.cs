using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Shared
{
    public class SearchFilterOptions
    {
        public string? SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public bool? IsFasting { get; set; }
        public bool? HaveSale { get; set; }

        // Paging and Sorting
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        
        
    }
}
