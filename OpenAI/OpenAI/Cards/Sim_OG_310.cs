using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_310 : SimTemplate //* Steward of Darkshire
	{
		//Whenever you summon a 1-Health minion, give it Divine Shield.

        public override void OnMinionWasSummoned(Playfield p, Minion m, Minion summonedMinion)
        {
            if (summonedMinion.Hp == 1 && m.own == summonedMinion.own && m.entityID != summonedMinion.entityID)
            {
                summonedMinion.divineshild = true;
            }
        }
    }
}