using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_407 : SimTemplate //brawl
	{

//    vernichtet alle diener bis auf einen. (zufällige auswahl)

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.allMinionsGetDestroyed();
		}
	}
}