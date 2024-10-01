/**
* This class is responsible for running the monitoring services in the background.
*/


using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Backend.Services.notification
{

    public class MonitoringWorker : BackgroundService
    {
        private readonly UserMonitoringService _monitoringService;
        private readonly StockMonitoringService _stockMonitoringService;

        public MonitoringWorker(UserMonitoringService monitoringService, StockMonitoringService stockMonitoringService)
        {
            _monitoringService = monitoringService;
            _stockMonitoringService = stockMonitoringService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _monitoringService.MonitorUserActivityAsync();
                await _stockMonitoringService.MonitorStockLevelsAsync();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Run every 1 minutes
            }
        }
    }
}
