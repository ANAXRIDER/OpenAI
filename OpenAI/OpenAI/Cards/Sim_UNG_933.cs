using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_933 : SimTemplate //King Mosh
    {

        //Battlecry: Destroy all damaged minions.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            foreach (Minion m in p.ownMinions)
            {
                if (m.wounded) p.minionGetDestroyed(m);
            }
            foreach (Minion m in p.enemyMinions)
            {
                if (m.wounded) p.minionGetDestroyed(m);
            }
        }
    }

}