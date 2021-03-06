using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_BRM_027pH : SimTemplate //DIE, INSECTS!
    {

        //   Hero Power: Deal 8 damage to a random enemy. TWICE.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            List<Minion> temp2 = (ownplay) ? new List<Minion>(p.enemyMinions) : new List<Minion>(p.ownMinions);
            int dmg = 8;
            if (ownplay)
            {
                dmg += p.anzOwnFallenHeros;
                if (p.doublepriest >= 1) dmg *= (2 * p.doublepriest);
            }
            else
            {
                dmg += p.anzEnemyFallenHeros;
                if (p.enemydoublepriest >= 1) dmg *= (2 * p.enemydoublepriest);
            }

            int count = (ownplay) ? p.enemyMinions.Count : p.ownMinions.Count;
            if (count >= 1)
            {
                
                temp2.Sort((a, b) => a.Hp.CompareTo(b.Hp));//damage the lowest
                foreach (Minion mins in temp2)
                {
                    p.minionGetDamageOrHeal(mins, dmg);
                    break;
                }
            }
            else
            {
                p.minionGetDamageOrHeal(ownplay ? p.enemyHero : p.ownHero, dmg);
            }

            p.doDmgTriggers();

            count = (ownplay) ? p.enemyMinions.Count : p.ownMinions.Count;
            if (count >= 1)
            {
                
                temp2.Sort((a, b) => a.Hp.CompareTo(b.Hp));//damage the lowest
                foreach (Minion mins in temp2)
                {
                    p.minionGetDamageOrHeal(mins, dmg);
                    break;
                }
            }
            else
            {
                p.minionGetDamageOrHeal(ownplay ? p.enemyHero : p.ownHero, dmg);
            }

        }



    }

}