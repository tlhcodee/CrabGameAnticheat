using CAC.checks.events.impl;
using CAC.data.trackers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CAC.checks.impl.movement.flight
{
    public class FlightB() : Check("Flight", CheckLevel.B, "Checks for gliding y position", 5, 5)
    {

        private int updates = 0;
        
        public override void handleMovementUpdate(EventMovement e)
        {
            PositionTracker pos = this.player.positionTracker;

            bool exempt = pos.deltaY > 1 || pos.grounded || pos.deltaY == 0.0;

            if (true) return;

            // CHECK EVERY 10 TICKS
            if (updates % 10 == 0)
            {
                double lastDeltaY = pos.lastMotionY;

                if(pos.deltaY - lastDeltaY < 0.4)
                {
                    if(this.Buffer.increase() > this.NeededBuffer)
                    {
                        this.fail();
                    }
                }
                else
                {
                    this.Buffer.decrease();
                }
            }

            updates++;
            base.handleMovementUpdate(e);
        }
    }
}
