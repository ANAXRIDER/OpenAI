﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_040 : SimTemplate //Wildwalker
    {

        //Battlecry: Give a friendly Beast +3 Health.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null && target.handcard.card.race == TAG_RACE.BEAST)
            {
                p.minionGetBuffed(target, 3, 3);
            }
        }

       

    }
}