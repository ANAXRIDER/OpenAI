using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_095 : SimTemplate //* Weasel Tunneler
	{
		// Deathrattle: Shuffle this minion into your opponent's deck.

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            if (m.own) p.enemyDeckSize++;
            else p.ownDeckSize++;
        }
    }
}