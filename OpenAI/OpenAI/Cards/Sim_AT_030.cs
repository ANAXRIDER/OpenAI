using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_030 : SimTemplate //Undercity Valiant
    {

        //   Combo: Deal 1 damage.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (p.cardsPlayedThisTurn>=1 && target!=null)
            {
                p.minionGetDamageOrHeal(own, 1);
            }
        }

        


    }

}