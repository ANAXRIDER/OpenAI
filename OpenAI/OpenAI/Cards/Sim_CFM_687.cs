using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_687 : SimTemplate //* Inkmaster Solia
	{
		// Battlecry: If your deck has no duplicates, the next spell you cast this turn costs (0).

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (m.own && Hrtprozis.Instance.noDuplicates) p.nextSpellThisTurnCost0 = true;
        }
    }
}