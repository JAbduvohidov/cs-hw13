using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace homework13
{
    internal static class Program
    {
        private static async Task Main()
        {
            const int height = 30;
            const int width = 50;
            const int spawnAmount = 10000;
            var rand = new Random();
            var tasks = new List<Task>();

            Console.Clear();

            DrawBorders(width + 10, height);

            Console.CursorVisible = false;

            for (var i = 0; i < spawnAmount; i++)
            {
                Thread.Sleep(rand.Next(0, 220));
                Parallel.For(0, rand.Next(2, 10), (x) =>
                {
                    var position = rand.Next(1, width);
                    tasks.Add(Task.Run(() =>
                    {
                        Thread.Sleep(x * rand.Next(0, 90));
                        Raindrop.Fall(width, height, rand.Next(5, 10), rand.Next(90, 120), position);
                    }));
                });
            }

            await Task.WhenAll(tasks);
        }

        private static void DrawBorders(int width, int height)
        {
            Console.SetCursorPosition(0, 0);
            fmt.Println(
                $"┌{new string('─', (width - 11) / 2)} C O P Y {new string('─', (width - 10) / 2)}┐",
                ConsoleColor.Green);

            for (var i = 1; i < height; i++)
            {
                fmt.Println($"|{new string(' ', width - 2)}|", ConsoleColor.Green);
            }

            fmt.Println($"└{new string('─', (width - 12) / 2)} P A S T E {new string('─', (width - 13) / 2)}┘", ConsoleColor.Green);
        }
    }

    internal static class Raindrop
    {
        private static readonly List<string> CharactersList = new()
        {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
            "V", "W", "X", "Y", "Z", "@", "!", "#", "$", "%", "&", "8"
        };

        private static readonly object Locker = new();

        public static void Fall(int width, int height, int length, int speed, int position)
        {
            for (var i = 1; i < height + length; i++)
            {
                if (i <= length)
                {
                    DrawRaindrop(GenerateNewRaindrop(i), position, i);

                    Thread.Sleep(speed);

                    ClearRaindrop(position, i, i);
                    continue;
                }

                if (i >= height - length)
                {
                    DrawRaindrop(GenerateNewRaindrop(height - i), position, i);

                    Thread.Sleep(speed);

                    ClearRaindrop(position, i, height - i);
                    continue;
                }

                DrawRaindrop(GenerateNewRaindrop(length), position, i);

                Thread.Sleep(speed);

                ClearRaindrop(position, i, length);
            }
        }

        private static List<string> GenerateNewRaindrop(int length)
        {
            var list = new List<string>();
            var rnd = new Random();
            for (var i = 0; i < length; i++)
            {
                list.Add(CharactersList[rnd.Next(0, CharactersList.Count)]);
            }

            return list;
        }

        private static void DrawRaindrop(List<string> raindrop, int left, int top)
        {
            foreach (var item in raindrop)
            {
                Monitor.Enter(Locker);

                Console.SetCursorPosition(left, top);
                top++;
                var currentColor = Console.ForegroundColor;

                if (raindrop.FindIndex(i => i == item) < raindrop.Count - 1)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                if (raindrop.FindIndex(i => i == item) < raindrop.Count - 2)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                }

                if (raindrop.FindIndex(i => i == item) < raindrop.Count - 5)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                Console.Write(item);
                Console.ForegroundColor = currentColor;

                Monitor.Exit(Locker);
            }
        }

        private static void ClearRaindrop(int left, int top, int length)
        {
            Monitor.Enter(Locker);

            var (currentLeft, currentTop) = Console.GetCursorPosition();
            Console.SetCursorPosition(left, top);

            for (var i = 0; i < length; i++)
                fmt.Print(" ");

            Console.SetCursorPosition(currentLeft, currentTop);

            Monitor.Exit(Locker);
        }
    }
}