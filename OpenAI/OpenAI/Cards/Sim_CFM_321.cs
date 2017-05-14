using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_321 : SimTemplate //* Grimestreet Informant
	{
		// Battlecry: Discover a Hunter, Paladin or Warrior card.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, m.own, true);
        }
    }
}