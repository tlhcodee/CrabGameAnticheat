using CAC.checks.events.impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAC.checks.impl.combat.interact
{
    public class InteractA() : Check("Interact", CheckLevel.A, "Checks for invalid hitbox", 5, 2)
    {
        public override void handleCombatTick(EventCombat e)
        {
            if(!e.rayHit) // HITBOX / AIMBOT??
            {
                if(this.Buffer.increase() > this.NeededBuffer)
                {
                    this.fail();
                }
            } else
            {
                this.Buffer.decreaseByValue(0.5);
            }

            base.handleCombatTick(e);
        }
    }
}
