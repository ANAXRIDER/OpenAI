using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_080b : SimTemplate //* Kingsblood Toxin
	{
		//Draw a card.
		
        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, ownplay);
        }
    }
}
