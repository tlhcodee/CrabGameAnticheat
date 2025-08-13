using System;
using System.Collections.Generic;
using System.Text;

namespace CAC.data.trackers
{
    public class RotationTracker
    {
        public Player player;

        public float yaw, pitch, lastYaw, lastPitch, deltaYaw, deltaPitch, lastDeltaYaw, lastDeltaPitch;

        public DateTime lastRotationTick;

        public RotationTracker(Player player)
        {
            this.player = player;
        }

        public void handleRotationTick(float yaw, float pitch)
        {
            lastYaw = this.yaw;
            lastPitch = this.pitch;

            lastDeltaYaw = this.deltaYaw;
            lastDeltaPitch = this.pitch;    

            deltaYaw = Math.Abs(this.yaw - this.lastYaw);
            deltaPitch = Math.Abs(this.pitch - this.lastPitch);

            this.yaw = yaw;
            this.pitch = pitch;

            this.lastRotationTick = DateTime.Now;
        }

    }
}
