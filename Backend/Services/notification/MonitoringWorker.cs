/**
* This class is responsible for running the monitoring services in the background.
* It inherits from the BackgroundService class in the Microsoft.Extensions.Hosting namespace.
* The BackgroundService class is a base class for implementing a long-running IHostedService.
* The MonitoringWorker class has a constructor that takes instances of the UserMonitoringService, StockMonitoringService, and OrderMonitoringService classes.
* The ExecuteAsync method is overridden to run the monitoring services at regular intervals.
* The MonitorUserActivityAsync, MonitorStockLevelsAsync, and MonitorOrderStatusesAsync methods are called to monitor user activity, stock levels, and order statuses, respectively.
* The Task.Delay method is used to wait for a specified period before running the monitoring services again.
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
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
