using CAC.checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using UnityEngine;

namespace CAC.data.trackers
{
    public class PositionTracker
    {
        public Player player;

        public bool grounded;

        public double horizontalSpeed;

        public double deltaX, deltaY, deltaZ;

        public double x, y, z, lastX, lastY, lastZ;
        public double motionX, motionY, motionZ, lastMotionX, lastMotionY, lastMotionZ;
        public double fallDistance;

        public int groundTick, airTick;

        public UnityEngine.Vector3 lastSafePosition;

        public void handleTickUpdate(Player player, double x, double y, double z, bool grounded)
        {
            this.player = player;

            if(canHandleSafePos())
            {
                this.lastSafePosition = new UnityEngine.Vector3((float) x, (float) y, (float) z);
            }


            float angle = 0;

            for (int i = 0; i < 36; i++)
            {
                float xx = Mathf.Sin(angle);
                float zz = Mathf.Cos(angle);
                angle += 2 * Mathf.PI / 36;

                UnityEngine.Vector3 dir = new UnityEngine.Vector3((float) (x + x), (float)y, (float) (z + z));
                RaycastHit hit;

                player.isCollidingHorizontally = Physics.Raycast(new UnityEngine.Vector3((float)x, (float)y, (float)z), dir, out hit, 5F);
            }

            this.grounded = grounded;

            // LAST DOUBLES SHOULD STAY A TICK BEHIND
            lastX = this.x;
            lastY = this.y;
            lastZ = this.z;

            lastMotionX = motionX;
            lastMotionY = motionY;
            lastMotionZ = motionZ;

            // UPDATE TO THE NEW TICK
            this.x = x;
            this.y = y;
            this.z = z;

            motionX = Math.Abs(this.x - lastX);
            motionY = Math.Abs(this.y - lastY);
            motionZ = Math.Abs(this.z - lastZ);

            horizontalSpeed = Math.Sqrt((motionX * motionX) + (motionZ * motionZ));

            if (motionY > 0)
            {
                fallDistance += motionY;
            }

            if (this.grounded)
            {
                groundTick++;
                airTick = 0;
                fallDistance = 0;
            }
            else
            {
                groundTick = 0;
                airTick++;
            }

            if(this.player.isCollidingHorizontally)
            {
                this.airTick = 0;
            }
        }

        public void setPositionOfPlayer(float x, float y, float z)
        {
            Utils.getRigidbodyByName(this.player.name).position = new UnityEngine.Vector3(x, y, z);
        }

        public bool canHandleSafePos()
        {
            foreach(Check check in player.checkHandler.getMovementChecks())
            {
                if (check.Buffer.Buffer < check.neededBuffer + 1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
