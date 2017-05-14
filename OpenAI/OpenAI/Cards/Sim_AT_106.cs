﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_106 : SimTemplate//Light's Champion
    {
        //Battlecry: Silence a Demon
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {

            if (target != null)
            {
                p.minionGetSilenced(target);
            }
        }

    }
}
