using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_076 : SimTemplate //Explosive Sheep
    {

        //  Deathrattle: Deal 2 damage to all minions. 

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.allMinionsGetDamage(2);
        }


    }

}