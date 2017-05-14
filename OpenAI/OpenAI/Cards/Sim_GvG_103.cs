using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_103 : SimTemplate //Micro Machine
    {

        //   At the start of each turn, gain +1 Attack.

        public override void OnTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
            p.minionGetBuffed(triggerEffectMinion, 1, 0);
        }


    }

}