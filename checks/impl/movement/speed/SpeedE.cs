using CAC.checks.events.impl;
using CAC.data.trackers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace CAC.checks.impl.movement.speed
{
    public class SpeedE() : Check("Speed", CheckLevel.E, "Simulates client behaviour to predict movement", 5, 5)
    {

        private bool should;
        private int times, sinceNoMoveTicks;

        public override void handleMovementUpdate(EventMovement e)
        {
            PositionTracker positionTracker = this.player.positionTracker;

            double speed = positionTracker.horizontalSpeed;
            if (speed <= 0.1)
            {
                sinceNoMoveTicks = 0;
            } else
            {
                sinceNoMoveTicks++;
            }

            bool grounded = positionTracker.grounded;

            if(speed > (grounded ? 2.5 : 2.7) && sinceNoMoveTicks < 15)
            {
                if(should)
                {
                    times++;
                    ServerSend.SendChatMessage(1, "times: " + times);
                    should = false;
                }
            }
            else
            {
                should = true;
            }

            if(speed > 2.5 && times >= 8)
            {
                this.fail("times: " + times + " speed: " + speed);
            }

            base.handleMovementUpdate(e);
        }
    }
}
