using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_672 : SimTemplate //* Madam Goya
	{
		// Battlecry: Choose a friendly minion. Swap it with a minion in your deck.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (target != null)
            {
                target.frozen = true;
                p.drawACard(CardDB.cardName.unknown, m.own, true);
            }
        }
    }
}