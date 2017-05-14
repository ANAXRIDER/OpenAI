using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_759 : SimTemplate //* Meanstreet Marshal
	{
		// Deathrattle: If this minion has 2 or more Attack, draw a card.

        public override void onDeathrattle(Playfield p, Minion m)
        {
            if (m.Angr >= 2) p.drawACard(CardDB.cardName.unknown, m.own);
        }
    }
}