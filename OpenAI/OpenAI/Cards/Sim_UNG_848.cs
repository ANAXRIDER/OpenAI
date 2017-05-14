using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_848 : SimTemplate //Primordial Drake
    {

        //Taunt Battlecry: Deal 2 damageto all other minions.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.allMinionsGetDamage(2);
        }

    }

}