using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_113 : SimTemplate //* Darkshire Councilman
	{
		//After you summon a minion, gain +1 Attack.

        public override void onMinionWasSummoned(Playfield p, Minion m, Minion summonedMinion)
        {
            if (m.entityID != summonedMinion.entityID && m.own == summonedMinion.own)
            {
				p.minionGetBuffed(m, 1, 0);
            }
        }
    }
}