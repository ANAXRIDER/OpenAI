using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_LOE_104 : SimTemplate //entomb
    {

        //   choose enemy minion, shuffle it in your deck

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (target != null)
            {
                p.minionGetDestroyed(target);
                if (ownplay)
                {
                    p.ownDeckSize++;
                }
                else
                {
                    p.enemyDeckSize++;
                }
            }
        }
    }
}