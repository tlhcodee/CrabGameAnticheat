using CAC.checks.events.impl;
using CAC.data;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CAC.checks.events
{
    public class EventHandler
    {

        private Player player {  get; set; }

        public EventHandler(Player player) 
        {
            this.player = player;
        }

        public void PostEvent(Event e) 
        {
            if (player.checkHandler.checks == null || player.checkHandler.checks.Count == 0)
            {
                ServerSend.SendChatMessage(0, $"Checks are null for {this.player.name}");
                return;
            }

            foreach (Check check in player.checkHandler.checks)
            {
                if (e is EventMovement)
                    check.handleMovementUpdate((EventMovement)e);
                else if(e is EventCombat)
                    check.handleCombatTick((EventCombat)e);
            }
        }
    }
}
