using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_084 : SimTemplate //huntersmark
	{

//    setzt das leben eines dieners auf 1.
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.minionSetLifetoOne(target);
		}

	}
}