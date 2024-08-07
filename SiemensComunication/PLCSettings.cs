using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiemensComunication
{
    public class PLCSettings
    {
        public string IpAddress { get; set; }
        public string CpuType { get; set; }
        public int Rack { get; set; }
        public int Slot { get; set; }
        public string Address { get; set; }
        public int MaxReconnectAttempts { get; set; } // Adaugă această proprietate
    }
}
