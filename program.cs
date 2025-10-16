using Controller;
using View;

class Program
{
    static void Main()
    {
        var view = new ConsoleInterface();
        var controller = new MainController(view);
        controller.Run();
    }
}
