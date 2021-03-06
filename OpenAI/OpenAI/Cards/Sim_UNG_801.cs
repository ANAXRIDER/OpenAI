using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_801 : SimTemplate //Nesting Roc
    {

        //Battlecry: If you control at_least 2 other minions, gain Taunt.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {

            if (p.ownMinions.Count >= 2) own.taunt = true;
        }

    }

}