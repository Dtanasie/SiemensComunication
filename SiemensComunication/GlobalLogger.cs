using Serilog;

public static class GlobalLogger
{
    public static ILogger Logger { get; private set; }

    public static void InitializeLogger()
    {
        Logger = Log.Logger;
    }
}