using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_108 : SimTemplate //Recombobulator
    {

        //   Battlecry: Transform a friendly minion into a random minion with the same Cost.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if(target == null) return;
            p.minionTransform(target, target.handcard.card);
        }

    }

}