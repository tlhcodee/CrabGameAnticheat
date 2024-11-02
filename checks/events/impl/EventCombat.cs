using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CAC.checks.events.impl
{
    public class EventCombat : Event
    {
        public Vector3 from, to;
        public double distance;
        public bool rayHit;
        
        public EventCombat(Vector3 from, Vector3 to, double distance, bool isRayHit)
        {
            this.from = from;
            this.to = to;
            this.distance = distance;
            this.rayHit = isRayHit;
        }
    }
}
