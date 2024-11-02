using CAC.checks.events.impl;
using CAC.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAC.checks
{
    public partial class Check
    {
        private Player thePlayer;

        public string name, desc;
        public CheckLevel level;

        public CheckBuffer theBuffer;
        public int theVL, neededBanVL, neededBuffer;

        public bool exempted;

        public Check(string name, CheckLevel level, string desc, int neededBanVL, int neededBuffer)
        {
            this.name = name;
            this.level = level;
            this.desc = desc;
            this.neededBanVL = neededBanVL;
            this.neededBuffer = neededBuffer;
            this.theBuffer = new CheckBuffer();
            this.theVL = 0;
        }

        public void fail()
        {
            VL++;

            ServerSend.SendChatMessage(1, $"[CAC] {this.player.name} failed {name} {level} ({VL})");

            if(VL >= neededBanVL)
            {
                ServerSend.SendChatMessage(1, "");
                ServerSend.SendChatMessage(1, $"[CAC] {this.player.name} is removed from the network");
                ServerSend.SendChatMessage(1, $"[CAC] Reason: Cheating ({this.name} {this.level})");
                ServerSend.SendChatMessage(1, "");
                reset();
            }
        }

        public void fail(String debug) { }

        public void applyMitigation(CheckMitigation type)
        {
            UnityEngine.Vector3 safePos = this.player.positionTracker.lastSafePosition;

            switch(type)
            {
                case CheckMitigation.SETBACK_TO_SAFE_POS:
                    player.positionTracker.setPositionOfPlayer(safePos.x, safePos.y, safePos.z);
                    break;
                case CheckMitigation.CANCEL: // TODO: LATER LMAO
                    break;
            }
        }

        public void reset() { this.theVL = 0; }

        public virtual void handleMovementUpdate(EventMovement e) { }
        public virtual void handleCombatTick(EventCombat e) { }


        public void loadPlayerData(Player player)
        {
            this.player = player;
        } 

        public Player player
        {
            get { return thePlayer; }
            set { thePlayer = value; }
        }

        public int VL
        {
            get { return theVL; }
            set { theVL = value; }
        }

        public int NeededBuffer
        {
            get { return theVL; }
            set { theVL = value; }
        }

        public CheckBuffer Buffer
        {
            get { return theBuffer; }
            set { theBuffer = value; }
        }
    }
}
