using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_178b : SimTemplate //uproot
	{

//    +5 angriff.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            p.minionGetBuffed(own, 5, 0);
		}



	}
}