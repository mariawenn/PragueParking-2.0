namespace PragueParking_2._0___OOP

{
    public class ParkingGarage
    {
        public List<ParkingSpot> Spots { get; set; }

        public ParkingGarage(int totalSpots)
        {
            Spots = new List<ParkingSpot>();

            for (int i = 1; i <= totalSpots; i++)
            {
                ParkingSpot newSpot = new ParkingSpot(i);
                Spots.Add(newSpot);
            }
        }

        public ParkingGarage()
        {
        }

        public int FindEmptySpot(Vehicle vehicleToPark)
        {
            int spotSize = vehicleToPark.VehicleSize;

            for (int i = 0; i < Spots.Count; i++)
            {
                ParkingSpot currentSpot = Spots[i];

                if (currentSpot.CanPark(vehicleToPark) == true)
                {
                    return currentSpot.SpotNumber;
                }
            }
            return -1;
        }

        public bool ParkVehicle(Vehicle vehicleToPark)
        {
            int findSpot = FindEmptySpot(vehicleToPark);

            if (findSpot == -1)
            {
                return false;
            }

            int listIndex = findSpot - 1;
            ParkingSpot foundSpot = Spots[listIndex];

            foundSpot.Vehicles.Add(vehicleToPark);

            PragueParking.Data.FileManager.SaveGarage(this);
            return true;
        }

        public ParkingSpot FindVehicle(string regNum)
        {
            for (int i = 0; i < Spots.Count; i++)
            {
                ParkingSpot currentSpot = Spots[i];

                foreach (Vehicle vehicle in currentSpot.Vehicles)
                {
                    if (vehicle.RegNumber.ToUpper() == regNum.ToUpper())
                    {
                        return currentSpot;
                    }
                }
            }
            return null;
        }

        public bool RemoveVehicle(ParkingSpot spot, Vehicle vehicle)
        {
            if (spot == null || vehicle == null)
            {
                return false;
            }
            Vehicle foundVehicle = null;

            foreach (Vehicle v in spot.Vehicles)
            {
                if (v.RegNumber.ToUpper() == vehicle.RegNumber.ToUpper())
                {
                    foundVehicle = v;
                    break;
                }
            }
            if (foundVehicle != null)
            {
                spot.Vehicles.Remove(foundVehicle);
                PragueParking.Data.FileManager.SaveGarage(this);
                return true;
            }
            return false;
        }

        public bool MoveVehicle(string regNum, int newSpotNum)
        {
            ParkingSpot currentSpot = FindVehicle(regNum);

            if (currentSpot == null)
            {
                return false;
            }

            if (newSpotNum < 1 || newSpotNum > 100)
            {
                return false;
            }

            ParkingSpot newSpot = Spots[newSpotNum - 1];

            Vehicle vehicleToMove = null;

            foreach (Vehicle vehicle in currentSpot.Vehicles)
            {
                if (vehicle.RegNumber.ToUpper() == regNum.ToUpper())
                {
                    vehicleToMove = vehicle;
                    break;
                }
            }

            if (newSpot.CanPark(vehicleToMove) == true) 
            {
                currentSpot.Vehicles.Remove(vehicleToMove);
                newSpot.Vehicles.Add(vehicleToMove);

                PragueParking.Data.FileManager.SaveGarage(this);

                return true;
            }

            return false;
        }

        public decimal CalculatePrice(Vehicle vehicle)
        {
            TimeSpan timeParked = DateTime.Now - vehicle.ArrivalTime;

            if (timeParked.TotalMinutes < 10)
            {
                return 0m;
            }

            int hours = (int)Math.Ceiling(timeParked.TotalHours);

            Dictionary<string, decimal> prices = PragueParking.Data.FileManager.LoadPriceList();
            if (vehicle is MC)
            {
                decimal mcPrice = prices.ContainsKey("MC") ? prices["MC"] : 10m;
                return hours * mcPrice;
            }
            else
            {
                decimal carPrice = prices.ContainsKey("Car") ? prices["Car"] : 20m;
                return hours * carPrice;
            }
        }
    }
}