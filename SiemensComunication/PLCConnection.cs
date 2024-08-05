using S7.Net;

public class PLCConnection
{
    private Plc plc;

    public PLCConnection(string ipAddress, CpuType cpuType, short rack, short slot)
    {
        plc = new Plc(cpuType, ipAddress, rack, slot);
    }

    public void Open()
    {
        plc.Open();
    }

    public void Close()
    {
        plc.Close();
    }

    public bool IsConnected()
    {
        return plc.IsConnected;
    }

    public object Read(string address)
    {
        return plc.Read(address);
    }
}
