using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WildberriesDeductions.Wildberries.Models;

namespace WildberriesDeductions.Wildberries
{
    public record AllConfigs
        (
            RequestConfig defectGoodsConfig,
            RequestConfig handingGoodsConfig,
            RequestConfig replaceGoodsConfig,
            RequestConfig ticketsConfig,
            RequestConfig loginConfig,
            RequestConfig validateConfig,
            RequestConfig switchConfig,
            RequestConfig pickpointsListConfig,
            RequestConfig findByShkConfig
        );
    public static class Factory
    {
        public static AllConfigs LoadConfigs()
        {
            // конфиг на каждый тип удержаний со своими url и headers
            RequestConfig defectGoodsConfig = ConfigLoader.LoadDefectGoods();
            RequestConfig ticketsConfig = ConfigLoader.LoadTickets(); // использует post и payload
            RequestConfig replaceGoodsConfig = ConfigLoader.LoadReplaceGoods();
            RequestConfig handingGoodsConfig = ConfigLoader.LoadHandingGoods();
            RequestConfig loginConfig = ConfigLoader.LoadLogin();
            RequestConfig validateConfig = ConfigLoader.LoadValidate();
            RequestConfig switchConfig = ConfigLoader.LoadSwitch();
            RequestConfig pickpointsListConfig = ConfigLoader.LoadPickpoints();
            RequestConfig findByShkConfig = ConfigLoader.LoadFindByShk();

            return new AllConfigs(
                defectGoodsConfig,
                handingGoodsConfig,
                replaceGoodsConfig,
                ticketsConfig,
                loginConfig,
                validateConfig,
                switchConfig,
                pickpointsListConfig,
                findByShkConfig
            );
        }
    }
}
