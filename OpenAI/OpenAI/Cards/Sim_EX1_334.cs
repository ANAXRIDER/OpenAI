using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_334 : SimTemplate //shadowmadness
	{

//    übernehmt bis zum ende des zuges die kontrolle über einen feindlichen diener mit max. 3 angriff.
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            target.shadowmadnessed = true;
            p.minionGetControlled(target, ownplay, true);
		}

	}
}