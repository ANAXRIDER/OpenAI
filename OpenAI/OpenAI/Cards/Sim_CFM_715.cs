using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_715 : SimTemplate //* Jade Spirit
	{
		// Battlecry: Summon a Jade Golem.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            p.callKid(p.getNextJadeGolem(m.own), m.zonepos, m.own, true);
        }
	}
}