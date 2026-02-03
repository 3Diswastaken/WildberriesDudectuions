using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildberriesDeductions.Models
{
    public interface GoodsInterface
    {
        public long Shk { get; set; }
        public DateTime? OnHoldDate { get; set; }
        public long TicketId { get; set; }
        public decimal Price { get; set; }
        public string Reason { get; set; }
        public string Pickpoint { get; set; }
    }
}
