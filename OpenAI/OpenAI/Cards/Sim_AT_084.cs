using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_084 : SimTemplate //Lance Carrier
    {

        //Battlecry: Give a friendly minion +2 Attack.


        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {

            if (target != null)
            {
                p.minionGetBuffed(target, 2, 0);
            }
        }

    }
}