﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_NEW1_038 : SimTemplate//Gruul
    {
        public override void OnTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            p.minionGetBuffed(triggerEffectMinion, 1, 1);
        }
        

    }
}
