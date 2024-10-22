namespace NetProxy
{
    internal static class Program
    {
        private static System.Timers.Timer ﾠ300;
        private static bool s_showSparta = true;

        static Program()
        {
            ﾠ300 = new System.Timers.Timer(4000);
            ﾠ300.AutoReset = true;
            ﾠ300.Start();
        }

        private static void Main(string[] args)
        {
            System.Console.Title = "Postgres profiler";
            Profile.StartProxy();
        }
    }
}