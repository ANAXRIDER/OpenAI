using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_851t1 : SimTemplate //Un'Goro Pack
    {

        //Add 5 Journey to Un'Goro cards to your hand.

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.CardToHand(CardDB.cardName.unknown, ownplay);
            p.CardToHand(CardDB.cardName.unknown, ownplay);
            p.CardToHand(CardDB.cardName.unknown, ownplay);
            p.CardToHand(CardDB.cardName.unknown, ownplay);
            p.CardToHand(CardDB.cardName.unknown, ownplay);
        }

    }

}