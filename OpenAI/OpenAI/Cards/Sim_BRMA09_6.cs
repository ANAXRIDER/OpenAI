using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_BRMA09_6 : SimTemplate //* The True Warchief
    {
        // Destroy a Legendary minion.
		
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
			if (target != null) p.minionGetDestroyed(target);
        }
    }
}