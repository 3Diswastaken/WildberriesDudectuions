using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildberriesDudectuions.Models
{
    public interface GoodsInterface
    {
        public long Shk { get; set; }
        public DateTime? OnHoldDate { get; set; }
        public long TicketId { get; set; }
        public decimal Price { get; set; }
    }
}
