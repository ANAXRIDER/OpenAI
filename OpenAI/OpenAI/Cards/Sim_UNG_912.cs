using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_912 : SimTemplate //Jeweled Macaw
    {

        //Battlecry: Add a random Beast to your hand.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.CardToHand(CardDB.cardName.unknown, own.own);
        }

    }

}