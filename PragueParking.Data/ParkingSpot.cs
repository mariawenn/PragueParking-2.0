namespace PragueParking_2._0___OOP

{
    public class ParkingSpot
    {
        public int SpotNumber { get; set; }
        public List<Vehicle> Vehicles { get; set; }
        public ParkingSpot(int spotNumber)
        {
            SpotNumber = spotNumber;
            Vehicles = new List<Vehicle>();
        }

        public int UsedSpace()
        {
            int usedSpace = 0;

            foreach (Vehicle vehicle in Vehicles)
            {
                usedSpace = usedSpace + vehicle.VehicleSize;
            }
            return usedSpace;
        }

        public bool CanPark(Vehicle vehicle)
        {
            int totalSpotSpace = 4;
            int usedSpace = UsedSpace();

            if (usedSpace + vehicle.VehicleSize <= totalSpotSpace)
            {
                return true;
            }

            return false;
        }
    }
}