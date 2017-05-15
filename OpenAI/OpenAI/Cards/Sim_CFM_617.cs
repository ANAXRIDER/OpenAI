using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_617 : SimTemplate //* Celestial Dreamer
	{
		// Battlecry: If a friendly minion has 5 or more attack, gain +2/+2.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            List<Minion> temp = (m.own) ? p.ownMinions : p.enemyMinions;
            foreach (Minion mnn in temp)
            {
                if (mnn.Angr > 4)
                {
                    p.minionGetBuffed(m, 2, 2);
                    break;
                }
            }
        }
    }
}