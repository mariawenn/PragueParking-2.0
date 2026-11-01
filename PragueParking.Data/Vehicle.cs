using System.Text.Json.Serialization;

namespace PragueParking_2._0___OOP
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(Car), "Car")]
    [JsonDerivedType(typeof(MC), "MC")]
    public class Vehicle
    {
        public string RegNumber { get; set; }
        public string VehicleType { get; set; }
        public int VehicleSize { get; set; }
        public DateTime ArrivalTime { get; set; }

        public Vehicle()
        {
        }

        public Vehicle(string regNumber, string vehicleType, int vehicleSize, DateTime arrivalTime)
        {
            RegNumber = regNumber;
            VehicleType = vehicleType;
            VehicleSize = vehicleSize;
            ArrivalTime = arrivalTime;
        }
    }
}