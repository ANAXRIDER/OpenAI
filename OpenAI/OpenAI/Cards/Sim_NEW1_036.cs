﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_NEW1_036 : SimTemplate//commanding shout
    {

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            List<Minion> temp = (ownplay) ? p.ownMinions : p.enemyMinions;
            foreach (Minion t in temp)
            {
                t.cantLowerHPbelowONE = true;
            }
            p.drawACard(CardDB.cardIDEnum.None, ownplay);
        }

    }
}
