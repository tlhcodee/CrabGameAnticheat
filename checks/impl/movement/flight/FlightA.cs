using CAC.checks.events.impl;
using CAC.data.trackers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CAC.checks.impl.movement.flight
{
    public class FlightA() : Check("Flight", CheckLevel.A, "Player always goes up ? LOL", 10, 20)
    {

        private double noFallAirTicks;

        public override void handleMovementUpdate(EventMovement e)
        {
            PositionTracker positionTracker = this.player.positionTracker;

            bool shouldExit = !e.isPosVerticallyChanged || positionTracker.grounded || player.isCollidingHorizontally;

            if (shouldExit)
            {
                this.Buffer.setBuffer(0); // TO NOT FALSE LEGITS ON LADDERS
                // YES... it falses legits whenever doing a legit hyperglide into a ladder
                return;
            }

            bool isUpwardsMotion = positionTracker.y > positionTracker.lastY; // is the player going upwards and not falling down?

            if (isUpwardsMotion && positionTracker.airTick > 10) // there must be a case that the player is cheating, since he hasnt fell down since 10 air tick
            {
                if(this.Buffer.increase() > this.NeededBuffer) // still need buffer to not false
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
