﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_045 : SimTemplate //Aviana
    {

        //Your minions cost (1).

        public override void onAuraStarts(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.anzOwnAviana++;
            }
            else
            {
                p.anzEnemyAviana++;
            }
        }

        public override void onAuraEnds(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.anzOwnAviana--;
            }
            else
            {
                p.anzEnemyAviana--;
            }
        }

       

    }
}