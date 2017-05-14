using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_AT_103 : SimTemplate //north sea kraken
	{

        //   bttlcry Deal 4 damage
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (target != null)
            {
                p.minionGetDamageOrHeal(target, 4);
            }

		}

	}
}