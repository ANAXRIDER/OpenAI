using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_341 : SimTemplate //* Sergeant Sally
	{
		// Deathrattle: Deal damage equal to the minion's Attack to all enemy minions.

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.allMinionOfASideGetDamage(!m.own, m.Angr);
        }
    }
}