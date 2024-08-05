using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using S7.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    //  private PLCConnection _plcConnection;
    private SimulatedPLCConnection _plcConnection; //foloseste simulatorul 
    private ImageMover _imageMover;
    private readonly string _plcAddress = "DB1.DBD0";
    private object _lastValue;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        //   _plcConnection = new PLCConnection("192.168.0.1", CpuType.S71200, 0, 1);
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
                _logger.LogInformation("Connected to PLC.");

                var currentValue = _plcConnection.Read(_plcAddress);

                if (currentValue != null && !currentValue.Equals(_lastValue))
                {
                    _logger.LogInformation($"Value changed: {currentValue}");

                    if (currentValue is int intValue && intValue == 1)
                    {
                        _imageMover.MoveImages();
                    }

                    _lastValue = currentValue;
                }
            }
            else
            {
                _logger.LogError("Failed to connect to PLC.");
            }

            await Task.Delay(1000, stoppingToken); // Verificare la fiecare 1 secundă
        }

        _plcConnection.Close();
    }
}
