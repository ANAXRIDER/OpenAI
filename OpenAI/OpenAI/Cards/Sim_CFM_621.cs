using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_621 : SimTemplate //* Kazakus
	{
		// Battlecry: If your deck has no duplicates, create a custom spell.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (m.own && Hrtprozis.Instance.noDuplicates) p.drawACard(CardDB.cardName.unknown, m.own, true);
        }
    }
}