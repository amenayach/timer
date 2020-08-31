using System;
using System.Linq;

namespace timer
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Invalid input");
                return;
            }

            var (seconds, isValid) = GetArgsSeconds(args[0]);

            if (!isValid)
            {
                Console.WriteLine("Invalid input");
                return;
            }

            var done = false;

            TimerService.Start(seconds, (leftSeconds, completed) =>
            {
                Display(leftSeconds);

                if (completed)
                {
                    SoundService.PlayAlarm();
                    done = true;
                }
            });

            while (!done) ;

            Console.WriteLine();
            Console.WriteLine("Please hit enter to exit");
            Console.ReadLine();
        }

        private static (int seconds, bool valid) GetArgsSeconds(string arg)
        {
            var type = arg.Last();

            var numberText = arg.Substring(0, arg.Length - 1);
            if (!int.TryParse(numberText, out int number))
            {
                return (-1, false);
            }

            switch (type)
            {
                case 'h':
                    return (number * 3600, true);
                case 'm':
                    return (number * 60, true);
                case 's':
                default:
                    return (number, true);
            }
        }

        private static void Display(int leftSeconds)
        {
            var timeSpan = new TimeSpan(0, 0, leftSeconds);
            Console.Write($"\r{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}        ");
        }
    }
}
