using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_028 : SimTemplate //Shado-Pan Rider
    {

        //   Combo: Gain +3 Attack

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (p.cardsPlayedThisTurn >= 1) p.minionGetBuffed(own, 3, 0);
        }

        


    }

}