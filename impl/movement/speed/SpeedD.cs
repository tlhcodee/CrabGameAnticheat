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
        public override void handleMovementUpdate(EventMovement e)
        {
            PositionTracker positionTracker = this.player.positionTracker;
            RotationTracker rotationTracker = this.player.rotationTracker;

            // NEVERMIND

            base.handleMovementUpdate(e);
        }
    }
}
