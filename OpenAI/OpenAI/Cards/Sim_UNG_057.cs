using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_057 : SimTemplate //Razorpetal Volley
    {

        //Add two Razorpetals to_your hand that deal_1 damage.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.CardToHand(CardDB.cardName.razorpetal, ownplay);
            p.CardToHand(CardDB.cardName.razorpetal, ownplay);
        }

    }

}