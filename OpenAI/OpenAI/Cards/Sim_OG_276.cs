using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_276 : SimTemplate //* Blood Warriors
	{
		//Add a copy of each damaged friendly minion to your hand.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            List<Minion> temp = (ownplay) ? p.ownMinions : p.enemyMinions;

            foreach (Minion m in temp)
            {
                if (m.wounded) p.drawACard(m.handcard.card.name, ownplay, true);
            }
        }
    }
}