namespace DominoForm.Controller
{
    internal static class Program
    {
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            new MenuController();
        }
    }
}