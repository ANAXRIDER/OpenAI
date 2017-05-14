using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_EX1_593 : SimTemplate //nightblade
    {

        //    kampfschrei: /fügt dem feindlichen helden 3 schaden zu.
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.minionGetDamageOrHeal(own.own ? p.enemyHero : p.ownHero, 3);
        }

    }
}