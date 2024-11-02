using CAC.checks.events.impl;
using CAC.data.trackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CAC.checks.impl.movement.speed
{
    public class SpeedB() : Check("Speed", CheckLevel.B, "This check stands for horizontal speed analyses", 10, 10)
    {
        public List<double> movementUpdates = new List<double>();
        public double lastAverage;

        public override void handleMovementUpdate(EventMovement e)
        {
            PositionTracker positionTracker = this.player.positionTracker;

            if(!e.isPosHorizontallyChanged || positionTracker.horizontalSpeed < 1.5 || positionTracker.motionY > 0.8) // we dont care about the speeds that can be happen by legit gameplay. (hyperglide and some other techs)
            {
                this.Buffer.setBuffer(0);
                return;
            }

            movementUpdates.Add(positionTracker.horizontalSpeed); // ADD the current speed to the list

            if (movementUpdates.Count < 30) return; // we need some more to check the average

            double averageSpeed = movementUpdates.Sum() / movementUpdates.Count; // Average speed

            double difference = averageSpeed - lastAverage; // diff between averages

            bool invalid = difference <= 0.4; // the difference is actually low meaning the player hasnt slow down yet (constant speed?)

            if(invalid)
            {
                if(this.Buffer.increase() > this.NeededBuffer)
                {
                    this.fail();
                }
            } else
            {
                this.Buffer.decreaseByValue(0.05);
            }

            lastAverage = averageSpeed;

            movementUpdates.Clear(); // CLEAR THE LIST SO WE DONT SPAM LOL
            base.handleMovementUpdate(e);
        }
    }
}
