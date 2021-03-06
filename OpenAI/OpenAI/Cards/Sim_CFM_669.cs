using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_669 : SimTemplate //* Burgly Bully
	{
		// Whenever your opponent casts a spell, add a Coin to your hand.

        public override void OnCardIsGoingToBePlayed(Playfield p, CardDB.Card c, bool wasOwnCard, Minion triggerEffectMinion, Minion target, int choice)
        {
            if (c.type == CardDB.cardtype.SPELL && wasOwnCard != triggerEffectMinion.own)
            {
                p.drawACard(CardDB.cardName.thecoin, triggerEffectMinion.own);
            }
        }
    }
}