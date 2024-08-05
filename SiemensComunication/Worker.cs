using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using S7.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private PLCConnection _plcConnection;
    private ImageMover _imageMover;
    private readonly string _plcAddress = "DB1.DBD0";

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _plcConnection = new PLCConnection("192.168.0.1", CpuType.S71200, 0, 1);
        _imageMover = new ImageMover(@"C:\sourceFolder", @"\\server\destinationFolder");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _plcConnection.Open();

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_plcConnection.IsConnected())
            {
                _logger.LogInformation("Connected to PLC.");

                var value = _plcConnection.Read(_plcAddress);

                if (value is int intValue && intValue == 1)
                {
                    _imageMover.MoveImages();
                }
            }
            else
            {
                _logger.LogError("Failed to connect to PLC.");
            }

            await Task.Delay(1000, stoppingToken); // Citire la fiecare 1 secundă
        }

        _plcConnection.Close();
    }
}
