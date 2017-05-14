using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_290 : SimTemplate //* Ancient Harbinger
	{
		//At the start of your turn, put a 10-Cost minion from your deck into your hand.
		
        public override void OnTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
            if (triggerEffectMinion.own == turnStartOfOwner)
            {
				p.drawACard(CardDB.cardName.varianwrynn, turnStartOfOwner);
            }
        }
	}
}