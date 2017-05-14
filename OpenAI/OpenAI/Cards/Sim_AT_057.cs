using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_057 : SimTemplate //Stablemaster
    {

        //Battlecry: Give a friendly Beast Immune this turn.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null && target.handcard.card.race == TAG_RACE.BEAST)
            {
                target.immune = true;
            }
        }

       

    }
}