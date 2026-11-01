using Spectre.Console;
using PragueParking_2._0___OOP;
using PragueParking.Data;

namespace PragueParking_2._0___OOP
{
    internal class ProgramUI
    {
        private ParkingGarage garage;

        public ProgramUI(ParkingGarage loadedGarage)
        {
            garage = loadedGarage;
        }

        public int ShowMainMenu()
        {
            AnsiConsole.Write(new FigletText("PRAGUE PARKING").Centered().Color(Color.RoyalBlue1));
            AnsiConsole.Write(new Rule("[RoyalBlue1]v2.0 - OOP Edition[/]").Centered());
            AnsiConsole.WriteLine();

            List<string> choice = [
                "1. Park vehicle",
                "2. Move vehicle",
                "3. Remove vehicle",
                "4. Search for a vehicle",
                "5. Show parking map",
                "6. Exit program"];

            string menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[bold white]Select an option below:[/]")
                .AddChoices(choice));

            int choiceIndex = choice.IndexOf(menuChoice);
            return choiceIndex + 1;
        }

        public void UIParkVehicle()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold white]PARK VEHICLE[/]").LeftJustified().RuleStyle("grey"));
            AnsiConsole.WriteLine();

            string choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("What type of vehicle do you want to park?")
                .AddChoices(["Car", "MC"]));

            string regNumber = AnsiConsole.Ask<string>("Enter vehicle registration number:");

            ParkingSpot existingSpot = garage.FindVehicle(regNumber);
            if (existingSpot != null)
            {
                ErrorMessage($"Vehicle {regNumber.ToUpper()} is already parked at spot {existingSpot.SpotNumber}");
                ReturnToMainMenu();
                return;
            }

            Vehicle newVehicle = null;

            if (choice == "Car")
            {
                newVehicle = new Car(regNumber);
            }
            else
            {
                newVehicle = new MC(regNumber);
            }

            bool parkingSuccess = garage.ParkVehicle(newVehicle);

            if (parkingSuccess == true)
            {
                ParkingSpot spot = garage.FindVehicle(regNumber);
                var panelContent = new Markup($"[green]Vehicle [bold]{regNumber.ToUpper()}[/] has been parked![/]\n[white]Assigned to spot:[/] " +
                    $"[yellow bold]{spot.SpotNumber}[/]");
                AnsiConsole.Write(new Panel(panelContent).Header("[bold green] SUCCESS [/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(Color.Green));
            }
            else
            {
                ErrorMessage("Sorry, the garage is full!");
            }

            ReturnToMainMenu();
        }

        public void UIMoveVehicle()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold white]MOVE VEHICLE[/]").LeftJustified().RuleStyle("grey"));
            AnsiConsole.WriteLine();

            string regNumber = AnsiConsole.Ask<string>("Enter vehicle registration number:");

            ParkingSpot currentSpot = garage.FindVehicle(regNumber);
            if (currentSpot == null)
            {
                ErrorMessage($"Vehicle {regNumber.ToUpper()} was not found!");
                ReturnToMainMenu();
                return;
            }

            int newSpot = AnsiConsole.Ask<int>("Enter new parking spot number (1-100):");
            bool moveSuccess = garage.MoveVehicle(regNumber, newSpot);

            if (moveSuccess == true)
            {
                AnsiConsole.MarkupLine($"[green]\nVehicle {regNumber.ToUpper()} has been moved to spot {newSpot}![/]");
            }
            else
            {
                ErrorMessage("Could not move the vehicle, please try again!");
            }

            ReturnToMainMenu();
        }

        public void UIRemoveVehicle()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold white]REMOVE VEHICLE[/]").LeftJustified().RuleStyle("grey"));
            AnsiConsole.WriteLine();

            string regNum = AnsiConsole.Ask<string>("Enter vehicle registration number:");

            ParkingSpot spot = garage.FindVehicle(regNum);

            if (spot != null)
            {
                Vehicle vehicleToRemove = null;

                foreach (Vehicle vehicle in spot.Vehicles)
                {
                    if (vehicle.RegNumber.ToUpper() == regNum.ToUpper())
                    {
                        vehicleToRemove = vehicle;
                        break;
                    }
                }

                TimeSpan duration = DateTime.Now - vehicleToRemove.ArrivalTime;
                decimal price = garage.CalculatePrice(vehicleToRemove);

                bool removeSuccess = garage.RemoveVehicle(spot, vehicleToRemove);

                if (removeSuccess == true)
                {
                    var receiptGrid = new Grid().AddColumns(2);
                    receiptGrid.AddRow(new Markup("[white]Vehicle:[/]"), new Markup($"[yellow bold]{regNum}[/]"));
                    receiptGrid.AddRow(new Markup("[white]Removed From Spot:[/]"), new Markup($"[yellow]{spot.SpotNumber}[/]"));
                    receiptGrid.AddRow(new Markup("[white]Duration:[/]"), new Markup($"[yellow]{duration.Days}d {duration.Hours}h {duration.Minutes}m[/]"));
                    receiptGrid.AddRow(new Markup("[white]Total Price:[/]"), new Markup($"[green bold]{price} CZK[/]"));

                    AnsiConsole.Write(new Panel(receiptGrid).Header("[bold green] RECEIPT [/]").Border(BoxBorder.Rounded).BorderColor(Color.Green));
                }
                else
                {
                    ErrorMessage("Could not remove the vehicle, please try again");
                }
            }
            else
            {
                ErrorMessage($"Vehicle {regNum.ToUpper()} was not found!");
            }

            ReturnToMainMenu();
        }

        public void UIFindVehicle()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[bold white]SEARCH FOR VEHICLE[/]").LeftJustified().RuleStyle("grey"));
            AnsiConsole.WriteLine();

            string searchReg = AnsiConsole.Ask<string>("Enter registration number:");

            ParkingSpot foundSpot = garage.FindVehicle(searchReg);

            if (foundSpot != null)
            {
                AnsiConsole.MarkupLine($"\n[bold green]Vehicle found on spot number [/][bold yellow]{foundSpot.SpotNumber}[/]");
            }
            else
            {
                ErrorMessage($"Vehicle {searchReg.ToUpper()} was not found!");
            }

            ReturnToMainMenu();
        }

        public void UIShowMap()
        {
            int emptySpots = 0;
            int partialSpots = 0;
            int fullSpots = 0;
            int totalCars = 0;
            int totalMcs = 0;

            Table mapTable = new Table()
            {
                Title = new TableTitle("[bold white]GARAGE OVERVIEW[/]"),
                Border = TableBorder.Rounded
            };

            mapTable.HideHeaders();

            for (int col = 0; col < 10; col++)
            {
                mapTable.AddColumn(new TableColumn("").Centered());
            }

            for (int row = 0; row < 10; row++)
            {
                Markup[] rowCells = new Markup[10];

                for (int col = 0; col < 10; col++)
                {
                    int index = (col * 10) + row;

                    if (index >= garage.Spots.Count)
                    {
                        break;
                    }

                    ParkingSpot spot = garage.Spots[index];
                    string spotNum = spot.SpotNumber.ToString("D2");

                    string type = "";
                    string reg1 = "";
                    string reg2 = "";
                    int count = 0;

                    foreach (Vehicle v in spot.Vehicles)
                    {
                        count++;
                        if (count == 1)
                        {
                            type = v.VehicleType;
                            reg1 = v.RegNumber.ToUpper();
                        }
                        else if (count == 2)
                        {
                            reg2 = v.RegNumber.ToUpper();
                        }

                        if (v.VehicleType == "Car") { totalCars++; }
                        else if (v.VehicleType == "MC") { totalMcs++; }
                    }

                    if (spot.Vehicles.Count == 0)
                    {
                        emptySpots++;
                        rowCells[col] = new Markup($"[black on green] {spotNum} [/]");
                    }
                    else if (spot.Vehicles.Count == 1 && type == "MC")
                    {
                        partialSpots++;
                        rowCells[col] = new Markup($"[black on yellow] {spotNum}: {reg1} [/]");
                    }
                    else
                    {
                        fullSpots++;
                        if (type == "Car")
                        {
                            rowCells[col] = new Markup($"[white on red] {spotNum}: {reg1} [/]");
                        }
                        else
                        {
                            rowCells[col] = new Markup($"[white on red] {spotNum}:{reg1}/{reg2} [/]");
                        }
                    }
                }

                mapTable.AddRow(rowCells);
            }

            Grid statusGrid = new Grid().AddColumns(4);
            statusGrid.AddRow(
                new Markup($"[green]Empty: {emptySpots}[/]"),
                new Markup($"[yellow]Partial: {partialSpots}[/]"),
                new Markup($"[red]Full: {fullSpots}[/]"),
                new Markup($"[aqua]Cars: {totalCars} | MCs: {totalMcs}[/]")
            );

            Panel statusPanel = new Panel(statusGrid)
            {
                Header = new PanelHeader("[bold white]GARAGE STATUS[/]"),
                Border = BoxBorder.Square
            };
            statusPanel.BorderColor(Color.Grey);

            AnsiConsole.Clear();
            AnsiConsole.Write(Align.Center(statusPanel));
            AnsiConsole.WriteLine();
            AnsiConsole.Write(Align.Center(mapTable));
            AnsiConsole.WriteLine();

            ReturnToMainMenu();
        }

        void ErrorMessage(string message)
        {
            AnsiConsole.Write(new Panel(new Markup($"[red]{message}[/]")).Header("[bold red] ERROR [/]")
                .Border(BoxBorder.Rounded).
                BorderColor(Color.Red));
        }

        public void ReturnToMainMenu()
        {
            AnsiConsole.MarkupLine("[bold]\nPress any key to return to the main menu...[/]");
            Console.ReadKey(true);
        }
    }
}