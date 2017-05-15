using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_OG_328 : SimTemplate //* Master of Evolution
    {
        //Battlecry: Transform a friendly minion into a random one that costs (1) more.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if(target == null) return;
            p.minionTransform(target, p.getRandomCardForManaMinion(target.handcard.card.cost + 1));
        }
    }
}