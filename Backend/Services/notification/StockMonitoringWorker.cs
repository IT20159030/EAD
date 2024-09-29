/**
* This class is responsible for running the stock monitoring service in the background.
* It is a background service that runs every 2 minutes to check the stock levels of all products.
* It uses the StockMonitoringService to monitor the stock levels.
*/


using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Backend.Services.notification
{
    public class StockMonitoringWorker : BackgroundService
    {
        private readonly StockMonitoringService _monitoringService;

        public StockMonitoringWorker(StockMonitoringService monitoringService)
        {
            _monitoringService = monitoringService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _monitoringService.MonitorStockLevelsAsync();
                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken); // Run every 2 minutes
            }
        }
    }
}
