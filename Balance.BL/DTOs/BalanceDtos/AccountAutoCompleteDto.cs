using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balance.BL.DTOs.BalanceDtos
{
    public class AccountAutoCompleteDto
    {
        public int Id { get; set; }
        public string? Text { get; set; } 
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
    }

}
