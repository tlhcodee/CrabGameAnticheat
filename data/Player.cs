using CAC.checks;
using SteamworksNative;
using CAC.checks.events;
using CAC.data.trackers;
using UnityEngine;

namespace CAC.data
{
    public class Player
    {

        public CheckHandler checkHandler;
        public EventHandler eventHandler;

        public PositionTracker positionTracker;
        public RotationTracker rotationTracker;
        public ConnectionTracker connectionTracker;
        public CollisionTracker collisionTracker;

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

        public int slidingTicks, crouchingTicks, sinceTaggedTicks, sinceCollideTicks;

        public Rigidbody getRigidbody()
        {
            if (steamID == Plugin.GetMyID()) return PlayerMovement.prop_MonoBehaviourPublicGaplfoGaTrorplTrRiBoUnique_0.GetRb();
            else return GameManager.Instance.activePlayers[steamID].prop_MonoBehaviourPublicObVeSiVeRiSiAnVeanTrUnique_0.field_Private_Rigidbody_0;
        } 

        public int getPing()
        {
            return LobbyManager.lobbyPlayers[steamID].ping;
        }

        public Player(ulong steam_id, string name)
        {
            checkHandler = new CheckHandler(this);
            eventHandler = new EventHandler(this);

            positionTracker = new PositionTracker(this);
            rotationTracker = new RotationTracker(this);
            connectionTracker = new ConnectionTracker();
            collisionTracker = new CollisionTracker();
            collisionTracker.init();

            steamID = steam_id;
            this.name = name;
        }

        public ulong getSteamID()
        {
            return steamID;
        }
    }
}
