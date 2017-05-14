using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_335 : SimTemplate //* Shifting Shade
	{
		//Deathrattle: Copy a card from your opponent's deck and add it to your hand.
		
        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardName.unknown, m.own, true);
        }
	}
}