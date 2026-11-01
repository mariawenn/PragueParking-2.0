using PragueParking_2._0___OOP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PragueParking.Data
{
    public static class FileManager
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        private static readonly string FileName = "garage.json";
        private static readonly string PriceListFileName = "Pricelist.txt";
        private static readonly string ConfigFileName = "config.json";


        public static void SaveGarage(ParkingGarage garage)
        {
            string json = JsonSerializer.Serialize(garage, Options);
            File.WriteAllText(FileName, json);
        }

        public static ParkingGarage? LoadGarage()
        {
            if (!File.Exists(FileName))
            {
                return null;
            }

            string json = File.ReadAllText(FileName);
            return JsonSerializer.Deserialize<ParkingGarage>(json, Options);
        }

        public static Dictionary<string, decimal> LoadPriceList()
        {
            var price = new Dictionary<string, decimal>();

            if (!File.Exists(PriceListFileName))
            {
                price["Car"] = 20m;
                price["MC"] = 10m;
                return price;
            }

            foreach (string line in File.ReadLines(PriceListFileName))
            {
                string trimmedLine = line.Trim();

                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                {
                    continue;
                }

                string[] parts = trimmedLine.Split('=');
                if (parts.Length == 2)
                {
                    string VehicleType = parts[0].Trim();
                    string priceText = parts[1].Trim();

                    if (decimal.TryParse(priceText, out decimal priceValue))
                    {
                        price[VehicleType] = priceValue;
                    }
                }
            }
            return price;
        }

        public static GarageConfig LoadConfig()
        {
            if (!File.Exists(ConfigFileName))
            {
                GarageConfig defaultConfig = new GarageConfig();
                string json = JsonSerializer.Serialize(defaultConfig, Options);
                File.WriteAllText(ConfigFileName, json);
                return defaultConfig;
            }

            string loadedJson = File.ReadAllText(ConfigFileName);
            return JsonSerializer.Deserialize<GarageConfig>(loadedJson, Options) ?? new GarageConfig();
        }
    }
}