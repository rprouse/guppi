using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataProvider.Weather;

namespace Alteridem.MyDay
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var provider = new WeatherDataProvider();
            if(!provider.Configured)
                provider.Configure();

            await provider.Execute(false);
            await provider.Execute(true);
        }
    }
}
