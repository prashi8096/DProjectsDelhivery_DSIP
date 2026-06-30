using Delhivery.ConsoleApp.Menus;
using Delhivery.Data.Interfaces;
using Delhivery.Data.Repository;
using Delhivery.Data.Services;

class Program
{
    static void Main(string[] args)
    {
        IShipmentRepository repository = new ShipmentRepository();
        ShipmentService shipmentService = new ShipmentService(repository);

        MainMenu menu = new MainMenu(shipmentService);
        menu.Start();
    }
}