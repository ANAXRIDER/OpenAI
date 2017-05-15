using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_324 : SimTemplate //* White Eyes
	{
		// Taunt. Deathrattle: Shuffle 'The Storm Guardian' into your deck.

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            if (m.own) p.ownDeckSize++;
            else p.enemyDeckSize++;
        }
    }
}