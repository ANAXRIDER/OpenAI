using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_693 : SimTemplate //* Gadgetzan Ferryman
	{
		// Combo: Return a friendly minion to your hand.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (p.cardsPlayedThisTurn > 0 && target != null) p.minionReturnToHand(target, target.own, 0);
        }
    }
}