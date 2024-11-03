using System.Text.RegularExpressions;

namespace timer
{
    static class Program
    {
        static void Main(string[] args)
        {
            string firstDuration = string.Empty;
            string secondDuration = string.Empty;
            string message = string.Empty;

            // If no arguments provided, ask for user input
            if (args.Length == 0)
            {
                Console.WriteLine("Enter the first duration (e.g., 4s, 15m, 1h):");
                firstDuration = Console.ReadLine()?.Trim() ?? string.Empty;

                Console.WriteLine("Enter the second duration (optional, press Enter to skip):");
                secondDuration = Console.ReadLine()?.Trim() ?? string.Empty;

                Console.WriteLine("Enter a message (optional, press Enter to skip):");
                string userMessage = Console.ReadLine()?.Trim() ?? string.Empty;
                if (!string.IsNullOrEmpty(userMessage))
                {
                    message = userMessage;
                }
            }
            else
            {
                try
                {
                    (firstDuration, secondDuration, message) = ParseCommandLineArgs(args);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ShowUsage();
                    return;
                }
            }

            // Validate and process the durations
            try
            {
                var seconds = ParseDuration(firstDuration);
                int additionalSeconds = 0;

                if (!string.IsNullOrEmpty(secondDuration))
                {
                    additionalSeconds = ParseDuration(secondDuration);
                }

                // starting the timer
                var done = false;
                var endsAt = DateTime.Now.AddSeconds(seconds);

                TimerService.Start(seconds + additionalSeconds, message, (leftSeconds, completed) =>
                {
                    Display(leftSeconds, endsAt);

                    if (completed)
                    {
                        SoundService.PlayAlarm();
                        done = true;
                    }
                });

                while (!done) ;

                Console.WriteLine();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ShowUsage();
            }

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

        private static void Display(int leftSeconds, DateTime endsAt)
        {
            var timeSpan = new TimeSpan(0, 0, leftSeconds);
            Console.Write($"\r{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00} (ends at {endsAt:yyyy-MM-dd HH:mm:ss})       ");
        }

        static (string firstDuration, string secondDuration, string message) ParseCommandLineArgs(string[] args)
        {
            string firstDuration = string.Empty;
            string secondDuration = string.Empty;
            string message = string.Empty;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-m" && i + 1 < args.Length)
                {
                    message = args[i + 1];
                    i++; // Skip the next argument as it's the message
                }
                else if (string.IsNullOrEmpty(firstDuration))
                {
                    firstDuration = args[i];
                }
                else if (string.IsNullOrEmpty(secondDuration))
                {
                    secondDuration = args[i];
                }
                else
                {
                    throw new ArgumentException("Too many arguments provided.");
                }
            }

            if (string.IsNullOrEmpty(firstDuration))
            {
                throw new ArgumentException("First duration is required.");
            }

            return (firstDuration, secondDuration, message);
        }

        static int ParseDuration(string duration)
        {
            if (string.IsNullOrEmpty(duration))
            {
                throw new ArgumentException("Duration cannot be empty.");
            }

            var regex = new Regex(@"^(\d+)(s|m|h)$");
            var match = regex.Match(duration);

            if (!match.Success)
            {
                throw new ArgumentException($"Invalid duration format: {duration}. Expected format: number followed by s, m, or h (e.g., 4s, 15m, 1h)");
            }

            int value = int.Parse(match.Groups[1].Value);
            string unit = match.Groups[2].Value;

            return unit switch
            {
                "s" => value,
                "m" => value * 60,
                "h" => value * 3600,
                _ => 0
            };
        }

        static void ShowUsage()
        {
            Console.WriteLine("\nUsage:");
            Console.WriteLine("  Program.exe <duration1> [duration2] [-m \"message\"]");
            Console.WriteLine("\nExamples:");
            Console.WriteLine("  Program.exe 4s");
            Console.WriteLine("  Program.exe 15m 1h");
            Console.WriteLine("  Program.exe 30s -m \"Custom message\"");
            Console.WriteLine("\nDuration format: number followed by s (seconds), m (minutes), or h (hours)");
        }
    }
}
