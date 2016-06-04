using System;

namespace SharpECS.Samples
{
#if WINDOWS || LINUX
    internal static class Launcher
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new SampleGame())
            {
                game.Run();
            }
        }
    }
#endif
}