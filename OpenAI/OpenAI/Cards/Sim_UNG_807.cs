using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_807 : SimTemplate //Golakka Crawler
    {

        //Battlecry: Destroy a Pirate and gain +1/+1.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null && target.handcard.card.race == TAG_RACE.PIRATE)
            {
                p.minionGetDestroyed(target);
                p.minionGetBuffed(own, 1, 1);
            }
        }

    }

}