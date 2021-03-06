using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_657 : SimTemplate //* Kabal Songstealer
	{
		// Battlecry: Silence a minion.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (target != null) p.minionGetSilenced(target);
        }
    }
}