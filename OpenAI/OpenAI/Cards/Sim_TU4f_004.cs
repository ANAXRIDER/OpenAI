using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_TU4f_004 : SimTemplate //* Legacy of the Emperor
	{
		// Give your minions +2/+2. (+2 Attack/+2 Health)
		
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
			p.allMinionOfASideGetBuffed(ownplay, 2, 2);
		}
	}
}