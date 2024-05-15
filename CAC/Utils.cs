

using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace CAC
{
    internal class Utils
    {

        public static string getNameByUlong(ulong id)
        {
            bool flag = MonoBehaviourPublicDi2UIObacspDi2UIObUnique.Instance != null && MonoBehaviourPublicDi2UIObacspDi2UIObUnique.Instance.activePlayers != null;
            if (flag)
            {
                foreach (KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> keyValuePair in MonoBehaviourPublicDi2UIObacspDi2UIObUnique.Instance.activePlayers)
                {
                    bool flag2 = keyValuePair.Value.steamProfile.m_SteamID == id;
                    if (flag2)
                    {
                        return keyValuePair.value.username;
                    }
                }
            }
            return string.Empty;
        }

        public static Player getPlayerDataByName(string name)
        {
            foreach(Player p in Plugin.players)
            {
                if (p.name.Equals(name))
                {
                    return p;
                }
            }
            return null;
        }

        public static Rigidbody getRigidbodyByName(string name)
        {
            foreach (KeyValuePair<ulong, MonoBehaviourPublicCSstReshTrheObplBojuUnique> keyValuePair in MonoBehaviourPublicDi2UIObacspDi2UIObUnique.Instance.activePlayers)
            {
                bool flag2 = keyValuePair.Value.username == name;
                if (flag2)
                    return keyValuePair.Value.GetComponent<Rigidbody>();
            }
            return null;
        }
    }
}
