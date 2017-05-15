﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_EX1_012 : SimTemplate//bloodmage thalnos
    {
        public override void OnAuraStarts(Playfield p, Minion own)
        {

            own.spellpower = 1;
            if (own.own)
            {
                p.spellpower++;
            }
            else
            {
                p.enemyspellpower++;
            }
        }

       

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardIDEnum.None, m.own);
        }

    }
}
