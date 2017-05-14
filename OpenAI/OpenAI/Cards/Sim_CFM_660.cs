using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_660 : SimTemplate //* Manic Soulcaster
	{
		// Battlecry: Choose a friendly minion. Shuffle a copy into your deck.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (target != null)
            {
                if (m.own) p.ownDeckSize++;
                else p.enemyDeckSize++;
            }
        }
    }
}