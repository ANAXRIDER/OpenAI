using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_078 : SimTemplate //Tortollan Forager
    {

        //Battlecry: Add a random minion with 5 or more Attack to your hand.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.CardToHand(CardDB.cardName.unknown, own.own);
        }


    }

}