using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_663 : SimTemplate //* Kabal Trafficker
	{
		// At the end of your turn, add a random Demon to your hand.

        public override void OnTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            if (triggerEffectMinion.own == turnEndOfOwner)
            {
                p.drawACard(CardDB.cardName.unknown, turnEndOfOwner, true);
            }
        }
    }
}