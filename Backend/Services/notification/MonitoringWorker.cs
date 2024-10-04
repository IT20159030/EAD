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
        private readonly OrderMonitoringService _orderMonitoringService;

        public MonitoringWorker(UserMonitoringService monitoringService, StockMonitoringService stockMonitoringService, OrderMonitoringService orderMonitoringService)
        {
            _monitoringService = monitoringService;
            _stockMonitoringService = stockMonitoringService;
            _orderMonitoringService = orderMonitoringService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _monitoringService.MonitorUserActivityAsync();
                await _stockMonitoringService.MonitorStockLevelsAsync();
                await _orderMonitoringService.MonitorOrderStatusesAsync();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Run every 1 minutes
            }
        }
    }
}
