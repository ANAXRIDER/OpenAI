using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_063 : SimTemplate //* Kooky Chemist
	{
		// Battlecry: Swap the Attack and Health of a minion.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (target != null) p.minionSwapAngrAndHP(target);
        }
    }
}