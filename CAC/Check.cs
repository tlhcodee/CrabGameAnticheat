using System;
using System.Collections.Generic;
using System.Text;

namespace CAC
{
    internal class Check
    {

        public static void flagOnChat(String playerName, String checkName)
        {
            ServerSend.SendChatMessage(0, "[CAC] " + playerName + " is detected using " + checkName);
        }
    }
}
