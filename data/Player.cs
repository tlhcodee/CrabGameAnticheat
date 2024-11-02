using CAC.checks;
using SteamworksNative;
using CAC.checks.events;
using CAC.data.trackers;

namespace CAC.data
{
    public class Player
    {

        public CheckHandler checkHandler;
        public EventHandler eventHandler;

        public PositionTracker positionTracker;
        public ConnectionTracker connectionTracker;

        public string name;
        ulong steamID;

        // EXEMPTS
        public bool
            isOnLadder,
            isSliding,
            isInWater,
            isHost,
            isReceivedKnockback,
            isCollidingHorizontally;

        public int slidingTicks, crouchingTicks, sinceTaggedTicks;

        public Player(ulong steam_id, string name)
        {
            checkHandler = new CheckHandler(this);
            eventHandler = new EventHandler(this);

            positionTracker = new PositionTracker();
            connectionTracker = new ConnectionTracker();

            steamID = steam_id;
            this.name = name;
        }

        public ulong getSteamID()
        {
            return steamID;
        }
    }
}
