using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AOGweb.Models
{
    public abstract class Device
    {
        [Key]
        public string MAC { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime LastUpdate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime SettingTime { get; set; }

        [StringLength(50, ErrorMessage = "Room name cannot be longer than 50 characters.")]
        public string Room { get; set; }

        [StringLength(50, ErrorMessage = "Device name cannot be longer than 50 characters.")]
        public string Name { get; set; }
    }
}
