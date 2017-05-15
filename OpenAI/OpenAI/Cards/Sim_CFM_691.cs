using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_691 : SimTemplate //* Jade Swarmer
	{
		// Stealth, Deathrattle: Summon a Jade Golem.

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.callKid(p.getNextJadeGolem(m.own), m.zonepos - 1, m.own);
        }
    }
}