using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WildberriesDeductions.Models;
using WildberriesDudectuions.Models;

namespace WildberriesDudectuions
{
    public static class ResultsConverter
    {
        public static DefectGoods ConvertDefectGoods(string jsonData) => JsonSerializer.Deserialize<DefectGoods>(jsonData);
        public static ReplaceGoods ConvertReplaceGoods(string jsonData) => JsonSerializer.Deserialize<ReplaceGoods>(jsonData);
        public static Root ConvertHandingGoods(string jsonData) => JsonSerializer.Deserialize<Root>(jsonData);
        public static TicketsRoot ConvertTickets(string jsonData) => JsonSerializer.Deserialize<TicketsRoot>(jsonData);
    }
}
