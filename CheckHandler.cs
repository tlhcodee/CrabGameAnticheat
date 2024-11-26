using CAC.checks.impl.combat.interact;
using CAC.checks.impl.movement.flight;
using CAC.checks.impl.movement.invalid;
using CAC.checks.impl.movement.speed;
using CAC.data;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CAC.checks
{
    public class CheckHandler
    {
        private Player player { get; set; } // WE WANT THE CHECKS AND ITS HANDLERS TO BE CUSTOM FOR PLAYER
        public List<Check> checks = new List<Check>();

        public CheckHandler(Player player)
        {
            this.player = player;
            loadChecks();
            foreach (Check check in checks)
            {
                check.loadPlayerData(this.player);
            }
        }

        public void loadChecks()
        {
            // COMBAT CHECKS
            checks.Add(new InteractA());
            checks.Add(new InteractB());

            // MOVEMENT CHECKS
            checks.Add(new FlightA());
            checks.Add(new FlightB());
            checks.Add(new FlightC());
            checks.Add(new SpeedA());
            checks.Add(new SpeedB());
            checks.Add(new SpeedC());
            checks.Add(new InvalidMoveA());
        }

        public List<Check> getMovementChecks()
        {
            string[] movementChecks = new string[] { "Flight", "Speed" };

            List<Check> checksList = new List<Check>();

            foreach(Check check in checks)
            {
                if(movementChecks.Contains(check.name))
                {
                    checksList.Add(check);
                }
            }

            return checksList;
        }
    }
}
