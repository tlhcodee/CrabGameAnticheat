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

namespace CAC
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static System.Collections.Generic.List<Player> players = new System.Collections.Generic.List<Player>();

        public static bool shouldExempt;


        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            shouldExempt = false;
        }

        public static void mitigate(String name, Vector3 currentPos)
        {
            Player player = Utils.getPlayerDataByName(name);

            if (player == null) return;

            if (++player.mitigateTick <= 5)
            {
                if (Utils.getRigidbodyByName(name) == null) return;

                Utils.getRigidbodyByName(name).position = new Vector3(currentPos.x, currentPos.y - 1, currentPos.z);
            } else
            {
                player.mitigateTick = 0;
            }
        }


        [HarmonyPatch(typeof(PlayerMovement), nameof(PlayerMovement.Update))]
        [HarmonyPostfix]
        public static void PlayerMovementUpdate(PlayerMovement __instance)
        {
            if (__instance == null || shouldExempt) return;

            ulong steamID = __instance.GetComponent<PlayerManager>().steamProfile.m_SteamID;
            String name = Utils.getNameByUlong(steamID);

            Player playerData = Utils.getPlayerDataByName(name);

            if (playerData == null) return;

            playerData.grounded = __instance.grounded;

            if (!playerData.grounded)
            {
                playerData.isFalling = __instance.transform.position.y < playerData.lastY;
            }
            else
            {
                playerData.lastMX = __instance.transform.position.x;
                playerData.lastMY = __instance.transform.position.y;
                playerData.lastMZ = __instance.transform.position.z;
            }


            float currentYaw = __instance.transform.rotation.x;
            float currentPitch = __instance.transform.rotation.y;
            float currentY = __instance.transform.position.y;

            bool moonwalk = Math.Abs(playerData.lastYaw - currentYaw) > 0.20;
            playerData.isMoonwalking = moonwalk; // HAVENT TESTED if it actually works

            if (playerData.isFalling)
            {
                playerData.lastFallMS = DateTime.Now.Millisecond;
            }

            if (__instance.IsCrouching())
            {
                playerData.airTick = 0;
                playerData.staticYTick = 0;
                playerData.isFalling = false;
                playerData.lastFallMS = DateTime.Now.Millisecond;
            }

            if (!__instance.grounded)
            {
                if (!playerData.isFalling && playerData.lastFallMS - DateTime.Now.Millisecond > 1.5)
                {
                    Check.flagOnChat(playerData.name, "Flight (Motion A)");
                }

                if (playerData.isFalling && Math.Abs(currentY - playerData.lastY) < 1 && playerData.airTick > 555)
                {
                    Check.flagOnChat(playerData.name, "Flight (Had to be on ground.)");
                }

                if (currentY == playerData.lastY)
                {
                    if (playerData.staticYTick > 60)
                    {
                        Check.flagOnChat(playerData.name, "Flight (Static Motion)");
                    }
                    playerData.staticYTick++;
                }

                playerData.airTick++;
            }
            else
            {
                playerData.airTick = 0;
                playerData.staticYTick = 0;
                playerData.isFalling = false;
                playerData.lastFallMS = DateTime.Now.Millisecond;
            }

            playerData.lastYaw = __instance.transform.rotation.x;
            playerData.lastPitch = __instance.transform.rotation.y;
            playerData.lastY = __instance.transform.position.y;
        }

        [HarmonyPatch(typeof(GameModeTag), nameof(GameModeTag.OnFreezeOverAlert))]
        [HarmonyPostfix]
        public static void onFreezeOver(GameModeTag __instance)
        {
            shouldExempt = false;
            ServerSend.SendChatMessage(0, "This server is using CCA");
            ServerSend.SendChatMessage(0, "Do not try to cheat! or try and see what happens :)");
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

        [HarmonyPatch(typeof(ServerSend), nameof(ServerSend.SendWinner))]
        [HarmonyPrefix]
        public static void ServerSendSendWinner()
        {
            shouldExempt = true;
        }

        [HarmonyPatch(typeof(GameModeTag), nameof(GameModeTag.OnRoundOver))]
        [HarmonyPostfix]
        public static void onRoundOver()
        {
            shouldExempt = true;
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