using System;
using System.Threading;

namespace DeticatedServer
{
    class Program
    {
        private static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.Title = "Among EU Server";
            isRunning = true;

            Thread mainThread = new Thread(() =>
            {
                Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks per second.");
                DateTime _nextLoop = DateTime.Now;

                while (isRunning)
                {
                    while (_nextLoop < DateTime.Now)
                    {
                        // If the time for the next loop is in the past, aka it's time to execute another tick
                        /*foreach (ServerClient _client in Server.Clients.Values)
                        {
                            
                        }*/

                        ThreadManager.UpdateMain();
                        foreach (ServerClient client in Server.Clients.Values)
                            client.Update();

                        _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK); // Calculate at what point in time the next tick should be executed

                        if (_nextLoop > DateTime.Now)
                        {
                            // If the execution time for the next tick is in the future, aka the server is NOT running behind
                            Thread.Sleep(_nextLoop - DateTime.Now); // Let the thread sleep until it's needed again.
                        }
                    }
                }
            });
            mainThread.Start();

            Server.Start(10, 42069);
        }
    }
}