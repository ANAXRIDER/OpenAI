using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_852 : SimTemplate //* Lotus Agents
	{
		// Battlecry: Discover a Druid, Rogue or Shaman card.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, m.own, true);
        }
    }
}