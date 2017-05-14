using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_694 : SimTemplate //* Shadow Sensei
	{
		// Battlecry: Give a Stealth minion +2/+2.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (target != null) p.minionGetBuffed(target, 2, 2);
        }
    }
}