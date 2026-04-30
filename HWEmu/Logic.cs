using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HWEmu
{
    public static class Logic
    {
        public static List<Connector> ConnectorStateQueue = new List<Connector>();
        public static CancellationTokenSource? _logicCts;
        public static Task? _logicTask;

        public static bool PointCloseEnough(Vector2 a, Vector2 b)
        {
            if (Raylib.CheckCollisionPointCircle(a, b, 20f))
            {
                return true;
            }

            return false;
        }

        public static void StartStateUpdateLoop()
        {
            _logicCts = new CancellationTokenSource();
            _logicTask = Task.Run(() => StateCalcLoop(_logicCts.Token));
            Console.WriteLine("Logic started");
        }

        public static void StopStateUpdates()
        {
            var cts = _logicCts;
            var task = _logicTask;

            if (cts == null || task == null)
                return;

            _logicCts = null;
            _logicTask = null;

            cts.Cancel();
            Console.WriteLine("Logic cancel signal sent");
            task.GetAwaiter().GetResult();
            cts.Dispose();
        }

        public static async Task StateCalcLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                StateTick();
                await Task.Delay(300);
            }

            Console.WriteLine("Logic stopped");
        }

        public static void ProcessAll()
        {
            while (ConnectorStateQueue.Count > 0)
            {
                StateTick();
                Task.Delay(3000);
            }
        }

        private static void StateTick()
        {
            if (ConnectorStateQueue.Count > 0)
            {
                var connector = ConnectorStateQueue.First();

                if(connector.NewInput.Parent != null)
                {
                    connector.NewInput.Parent.CheckIfInputShouldChange(connector);
                }

                ConnectorStateQueue.RemoveAt(0);
            }
        }
    }
}
