using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using SteamworksNative;
using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using CAC.checks.events.impl;
using CAC.data;
using CAC.data.trackers;

namespace CAC
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static System.Collections.Generic.List<Player> players = new System.Collections.Generic.List<Player>();

        public static string currentMap;

        public static bool isStarted;

        public static Language choosenLanguage = Language.ENGLISH;
        public static bool autoBan = true;

        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        [HarmonyPatch(typeof(ServerSend), nameof(ServerSend.LoadMap), new System.Type[] { typeof(int), typeof(int) })]
        [HarmonyPrefix]
        public static void LoadMapHook(ref int __0, int __1)
        {
           currentMap = MapManager.Instance.GetMap(__0).mapName;
           isStarted = false;
        }

        [HarmonyPatch(typeof(GameModeTag), nameof(GameModeTag.OnRoundOver))]
        [HarmonyPostfix]
        internal static void OnRoundOver()
        {
            // NEED THAT LATER
        }

        [HarmonyPatch(typeof(GameModeTag), nameof(GameModeTag.OnFreezeOver))]
        [HarmonyPostfix]
        internal static void OnFreezeOver()
        {
            isStarted = true;
        }

        [HarmonyPatch(typeof(ServerHandle), nameof(ServerHandle.PlayerRotation))]
        [HarmonyPostfix]
        internal static void ServerHandlePlayerRotation(ulong param_0, Packet param_1)
        {
            ulong playerSteam = param_0;
            string name = Utils.getNameByUlong(playerSteam);

            Player player = Utils.getPlayerDataByName(name);

            float yaw = BitConverter.ToSingle(param_1.field_Private_ArrayOf_Byte_0, 8);
            float pitch = BitConverter.ToSingle(param_1.field_Private_ArrayOf_Byte_0, 12);

            if(player != null)
            {
                player.rotationTracker.handleRotationTick(yaw, pitch);
            }
        }


        [HarmonyPatch(typeof(ServerHandle), nameof(ServerHandle.PlayerPosition))]
        [HarmonyPostfix]
        internal static void PostServerHandlePlayerPosition(ulong param_0, Packet param_1)
        {
            Vector3 receivedPosition = new(BitConverter.ToSingle(param_1.field_Private_ArrayOf_Byte_0, 8), BitConverter.ToSingle(param_1.field_Private_ArrayOf_Byte_0, 12), BitConverter.ToSingle(param_1.field_Private_ArrayOf_Byte_0, 16));
            ulong playerSteam = param_0;
            string name = Utils.getNameByUlong(playerSteam);

            Player player = Utils.getPlayerDataByName(name);

            if(player != null)
            {
                PlayerMovement movement = Utils.getPlayerMovement(name);

                bool grounded = false;

                RaycastHit hit;

                int groundLayer = LayerMask.NameToLayer("Ground");
                LayerMask raycastLayer = (1 << groundLayer);

                if (Physics.Raycast(receivedPosition, Vector3.down, out hit, 3f, raycastLayer))
                {
                    grounded = true;
                }

                double x = receivedPosition.x;
                double y = receivedPosition.y;
                double z = receivedPosition.z;

                float yaw = 0;
                float pitch = 0;

                bool isPosChanged = x != player.positionTracker.x || y != player.positionTracker.y || z != player.positionTracker.z;
                bool horizontalChange = x != player.positionTracker.x || z != player.positionTracker.z;
                bool verticalChange = y != player.positionTracker.y;

                player.positionTracker.handleTickUpdate(x, y, z, grounded);

                EventMovement moveEvent = new EventMovement(x, y, z, yaw, pitch, grounded, isPosChanged, horizontalChange, verticalChange);

                player.eventHandler.PostEvent(moveEvent);
            }
        }


        [HarmonyPatch(typeof(ServerSend), nameof(ServerSend.SendChatMessage))]
        [HarmonyPrefix]
        public static bool ServerSendSendChatMessagePre(ulong param_0, string param_1)
        {
            if (!IsHost()) return true;
            string msg = param_1.ToLower();
            if (param_0 == GetMyID() && msg.StartsWith("!"))
            {
                switch (msg)
                {
                    case "!autoban":
                        autoBan = !autoBan;
                        String current = autoBan ? choosenLanguage == Language.TURKISH ? "Anti hile otomatik banlarý açýldý" : "Anticheat automated bans are toggled on" :
                            choosenLanguage == Language.TURKISH ? "Anti hile otomatik banlarý kapatýldý" : "Anticheat automated bans are toggled off";

                        ChatBox.Instance.AppendMessage(1, current, "");
                        break;
                    case "!language":
                        if(choosenLanguage == Language.TURKISH)
                        {
                            ChatBox.Instance.AppendMessage(1, "Language is set to English.", "");
                            choosenLanguage = Language.ENGLISH;
                        } else if(choosenLanguage == Language.ENGLISH)
                        {
                            ChatBox.Instance.AppendMessage(1, "Dil türkçe olarak ayarlandý.", "");
                            choosenLanguage = Language.TURKISH;
                        }
                        break;
                    default:
                        if(choosenLanguage == Language.TURKISH)
                        {
                            ChatBox.Instance.AppendMessage(1, "Bilinmeyen komut. komut listemize göz atmak isteyebilirsin.", "");
                            ChatBox.Instance.AppendMessage(1, "!langauge - ingilizce ve türkçe arasýnda geçiþ", "");
                            ChatBox.Instance.AppendMessage(1, "!autoban - otomatik banlama aç kapat", "");
                        } else
                        {
                            ChatBox.Instance.AppendMessage(1, "Unknown command. you may want to check out command list", "");
                            ChatBox.Instance.AppendMessage(1, "!langauge - switch langauge between turkish and english", "");
                            ChatBox.Instance.AppendMessage(1, "!autoban - toggle on/off anticheat automated bans", "");
                        }
                        break;
                }
                return false;
            }
            else return true;
        }


        public static ulong GetMyID()
        {
            return SteamManager.Instance.field_Private_CSteamID_0.m_SteamID;
        }

        public static ulong GetHostID()
        {
            return SteamManager.Instance.field_Private_CSteamID_1.m_SteamID;
        }

        public static bool IsHost()
        {
            return SteamManager.Instance.IsLobbyOwner() && !LobbyManager.Instance.Method_Public_Boolean_0();
        }


        /*[HarmonyPatch(typeof(PlayerMovement), nameof(PlayerMovement.Update))]
        [HarmonyPostfix]
        public static void PlayerMovementUpdate(PlayerMovement __instance)
        {
            if (__instance == null) return;

            ulong steamID = __instance.GetComponent<PlayerManager>().steamProfile.m_SteamID;
            String name = Utils.getNameByUlong(steamID);

            Player player = Utils.getPlayerDataByName(name);

            if (player == null || player.positionTracker == null || player.eventHandler == null || player.checkHandler == null)
            {
                return;
            }
        }*/

        [HarmonyPatch(typeof(ServerSend), nameof(ServerSend.PlayerPosition))]
        [HarmonyPostfix]
        public static void PlayerPosition(ulong param_0, Vector3 param_1)
        {
            ulong user = param_0;
            Vector3 posReceived = param_1;

            String name = Utils.getNameByUlong(user);
            Player data = Utils.getPlayerDataByName(name);

            if (data == null) return;

            ConnectionTracker tracker = data.connectionTracker;

            tracker.handleNetworkPosition(param_0, param_1);
        }

        [HarmonyPatch(typeof(ServerSend), nameof(ServerSend.TagPlayer))]
        [HarmonyPostfix]
        public static void TagPlayer(ulong param_0, ulong param_1)
        {
            ulong tagger = param_0;
            ulong victim = param_1;

            Rigidbody taggerRB = Utils.getRigidbodyByName(Utils.getNameByUlong(tagger));
            Rigidbody victimRB = Utils.getRigidbodyByName(Utils.getNameByUlong(victim));

            ulong steamID = taggerRB.GetComponent<PlayerManager>().steamProfile.m_SteamID;
            String name = Utils.getNameByUlong(steamID);

            Player data = Utils.getPlayerDataByName(name);

            ulong steamIDVictim = victimRB.GetComponent<PlayerManager>().steamProfile.m_SteamID;
            String nameVictim = Utils.getNameByUlong(steamIDVictim);

            Player dataVictim = Utils.getPlayerDataByName(nameVictim);

            Vector3 from = taggerRB.position;
            Vector3 to = victimRB.position;

            double distance = Vector3.Distance(to, from);

            bool rayHitStuff = Physics.Raycast(taggerRB.GetComponent<PlayerMovement>().playerCam.position,
                taggerRB.GetComponent<PlayerMovement>().playerCam.forward,
                out RaycastHit raycastHit,

                8f);

            dataVictim.isReceivedKnockback = true;

            if (data == null) return;

            EventCombat e = new EventCombat(from, to, distance, rayHitStuff);
            data.eventHandler.PostEvent(e);
        }

        [HarmonyPatch(typeof(GameModeTag), nameof(GameModeTag.OnFreezeOverAlert))]
        [HarmonyPostfix]
        public static void onFreezeOver(GameModeTag __instance)
        {
            ServerSend.SendChatMessage(0, "[CAC ANNOUNCEMENT]");
            ServerSend.SendChatMessage(0, "This server has a anti-cheat named CAC");
            ServerSend.SendChatMessage(0, "Do NOT try to hack.");
            Il2CppSystem.Collections.Generic.Dictionary<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique>.Enumerator enumerator = MonoBehaviourPublicDi2UIObacspDi2UIObUnique.Instance.activePlayers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Il2CppSystem.Collections.Generic.KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> pl = enumerator.Current;
                Player playerData = Plugin.players.Find((Player p) => p.name == Utils.getNameByUlong(pl.Value.steamProfile.m_SteamID));
                bool flag2 = playerData == null;
                if (flag2)
                {
                    string name = Utils.getNameByUlong(pl.Value.steamProfile.m_SteamID);
                    ulong id = pl.Value.steamProfile.m_SteamID;
                    players.Add(new Player(id, name));

                    ServerSend.SendChatMessage(0, "registered + " + name);
                }
            }
        }


        public static bool isRegistered(ulong id)
        {
            foreach(Player p in players)
            {
                if(p.getSteamID() == id)
                {
                    return true;
                }
            }
            return false;
        }
    }

}