using CAC.checks.events.impl;
using CAC.data.trackers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAC.checks.impl.movement.flight
{
    public class FlightB() : Check("Flight", CheckLevel.B, "Checks for invalid fall motion", 10, 10)
    {

        public override void handleMovementUpdate(EventMovement e)
        {
            PositionTracker positionTracker = this.player.positionTracker;

            if (!e.isPosVerticallyChanged) return;
            if (positionTracker.grounded || this.player.isCollidingHorizontally) return;

            double deltaY = Math.Abs(positionTracker.motionY - positionTracker.lastMotionY); // Check the delta

            if(deltaY <= 0.001 && positionTracker.airTick > 10) // delta Y is so low meaning the player enabled noclip or something
            {
                if(this.Buffer.increase() > this.NeededBuffer) // BUFFER BUFFER BUFFER $$
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
