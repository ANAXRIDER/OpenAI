using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_090 : SimTemplate //Mukla's Champion
    {

        //Inspire: Give your other minions +1/+1.

        public override void OnInspire(Playfield p, Minion m)
        {
            foreach (Minion mini in (m.own) ? p.ownMinions : p.enemyMinions)
            {
                if (m.entityID != mini.entityID) p.minionGetBuffed(mini, 1, 1);
            }
        }


    }
}