﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_LOEA16_3 : SimTemplate //Lantern of Power
    {
        //giv a minion +10/10

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (target != null)
            {
                p.minionGetBuffed(target, 10, 10);
            }
        }


    }
}