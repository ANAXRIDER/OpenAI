using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_044 : SimTemplate //Mulch
    {

        //   Destroy a minion. Add a random minion to your opponent's hand.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (target != null)
            {
                p.minionGetDestroyed(target);
                p.drawACard(CardDB.cardIDEnum.None, !ownplay, true);
            }
        }
    }
}