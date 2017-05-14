using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_032 : SimTemplate //Shady Dealer
    {

        //   Battlecry: If you have a Pirate, gain +1/+1.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            bool hasPirate = false;
            foreach (Minion m in (own.own) ? p.ownMinions : p.enemyMinions)
            {
                if (m.handcard.card.race == TAG_RACE.PIRATE)
                {
                    hasPirate = true;
                    break;
                }
            }

            if (hasPirate) p.minionGetBuffed(own, 1, 1);
        }

        


    }

}