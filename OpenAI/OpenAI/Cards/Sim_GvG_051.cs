using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_051 : SimTemplate //Warbot
    {

        //   Enrage:&lt;/b&gt; +1 Attack.

        public override void OnEnrageStart(Playfield p, Minion m)
        {
            p.minionGetBuffed(m, 1, 0);
        }

        public override void OnEnrageStop(Playfield p, Minion m)
        {
            p.minionGetBuffed(m, -1, 0);
        }


    }

}