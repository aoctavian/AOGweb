using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AOGweb.Models
{
    public class Device1 : Device
    {
        public int ERROR { get; set; }

        public string Temperature { get; set; }

        public string Humidity { get; set; }
    }
}
