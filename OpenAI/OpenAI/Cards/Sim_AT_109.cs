using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_109 : SimTemplate //Argent Watchman
    {

        //insprire: Can attack as normal this turn.

        public override void OnInspire(Playfield p, Minion m)
        {
            m.canAttackNormal = true;
            m.updateReadyness();
        }



    }

}