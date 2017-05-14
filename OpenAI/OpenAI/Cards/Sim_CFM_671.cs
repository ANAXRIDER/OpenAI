using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_671 : SimTemplate //* Cryomancer
	{
		// Battlecry: Gain +2/+2 if an enemy is Frozen.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            List<Minion> temp = (m.own) ? p.enemyMinions : p.ownMinions;
            foreach (Minion mnn in temp)
            {
                if (mnn.frozen)
                {
                    p.minionGetBuffed(m, 2, 2);
                    break;
                }
            }
        }
    }
}