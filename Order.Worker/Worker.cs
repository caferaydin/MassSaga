using MassTransit;

namespace Order.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBusControl _busControl;


        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker service started.");

            await _busControl.StartAsync(stoppingToken);

            stoppingToken.Register(() => _logger.LogInformation("Worker service stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            await _busControl.StopAsync(stoppingToken);
        }
    }
}
