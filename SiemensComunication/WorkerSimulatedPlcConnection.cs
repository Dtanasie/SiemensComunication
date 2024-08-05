using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class WorkerSimulatedPlcConnection : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private SimulatedPLCConnection _plcConnection;
    private ImageMover _imageMover;
    private readonly string _plcAddress = "DB1.DBD0";
    private object _lastValue;

    public WorkerSimulatedPlcConnection(ILogger<Worker> logger)
    {
        _logger = logger;
        _plcConnection = new SimulatedPLCConnection(); // Folosește simulatorul
        _imageMover = new ImageMover(@"C:\Photos", @"C:\Destinatie");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _plcConnection.Open();

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_plcConnection.IsConnected())
            {
                _logger.LogInformation("Connected to simulated PLC.");

                var currentValue = _plcConnection.Read(_plcAddress);

                if (currentValue != null && !currentValue.Equals(_lastValue))
                {
                    _logger.LogInformation($"Simulated value changed: {currentValue}");

                    if (currentValue is int intValue && intValue == 1)
                    {
                        _imageMover.MoveImages();
                    }

                    _lastValue = currentValue;
                }
            }
            else
            {
                _logger.LogError("Failed to connect to simulated PLC.");
            }

            await Task.Delay(1000, stoppingToken); // Verificare la fiecare 1 secundă
        }

        _plcConnection.Close();
    }
}
