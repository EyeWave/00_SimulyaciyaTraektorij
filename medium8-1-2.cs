using System;
using System.Collections.Generic;
using System.Linq;

namespace Task
{
    internal class Program
    {
        internal const int MagicInt = 21;

        public static void Main(string[] args)
        {
            var random = new Random();

            for (var i = 0; i < MagicInt; i++)
            {
                var runners = new List<Runner>()
                {
                    Runner.FromPosition(5, 5),
                    Runner.FromPosition(10, 10),
                    Runner.FromPosition(15, 15),
                    Runner.FromRandom(random),
                };

                while (runners.FirstOrDefault(runner => runner.IsAlive) != null)
                {
                    runners
                        .CompareRunners()
                        .Where(runner => runner.IsAlive)
                        .Each(runner =>
                        {
                            runner.TryMove(random);
                            runner.DoActionAtCurrentPosition((x, y) =>
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

        private int _x;
        private int _y;

        internal static Runner FromPosition(int x, int y)
        {
            var runner = new Runner();
            runner._x = x;
            runner._y = y;
            return runner;
        }

        internal static Runner FromRandom(Random random)
        {
            return FromPosition(random.Next(0, Program.MagicInt), random.Next(0, Program.MagicInt));
        }

        private Runner()
        {
            IsAlive = true;
        }

        internal bool HaveSimilarPosition(Runner otherRunner)
        {
            return _x == otherRunner._x && _y == otherRunner._y;
        }

        internal bool TryMove(Random random)
        {
            if (_x == 0 && _y == 0)
            {
                IsAlive = false;
                return false;
            }

            var prevX = _x;
            var prevY = _y;

            _x = Math.Max(0, _x + random.Next(-1, 1));
            _y = Math.Max(0, _y + random.Next(-1, 1));

            return prevX != _x || prevY != _y;
        }

        internal void DoActionAtCurrentPosition(Action<int, int> action)
        {
            if (IsAlive)
            {
                action?.Invoke(_x, _y);
            }
        }

        public void Dispose()
        {
            IsAlive = false;
        }
    }

    internal static class Extensions
    {
        internal static IEnumerable<Runner> CompareRunners(this IEnumerable<Runner> runners)
        {
            var countRunners = runners.Count();

            for (int i = 0; i < countRunners; i++)
            {
                var runner = runners.ElementAt(i);

                for (int j = i + 1; j < countRunners; j++)
                {
                    var otherRunner = runners.ElementAt(j);

                    if ((runner.IsAlive || otherRunner.IsAlive) && runner.HaveSimilarPosition(otherRunner))
                    {
                        runner.Dispose();
                        otherRunner.Dispose();
                    }
                }
            }

            return runners;
        }

        internal static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Action<T> action = null)
        {
            if (action != null)
                foreach (var element in collection)
                    action.Invoke(element);
            return collection;
        }
    }
}
