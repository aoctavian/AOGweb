using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AOGweb.Models
{
    public class UserDevices
    {
        public int UserID { get; set; }

        public string DeviceMAC { get; set; }

        public int OwnerID { get; set; }
    }
}
