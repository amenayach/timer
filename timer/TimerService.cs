namespace timer
{
    public static class TimerService
    {
        public static void Start(int seconds, Action<int, bool> onSecondsTick)
        {
            if (seconds <= 0)
            {
                onSecondsTick?.Invoke(seconds, false);
                return;
            }

            var timer = new System.Timers.Timer(1000);

            var leftSecond = seconds;

            timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                leftSecond--;

                if (leftSecond <= 0)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }

                if (leftSecond >= 0)
                {
                    onSecondsTick?.Invoke(leftSecond, leftSecond <= 0);
                }
            };

            Console.WriteLine("Starting the timer...");
            onSecondsTick?.Invoke(leftSecond, false);
            timer.Start();
        }
    }
}
