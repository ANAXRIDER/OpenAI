using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_058 : SimTemplate //Razorpetal Lasher
    {

        //Battlecry: Add aRazorpetal to your handthat deals 1 damage.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.CardToHand(CardDB.cardName.razorpetal, own.own);
        }

    }

}