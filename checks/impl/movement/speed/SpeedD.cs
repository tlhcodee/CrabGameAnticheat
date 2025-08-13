using CAC.checks.events.impl;
using CAC.data.trackers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CAC.checks.impl.movement.speed
{
    public class SpeedD() : Check("Speed", CheckLevel.D, "Basically checks if you're speeding while not moonwalking lol", 5, 5)
    {
        public double lastSpeed = 0;

        public override void handleMovementUpdate(EventMovement e)
        {
            PositionTracker positionTracker = this.player.positionTracker;
            RotationTracker rotationTracker = this.player.rotationTracker;

            float yaw = rotationTracker.yaw;
            float lastYaw = rotationTracker.lastYaw;

            double speed = positionTracker.horizontalSpeed;

            bool isRotating = (DateTime.Now - rotationTracker.lastRotationTick).TotalMilliseconds < 300;

            bool isSpeedCorrect = speed >= lastSpeed || speed - lastSpeed < 0.5;

            bool invalid = speed >= 1.7 && !isRotating && isSpeedCorrect && positionTracker.deltaY <= 0.3;

            if(positionTracker.deltaY >= 0.5)
            {
                this.Buffer.decreaseByValue(2.5);
            }

            if(invalid)
            {
                if(this.Buffer.increase() > 15)
                {
                    this.fail();
                }
            }
            else
            {
                this.Buffer.decreaseByValue(1.5);
            }

            lastSpeed = speed;

            base.handleMovementUpdate(e);
        }
    }
}
