using CAC.checks.events.impl;
using CAC.data;
using CAC.data.trackers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CAC.checks.impl.combat.interact
{
    public class InteractA() : Check("Interact", CheckLevel.A, "Checks for invalid hitbox (no rotation hits & range)", 5, 2)
    {
        public class OrderedAttack
        {
            Player attacker, victim;
            Vector3 from, to;
            double distance;

            public OrderedAttack(Player attacker, Player victim, Vector3 from, Vector3 to, double distance)
            {
                this.attacker = attacker;
                this.victim = victim;
                this.from = from;   
                this.to = to;
                this.distance = distance;
            }

            public bool isCorrectHit(float range)
            {
                Vector3 origin = from;
                Vector3 victimCenter = GetVictimCenter(victim);
                Vector3 dir = (victimCenter - origin);
                float dist = dir.magnitude;
                if (dist < 1e-4f) return true;
                dir /= dist;


                if (Physics.Raycast(origin, dir, out RaycastHit hit, Mathf.Min(range, dist + 0.2f)))
                {
                    if (IsVictimCollider(hit.collider, victim))
                        return true;               
                    else
                        return false;            
                }

                const float probeRadius = 0.25f;   
                int n = Physics.SphereCastNonAlloc(origin, probeRadius, dir, _buf,
                                                   Mathf.Min(range, dist + 0.3f));
                for (int i = 0; i < n; i++)
                {
                    if (IsVictimCollider(_buf[i].collider, victim))
                        return true;
                }

                return BoundsInflatedContains(victim, origin, range, 0.15f);
            }

            // === Helpers ===

            private static RaycastHit[] _buf = new RaycastHit[8];

            private static bool IsVictimCollider(Collider col, Player v)
            {
                var root = col.attachedRigidbody ? col.attachedRigidbody.transform.root : col.transform.root;
                var vRoot = v.getRigidbody().transform.root;
                return root == vRoot;
            }

            private static Vector3 GetVictimCenter(Player v)
            {
                var tr = v.getRigidbody().transform;
                return tr.position + Vector3.up * 0.9f; 
            }

            private static bool BoundsInflatedContains(Player v, Vector3 rayOrigin, float range, float inflate)
            {
                var cols = v.getRigidbody().transform.root.GetComponentsInChildren<Collider>(true);
                var end = rayOrigin + (range * Vector3.one); 
                foreach (var c in cols)
                {
                    var b = c.bounds;
                    b.Expand(inflate * 2f);
                    if (b.SqrDistance(rayOrigin) < range * range)
                        return true;
                }
                return false;
            }
        }

        public List<OrderedAttack> attacksPending = new List<OrderedAttack>();

        public bool mitigate = false;

        public override void handleCombatTick(EventCombat e)
        {
            Player attacker = player;
            Player victim = Utils.getPlayerDataByName(e.names[1]);
            Vector3 from = e.from, to = e.to;
            double dist = e.distance;

            PositionTracker positionTracker = player.positionTracker;

            bool isMovedInTheLastTicks = (DateTime.Now - positionTracker.lastUpdate).TotalMilliseconds < 400;

            if (attacksPending.Count > 2) return;

            attacksPending.Add(new OrderedAttack(attacker, victim, from, to, dist)); 
            // Queue Attack for the next tick, so we can actually lower the chance for false flags.
            // The next tick = we actually know where the player is

            if(mitigate)
            {
                e.cancel();
                // Mitigation.
                // Just like minecraft anti-cheats, if the player actually flagged our checks a lot
                // We should just let him chill a bit with the cheat
                // so lets cancel his hit.
            }

            base.handleCombatTick(e);
        }

        public override void handleMovementUpdate(EventMovement e)
        {
            // Check attack on movement update

            PositionTracker positionTracker = player.positionTracker;

            if(attacksPending.Count > 0)
            {
                OrderedAttack current = attacksPending[0];
                double delay = (DateTime.Now - positionTracker.lastUpdate).TotalMilliseconds;

                bool isMovedInTheLastTicks = delay < 400;
                // BRO isn't moving
                // Can cause false flags, cuz we're using ordered attacks.

                if (current == null) return;

                float range = 7f;

                // Add ping calculation
                range += (float) ((delay / 50) * 1.4f);

                if(!current.isCorrectHit(range))
                {
                    if(this.Buffer.increase() > 1)
                    {
                        mitigate = true;
                        this.fail();

                        if (this.VL >= this.neededBanVL)
                        {
                            this.execution();
                        }
                    }
                } else
                {
                    this.Buffer.decreaseByValue(0.3);
                }

                if(this.Buffer.Buffer < 1)
                {
                    mitigate = false;
                }

                attacksPending.Remove(current);
            } else
            {
                mitigate = false;
            }

            base.handleMovementUpdate(e);
        }
    }
}
