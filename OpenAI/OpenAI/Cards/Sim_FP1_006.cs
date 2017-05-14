using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_FP1_006 : SimTemplate //deathcharger
    {

        //    ansturm. todesröcheln:/ fügt eurem helden 3 schaden zu.

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.minionGetDamageOrHeal(m.own ? p.ownHero : p.enemyHero, 3);
        }

    }
}