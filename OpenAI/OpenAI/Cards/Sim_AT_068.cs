using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_068 : SimTemplate //Bolster
    {

        //  Give your Taunt minions +2/+2

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {

            foreach (Minion m in (ownplay) ? p.ownMinions : p.enemyMinions)
            {
                if (m.taunt) p.minionGetBuffed(m, 2, 2);
            }
        }


    }

}