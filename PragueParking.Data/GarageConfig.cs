using System;
using System.Collections.Generic;
using System.Text;

namespace PragueParking.Data
{
    public class GarageConfig
    {
        public int TotalSpots { get; set; } = 100;
        public int SpotCapacity { get; set; } = 4;
        public int CarSize { get; set; } = 4;
        public int McSize { get; set; } = 2;
    }
}
