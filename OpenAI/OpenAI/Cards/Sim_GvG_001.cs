using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_001 : SimTemplate //Flamecannon
    {

        //    Deal $4 damage to a random enemy minion.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            // conservative

            List<Minion> temp = (ownplay) ? p.enemyMinions : p.ownMinions;
            int times = (ownplay) ? p.getSpellDamageDamage(4) : p.getEnemySpellDamageDamage(4);

            if (temp.Count >= 1)
            {
                Minion chosen = temp[0];

                if (ownplay)
                {
                    bool hasDivineShield = false;
                    foreach (Minion m in temp)
                    {
                        if (m.divineshild)
                        {
                            chosen = m;
                            hasDivineShield = true;
                            break;
                        }
                    }

                    if (!hasDivineShield)
                    {
                        List<Minion> temp2 = new List<Minion>(temp);
                        temp2.Sort((a, b) => a.Angr.CompareTo(b.Angr));  // sorted by lowest atk
                        chosen = temp2[0];
                    }
                }
                else
                {
                    List<Minion> temp2 = new List<Minion>(temp);
                    temp2.Sort((a, b) => -a.Angr.CompareTo(b.Angr));  // sorted by highest atk

                    // find strongest minion that can be killed, or pick minion with highest hp
                    int maxhp = 0;
                    foreach (Minion m in temp2)
                    {
                        if (m.Hp <= times)
                        {
                            chosen = m;
                            break;
                        }

                        if (maxhp < m.Hp)
                        {
                            chosen = m;
                            maxhp = m.Hp;
                        }
                    }
                }

                p.minionGetDamageOrHeal(chosen, times);
            }
        }



    }

}