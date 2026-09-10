namespace CodeGenerator
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
#pragma warning disable WFO5001 // “ип предназначен только дл€ оценки и может быть изменен или удален в будущих обновлени€х. „тобы продолжить, скройте эту диагностику.
            Application.SetColorMode(SystemColorMode.Dark);
#pragma warning restore WFO5001 // “ип предназначен только дл€ оценки и может быть изменен или удален в будущих обновлени€х. „тобы продолжить, скройте эту диагностику.
            Application.Run(new MainForm());
        }
    }
}