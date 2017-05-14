using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_084 : SimTemplate //Fire Plume Phoenix
    {

        //Battlecry: Deal 2 damage.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.minionGetDamageOrHeal(target, 2);
        }

    }

}