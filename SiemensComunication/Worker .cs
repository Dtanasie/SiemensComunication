using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using S7.Net;
using SiemensComunication;
using Serilog; // Adaugă importul pentru Serilog

public class Worker
{
    private readonly SimulatedPLCConnection _plcConnection;
    private readonly ImageMover _imageMover;
    private object _lastValue;
    private readonly FolderSettings _folderSettings;
    private readonly PLCSettings _plcSettings;

    public Worker(IOptions<PLCSettings> plcSettings, IOptions<FolderSettings> folderSettings)
    {
        _plcSettings = plcSettings.Value;
        _folderSettings = folderSettings.Value;
       // CpuType cpuType = Enum.Parse<CpuType>(_plcSettings.CpuType);

        // Înlocuiește cu implementarea reală atunci când nu folosești simulatorul
        // _plcConnection = new PLCConnection(
        //    _plcSettings.IpAddress,
        //    cpuType,
        //    (short)_plcSettings.Rack,
        //    (short)_plcSettings.Slot
        //);

        _plcConnection = new SimulatedPLCConnection(); // Folosește simulatorul
        _imageMover = new ImageMover(_folderSettings.SourceFolder, _folderSettings.DestinationFolder);
    }

    public async Task RunAsync()
    {
        await EnsureInitialConnectionAsync();

        while (true)
        {
            try
            {
                await MonitorPlcConnectionAsync();
            }
            catch (Exception ex)
            {
                GlobalLogger.Logger.Error($"Error in main loop: {ex.Message}");
                await Task.Delay(5000); // Așteaptă 5 secunde înainte de a încerca din nou
            }
        }
    }

    private async Task EnsureInitialConnectionAsync()
    {
        while (true)
        {
            try
            {
                _plcConnection.Open();
                GlobalLogger.Logger.Information("Initial PLC connection opened.");
                return;
            }
            catch (Exception ex)
            {
                GlobalLogger.Logger.Error($"Error establishing initial PLC connection: {ex.Message}");
                await Task.Delay(5000); // Așteaptă 5 secunde înainte de a încerca din nou
            }
        }
    }

    private async Task MonitorPlcConnectionAsync()
    {
        bool wasConnected = false;

        while (true)
        {
            if (!_plcConnection.IsConnected())
            {
                if (wasConnected)
                {
                    GlobalLogger.Logger.Warning("PLC connection lost. Attempting to reconnect...");
                }

                await ReconnectPlcAsync();
                wasConnected = false;
            }
            else
            {
                if (!wasConnected)
                {
                    GlobalLogger.Logger.Information("Reconnected to PLC.");
                }

                await CheckPlcValueAsync();
                wasConnected = true;
            }

            await Task.Delay(1000); // Verificare la fiecare 1 secundă
        }
    }

    private async Task CheckPlcValueAsync()
    {
        try
        {
            var currentValue = _plcConnection.Read(_plcSettings.Address);

            if (currentValue != null && !currentValue.Equals(_lastValue))
            {
                GlobalLogger.Logger.Information($"Value changed: {currentValue}");

                if (currentValue is int intValue && intValue == 1)
                {
                    await MoveImagesAsync();
                }

                _lastValue = currentValue;
            }
        }
        catch (Exception ex)
        {
            GlobalLogger.Logger.Error($"Error reading from PLC: {ex.Message}");
        }
    }

    private async Task MoveImagesAsync()
    {
        try
        {
            await _imageMover.MoveImagesAsync();
        }
        catch (Exception ex)
        {
            GlobalLogger.Logger.Error($"Error moving images: {ex.Message}");
        }
    }

    private async Task ReconnectPlcAsync()
    {
        int maxRetries = _plcSettings.MaxReconnectAttempts;
        int attempt = 0;

        while (attempt < maxRetries)
        {
            try
            {
                _plcConnection.Close();
                _plcConnection.Open();
                GlobalLogger.Logger.Information("Reconnected to PLC.");
                return;
            }
            catch (Exception ex)
            {
                GlobalLogger.Logger.Error($"Reconnect attempt {attempt + 1} failed: {ex.Message}");
                attempt++;
                await Task.Delay(2000); // Așteaptă 2 secunde înainte de următoarea încercare
            }
        }

        GlobalLogger.Logger.Error("Failed to reconnect to PLC after multiple attempts.");
    }
}

