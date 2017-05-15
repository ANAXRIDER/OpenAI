using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_064 : SimTemplate //Vilespine Slayer
    {

        //Combo: Destroy a minion.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (p.cardsPlayedThisTurn >= 1 && target != null) p.minionGetDestroyed(target);
        }

    }

}