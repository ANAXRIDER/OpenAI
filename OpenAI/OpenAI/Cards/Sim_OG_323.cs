using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_323 : SimTemplate //* Polluted Hoarder
	{
		//Deathrattle: Draw a card.
		
        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardName.unknown, m.own);
        }
    }
}