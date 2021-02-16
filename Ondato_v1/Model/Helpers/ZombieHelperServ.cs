using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Ondato.Model.Helpers;

namespace ExchangeRates.Services
{
    public class ZombieHelperServ : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IConfiguration Configuration;
        private readonly CarDataHelper _helper;

        public ZombieHelperServ(IConfiguration iConfig)
        {
            Configuration = iConfig;
            _helper = new CarDataHelper(iConfig);

        }
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(CleanZombies, null, TimeSpan.Zero,
                TimeSpan.FromDays(Convert.ToInt32(Configuration["CarsApiConst:CleanUpDays"])));
            return Task.CompletedTask;
        }

        private void CleanZombies(object state)
        {
            _helper.DeleteExpired();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}