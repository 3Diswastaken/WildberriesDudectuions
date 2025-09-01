using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WildberriesDudectuions.Models;

namespace WildberriesDudectuions
{
    public static class ResultsConverter
    {
        public static DefectGoods Convert(string jsonData) => JsonSerializer.Deserialize<DefectGoods>(jsonData);
    }
}
