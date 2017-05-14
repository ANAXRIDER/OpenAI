using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_806 : SimTemplate //* Wrathion
	{
		// Taunt. Battlecry: Draw cards until you draw one that isn't a Dragon.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, m.own);
            p.drawACard(CardDB.cardName.unknown, m.own);
        }
    }
}