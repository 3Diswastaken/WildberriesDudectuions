using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WildberriesDeductions.Models;
using WildberriesDeductions.Models;

namespace WildberriesDeductions
{
    public static class ResultsConverter
    {
        public static DefectGoodsRoot ConvertDefectGoods(string jsonData) => JsonSerializer.Deserialize<DefectGoodsRoot>(jsonData);
        public static ReplaceGoodsRoot ConvertReplaceGoods(string jsonData) => JsonSerializer.Deserialize<ReplaceGoodsRoot>(jsonData);
        public static HandingGoodsRoot ConvertHandingGoods(string jsonData) => JsonSerializer.Deserialize<HandingGoodsRoot>(jsonData);
        public static TicketsRoot ConvertTickets(string jsonData) => JsonSerializer.Deserialize<TicketsRoot>(jsonData);
        public static LoginRoot ConvertLoginResponse(string jsonData) => JsonSerializer.Deserialize<LoginRoot>(jsonData);
        public static ValidateRoot ConvertValidateResponse(string jsonData) => JsonSerializer.Deserialize<ValidateRoot>(jsonData);
        public static SwitchRoot ConvertSwitchResponse(string jsonData) => JsonSerializer.Deserialize<SwitchRoot>(jsonData);

    }
}
