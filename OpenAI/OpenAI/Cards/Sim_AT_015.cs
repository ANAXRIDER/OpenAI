using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_015 : SimTemplate //Convert
    {

        // Put a copy of an enemy minion into your hand.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {

            if (target != null)
            {
                p.drawACard(target.handcard.card.cardIDenum, ownplay, true);
            }
        }


    }

}