using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_699 : SimTemplate //* Seadevil Stinger
	{
		// Battlecry: The next Murloc you play this turn costs Health instead of Mana.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (m.own) p.nextMurlocThisTurnCostHealth = true;
        }
    }
}