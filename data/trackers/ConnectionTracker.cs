using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace CAC.data.trackers
{
    public class ConnectionTracker
    {
        public List<long> updates = new List<long>();

        public double ping;

        public long averageDelay, lastDelay, lastAverage;

        public ConnectionTracker()
        {
            this.ping = 0;
            this.lastDelay = 0;
            this.averageDelay = 0;
        }

        public void handleNetworkPosition(ulong param_0, Vector3 param_1)
        {
            ulong user = param_0;
            Vector3 posReceived = param_1;

            String name = Utils.getNameByUlong(user);
            Player data = Utils.getPlayerDataByName(name);

            if (data == null) return;

            updates.Add(DateTime.Now.Millisecond - lastDelay);

            if (updates.Count >= 6)
            {
                long c = 0;
                int updatesSize = updates.Count;

                foreach (long update in updates)
                {
                    c += update;
                    updates.Remove(update);
                }

                long average = c / updatesSize;

                lastAverage = average;
            }

            lastDelay = DateTime.Now.Millisecond;
        }
    }
}
