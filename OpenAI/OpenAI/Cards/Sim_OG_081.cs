using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_081 : SimTemplate //* Shatter
	{
		//Destroy a Frozen minion.
		
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
			if (target.frozen) p.minionGetDestroyed(target);
        }
    }
}