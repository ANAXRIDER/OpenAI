using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS1_113 : SimTemplate //mindcontrol
	{

//    übernehmt die kontrolle über einen feindlichen diener.
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.minionGetControlled(target, ownplay, false);
		}

	}
}