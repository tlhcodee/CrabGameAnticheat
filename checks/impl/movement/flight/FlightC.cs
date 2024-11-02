using CAC.checks.events.impl;
using CAC.data.trackers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAC.checks.impl.movement.flight
{
    public class FlightC() : Check("Flight", CheckLevel.C, "Checks for static motion Y over 10 air ticks (air walk)", 5, 10)
    {
        public override void handleMovementUpdate(EventMovement e)
        {
            PositionTracker positionTracker = this.player.positionTracker;

            if (positionTracker.airTick < 10 || this.player.isCollidingHorizontally) // WE DONT WANNA FALSE ON LADDERS AND SOME OTHER CLIMABLE STUFF :D
            {
                this.Buffer.setBuffer(0);
                return;
            }

            double yDifference = Math.Abs(positionTracker.y - positionTracker.lastY); // GET THE Y DIFFERENCE BETWEEN LAST SENT Y & CURRENT ONE

            if(!e.isPosVerticallyChanged || yDifference <= 0.1) // Difference is smaller than 0.1 (airTick is >= 10 meaning player is using some air stuck / air walk module)
            {
                if(this.Buffer.increase() > this.NeededBuffer)
                {
                    this.fail();
                }
            } else
            {
                this.Buffer.decrease();
            }
            base.handleMovementUpdate(e);
        }
    }
}
