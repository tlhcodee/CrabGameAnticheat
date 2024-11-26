using CAC.checks.events.impl;
using CAC.data.trackers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAC.checks.impl.combat.interact
{
    public class InteractB() : Check("Interact", CheckLevel.B, "Checks for invalid hit distance based on ping", 10, 2)
    {

        public override void handleCombatTick(EventCombat e)
        {
            double dist = e.distance;

            if(dist > calculateDistanceWithPing())
            {
                if(this.Buffer.increase() > this.NeededBuffer)
                {
                    this.fail();
                }
            } else
            {
                this.Buffer.decreaseByValue(0.5); 
            }

            base.handleCombatTick(e);
        }

        public double calculateDistanceWithPing()
        {
            ConnectionTracker connectionTracker = this.player.connectionTracker; 

            double distanceNeeded = 8; // DEFAULT HIT DISTANCE LEGITS CAN DO (AS WHAT I KNOW)

            if(connectionTracker.ping > 100)
            {
                distanceNeeded += (connectionTracker.averageDelay / 50) * 2; // ADD EXTRA 2 UNIT PER EVERY 50 AMOUNT OF AVERAGE DELAY
            }

            // THIS WILL ALLOW A BYPASS BY SIMPLY FAKE LAGGING
            // SO WE'LL ALSO HANDLE FAKE LAGS WITH ANOTHER CHECK

            return distanceNeeded;
        }
    }
}
