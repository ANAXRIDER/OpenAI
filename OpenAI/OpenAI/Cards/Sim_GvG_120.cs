using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_120 : SimTemplate //Hemet Nesingwary
    {

        //   Battlecry: Destroy a Beast.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target == null) return;

            p.minionGetDestroyed(target);
        }



    }

}