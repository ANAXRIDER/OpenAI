using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_178a : SimTemplate //rooted
	{

//    +5 leben und spott/.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            p.minionGetBuffed(own, 0, 5);
            own.taunt = true;
		}


	}
}