using System;
using System.Linq;

namespace timer
{
    static class Program
    {
        static void Main(string[] args)
        {
            var input = string.Empty;
            var additionalInput = string.Empty;
            
            if (args.Length == 0)
            {
                Console.WriteLine("Please enter the time to wait, like 2s, 3m or 1h");
                input = Console.ReadLine();

                var split = input.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                
                if (split.Length > 1)
                {
                    input = split[0];
                    additionalInput = split[1];
                }
            }
            else
            {
                input = args[0];

                if (args.Length > 1)
                {
                    additionalInput = args[1];
                }
            }

            var (seconds, isValid) = GetArgsSeconds(input);

            if (!isValid)
            {
                Console.WriteLine("Invalid input");
                return;
            }

            if (!string.IsNullOrWhiteSpace(additionalInput))
            {
                var (additionalSeconds, additionalIsValid) = GetArgsSeconds(additionalInput);

                if (!additionalIsValid)
                {
                    Console.WriteLine("Invalid input");
                    return;
                }

                seconds += additionalSeconds;
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
