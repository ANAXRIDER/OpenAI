﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_CS2_103 : SimTemplate//Charge
    {
        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetBuffed(target, 2, 0);
            p.minionGetCharge(target);
        }

    }
}
