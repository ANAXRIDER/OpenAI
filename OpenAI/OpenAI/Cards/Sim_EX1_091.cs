using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_091 : SimTemplate //cabalshadowpriest
	{

//    kampfschrei:/ übernehmt die kontrolle über einen feindlichen diener mit max. 2 angriff.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (target != null) p.minionGetControlled(target, own.own, false);
		}


	}
}