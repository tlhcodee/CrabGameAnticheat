using CAC.checks.events.impl;
using CAC.data.trackers;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using UnityEngine;

namespace CAC.checks.impl.movement.flight
{
    public class FlightA() : Check("Flight", CheckLevel.A, "Experimental prediction check", 25, 5)
    {

        private bool disabled;

        public override void handleMovementUpdate(EventMovement e)
        {
            PositionTracker tracker = this.player.positionTracker;

            bool exempt = player.sinceCollideTicks < 20 || tracker.grounded;

            if(exempt)
            {
                this.Buffer.setBuffer(0);
                return;
            }

            double deltaY = Math.Abs(tracker.y - tracker.lastY);

            double velocityY = deltaY;

            double predictedVelocity = velocityY + (10 * Time.deltaTime);

            double predictedY = tracker.y + (predictedVelocity * Time.deltaTime);

            double diff = Math.Abs(tracker.y - predictedY);

            float tolerance = 0.05f;
            bool upwards = tracker.y > tracker.lastY;
            int ticks = tracker.airTick;

            if (velocityY > 1.5 && upwards)
            {
                return;                
            } else if(ticks > 11 && !upwards)
            {
                disabled = true;
            } else if(tracker.grounded || tracker.airTick <= 2)
            {
                disabled = false;
            }

            if(disabled) return;

            if (diff > tolerance)
            {
                if (this.Buffer.increase() > 5)
                {
                    this.fail();

                    if(this.VL >= this.neededBanVL)
                    {
                        this.execution();
                    }
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
