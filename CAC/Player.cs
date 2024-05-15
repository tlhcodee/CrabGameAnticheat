using SteamworksNative;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAC
{
    public class Player
    {
        public string name;

        ulong steamID;
        public bool isFalling;
        public bool grounded;
        public double lastX, lastY, lastZ;
        public float lastMX, lastMY, lastMZ;

        public float lastYaw, lastPitch;

        public long lastFallMS;

        public bool isMoonwalking;

        public int mitigateTick, airTick, staticYTick;

        public Player(ulong steam_id, string name)
        {
            this.steamID = steam_id;
            this.name = name;
            this.mitigateTick = 0;
            this.airTick = 0;
            this.staticYTick = 0;
        }

        public ulong getSteamID()
        {
            return steamID;
        }
    }
}
