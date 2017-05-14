using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_313 : SimTemplate //* Finders Keepers
	{
		// Discover a card with Overload. Overload: (1)

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, ownplay);
            p.changeRecall(ownplay, 1);
        }
    }
}