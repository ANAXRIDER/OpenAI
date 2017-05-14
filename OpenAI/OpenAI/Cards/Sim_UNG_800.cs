using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_800 : SimTemplate //Terrorscale Stalker
    {

        //Battlecry: Trigger a friendly minion's Deathrattle.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null) p.doDeathrattles(new List<Minion>() { target });
        }

    }

}