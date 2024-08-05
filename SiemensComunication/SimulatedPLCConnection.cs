public class SimulatedPLCConnection
{
    private object _currentValue;
    private static readonly Random _random = new Random();

    public SimulatedPLCConnection()
    {
        // Inițializare cu valoarea 0
        _currentValue = 0;
    }

    public void Open()
    {
        // Simulează deschiderea conexiunii
        Console.WriteLine("Simulated PLC connection opened.");
    }

    public void Close()
    {
        // Simulează închiderea conexiunii
        Console.WriteLine("Simulated PLC connection closed.");
    }

    public bool IsConnected()
    {
        // Simulează conexiunea
        return true;
    }

    public object Read(string address)
    {
        // Simulează schimbarea valorii
        // Simulează primirea valorii 1
        _currentValue = _random.Next(0, 2); // Random între 0 și 1
        return _currentValue;
    }
}
