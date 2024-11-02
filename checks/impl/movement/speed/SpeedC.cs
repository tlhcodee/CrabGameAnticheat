using CAC.checks.events.impl;
using CAC.data.trackers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CAC.checks.impl.movement.speed
{
    public class SpeedC() : Check("Speed", CheckLevel.C, "Checks for too much of difference in deltaX and deltaZ", 2, 2)
    {
        public override void handleMovementUpdate(EventMovement e)
        {
            PositionTracker positionTracker = this.player.positionTracker;

            if(!e.isPosHorizontallyChanged || !Plugin.isStarted || positionTracker.motionY > 0.8) // match isnt started so, teleporting players into maps can false LMAO
            {
                this.Buffer.setBuffer(0);
                return;
            }

            double deltaX = positionTracker.deltaX;
            double deltaZ = positionTracker.deltaZ;

            // GET THE DISTANCE BETWEEN LAST SENT POSITION AND THE CURRENT ONE
            double distance = Vector3.Distance(
                new Vector3((float)positionTracker.x, (float)positionTracker.y, (float)positionTracker.z),
                new Vector3((float)positionTracker.lastX, (float)positionTracker.lastY, (float)positionTracker.lastZ)); 

            // DISTANCE IS BIGGER THAN 5, seems like our guy is not legit
            // :D
            bool invalid = Plugin.currentMap.Equals("Mini Monke") ? distance > 25 : distance > 5; // TELEPORT HACKS???

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

            base.handleMovementUpdate(e);
        }
    }
}
