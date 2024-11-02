using CAC.checks.events.impl;
using CAC.data.trackers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAC.checks.impl.movement.invalid
{
    public class InvalidMoveA() : Check("InvalidMove", CheckLevel.A, "Checks if the player is moving before the freeze end", 5, 20)
    {
        public override void handleMovementUpdate(EventMovement e)
        {
            // TODO: ACTUALLY MAKE IT LMAO

            /*PositionTracker positionTracker = this.player.positionTracker;

            if(!e.isPosHorizontallyChanged || Plugin.isStarted)
            {
                return;
            }

            double speed = positionTracker.horizontalSpeed;

            if(speed > 0.4)
            {
                if(this.Buffer.increase() > this.NeededBuffer)
                {
                    this.fail();
                }
            } else
            {
                this.Buffer.decreaseByValue(0.2);
            }*/

            base.handleMovementUpdate(e);
        }
    }
}
