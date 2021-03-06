using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_321 : SimTemplate //* Crazed Worshipper
	{
		//Taunt. Whenever this minion takes damage, give your C'Thun +1/+1 (wherever it is).

		public override void OnMinionGotDmgTrigger(Playfield p, Minion m, bool ownDmgdmin)
        {
            if (m.anzGotDmg > 0)
            {
				p.anzOgOwnCThunHpBonus += m.anzGotDmg;
				p.anzOgOwnCThunAngrBonus += m.anzGotDmg;
            }
        }
	}
}