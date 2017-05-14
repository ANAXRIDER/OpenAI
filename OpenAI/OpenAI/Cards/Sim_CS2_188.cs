using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_188 : SimTemplate //abusivesergeant
	{

//    kampfschrei:/ verleiht einem diener +2 angriff in diesem zug.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (target != null) p.minionGetTempBuff(target, 2, 0);
		}


	}
}