using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_108 : SimTemplate //execute
	{

//    vernichtet einen verletzten feindlichen diener.
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.minionGetDestroyed(target);
		}

	}
}