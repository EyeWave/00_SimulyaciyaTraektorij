using System;
using System.Collections.Generic;
using System.Linq;

namespace Task
{
    internal class Program
    {
        internal const int MAGIC_INT = 21;

        public static void Main(string[] args)
        {
            var random = new Random();

            for (int i = 0; i < MAGIC_INT; i++)
            {
                var runners = new List<Runner>()
                {
                    new Runner(5, 5),
                    new Runner(10, 10),
                    new Runner(15, 15),
                    new Runner(random),
                };

                while (runners.FirstOrDefault(runner => runner.IsAlive) != null)
                {
                    runners.CompareRunners();
                    runners.ForEach(runner =>
                    {
                        runner.TryMove(random);
                        runner.ThrowXY((x, y) =>
                        {
                            Console.SetCursorPosition(x, y);
                            Console.Write(runners.IndexOf(runner));
                        });
                    });
                }

                Console.SetCursorPosition(0, 0);
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    internal class Runner : IDisposable
    {
        internal bool IsAlive { get; private set; }

        private int x;
        private int y;

        public Runner(int x, int y)
        {
            this.x = x;
            this.y = y;
            IsAlive = true;
        }

        public Runner(Random random) : this(random.Next(0, Program.MAGIC_INT), random.Next(0, Program.MAGIC_INT))
        {
        }

        internal bool Compare(Runner otherRunner)
        {
            return x == otherRunner.x && y == otherRunner.y;
        }

        internal bool TryMove(Random random)
        {
            if (IsAlive)
            {
                if (x == 0 && y == 0)
                {
                    IsAlive = false;
                    return false;
                }

                var prevX = x;
                var prevY = y;

                x = Math.Max(0, x + random.Next(-1, 1));
                y = Math.Max(0, y + random.Next(-1, 1));

                return prevX != x || prevY != y;
            }
            else
            {
                return false;
            }
        }

        internal void ThrowXY(Action<int, int> action)
        {
            if (IsAlive)
            {
                action?.Invoke(x, y);
            }
        }

        public void Dispose()
        {
            IsAlive = false;
        }
    }

    internal static class ExtensionsRunner
    {
        internal static void CompareRunners(this IEnumerable<Runner> runners)
        {
            var countRunners = runners.Count();

            for (int i = 0; i < countRunners; i++)
            {
                var runner = runners.ElementAt(i);

                for (int j = i + 1; j < countRunners; j++)
                {
                    var otherRunner = runners.ElementAt(j);

                    if ((runner.IsAlive || otherRunner.IsAlive) && runner.Compare(otherRunner))
                    {
                        runner.Dispose();
                        otherRunner.Dispose();
                    }
                }
            }
        }
    }
}