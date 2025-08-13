using CAC.checks.events.impl;
using CAC.data.trackers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAC.checks.impl.movement.speed
{
    public class SpeedA() : Check("Speed", CheckLevel.A, "70% Predicts player's horizontal motion", 2, 10)
    {
        public const double DEFAULT_MAX_SPEED = 5.0;
        public const double MINI_MONKE_MAX_SPEED = 8.0;

        public bool hadVelo;

        public override void handleMovementUpdate(EventMovement e)
        {
            PositionTracker positionTracker = player.positionTracker;

            if(positionTracker.deltaY > 1.0)
            {
                hadVelo = true;
            } else if(positionTracker.grounded)
            {
                hadVelo = false;
            }

            if (!e.isPosChanged || hadVelo) // Player has not moved, so we dont care :D
            {
                this.Buffer.setBuffer(0);
                return;
            }

            double horizontalSpeed = positionTracker.horizontalSpeed;

            // IDK WHY I DID THIS, but at the moment i'm coding it, idk how to check serverside if player is sliding.
            // so we're just gonna take the maximum speed they can reach in a map

            bool invalid = Plugin.currentMap.Equals("Mini Monke") || Plugin.currentMap.Equals("Small Beach") ? horizontalSpeed >= MINI_MONKE_MAX_SPEED : horizontalSpeed > DEFAULT_MAX_SPEED;

            if (invalid)
            {
                if (this.Buffer.increase() > this.NeededBuffer)
                {
                    this.fail();
                }
            }
            else
            {
                this.Buffer.decrease();
            }

            base.handleMovementUpdate(e);
        }
    }
}
