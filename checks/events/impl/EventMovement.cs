using System;
using System.Collections.Generic;
using System.Text;

namespace CAC.checks.events.impl
{
    public class EventMovement : Event
    {
        public double x, y, z;

        public float yaw, pitch;

        public bool grounded, isPosChanged, isPosHorizontallyChanged, isPosVerticallyChanged;

        public EventMovement(double x, double y, double z, float yaw, float pitch, bool grounded, bool isPosChanged, bool isPosHorizontallyChanged, bool isPosVerticallyChanged)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.yaw = yaw;
            this.pitch = pitch;
            this.grounded = grounded;
            this.isPosChanged = isPosChanged;
            this.isPosHorizontallyChanged = isPosHorizontallyChanged;
            this.isPosVerticallyChanged = isPosVerticallyChanged;
        }
    }
}
