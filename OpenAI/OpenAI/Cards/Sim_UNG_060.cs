using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_060 : SimTemplate //Mimic Pod
    {

        //Draw a card, then add a copy of it to your hand.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.CardToHand(CardDB.cardName.unknown, ownplay);
            p.CardToHand(CardDB.cardName.unknown, ownplay);
        }

    }

}