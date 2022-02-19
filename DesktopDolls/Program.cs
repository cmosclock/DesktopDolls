using System;

namespace DesktopDolls
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new ShimejiGflGame();
            game.Run();
        }
    }
}
