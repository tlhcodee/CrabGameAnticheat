using CAC.checks.events.impl;
using CAC.data.trackers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CAC.checks.impl.movement.flight
{
    public class FlightB() : Check("Flight", CheckLevel.B, "Checks for invalid jump height", 5, 5)
    {

        public override void handleMovementUpdate(EventMovement e)
        {
            PositionTracker pos = this.player.positionTracker;

            int airTicks = pos.airTick;
            double deltaY = pos.y - pos.lastY;

            if(airTicks > 0 && airTicks <= 3)
            {
                bool invalid = deltaY > 0.4;

                if(invalid)
                {
                    this.fail("deltaY: " + deltaY + " ticks: " + airTicks);
                }
            }

            base.handleMovementUpdate(e);
        }
    }
}
