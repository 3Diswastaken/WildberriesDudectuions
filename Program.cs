using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildberriesDudectuions
{
    class Program
    {
        static async Task Main(string[] args)
        {
            RequestConfig config = ConfigLoader.Load("C:\\Users\\rusla\\source\\repos\\WildberriesDudectuions\\headers.json");

            WbRequestService service = new(config);
            string pickpointId = config.Headers["X-PickPoint-External-Id"];

            try
            {
                string jsonData = await service.GetDataForPickpoint(pickpointId);
                Console.WriteLine(jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запросе {ex.Message}");
            }
        }
    }
}
