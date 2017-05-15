using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_312 : SimTemplate //twistingnether
	{

//    vernichtet alle diener.

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.allMinionsGetDestroyed();
		}

	}
}