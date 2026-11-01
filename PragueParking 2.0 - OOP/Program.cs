using PragueParking.Data;
using PragueParking_2._0___OOP;
using Spectre.Console;

{
    GarageConfig config = FileManager.LoadConfig();
    
    ParkingGarage myGarage = FileManager.LoadGarage();

    if (myGarage == null)
    {
        myGarage = new ParkingGarage(config.TotalSpots);
        FileManager.SaveGarage(myGarage);
    }

    ProgramUI ui = new ProgramUI(myGarage);

    bool isRunning = true;

    while (isRunning)
    {
        AnsiConsole.Clear();
        int userChoice = ui.ShowMainMenu();

        switch (userChoice)
        {
            case 1:
                ui.UIParkVehicle();
                break;
            case 2:
                ui.UIMoveVehicle();
                break;
            case 3:
                ui.UIRemoveVehicle();
                break;
            case 4:
                ui.UIFindVehicle();
                break;
            case 5:
                ui.UIShowMap();
                break;
            case 6:
                isRunning = false;
                AnsiConsole.MarkupLine("[bold red]Shutting down... Goodbye![/]");
                Thread.Sleep(2000);
                break;
        }
    }
}