using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_203 : SimTemplate //ironbeakowl
	{

//    kampfschrei:/ bringt einen diener zum schweigen/.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (target != null) p.minionGetSilenced(target);
		}


	}
}