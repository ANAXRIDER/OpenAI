using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_603 : SimTemplate //crueltaskmaster
	{

//    kampfschrei:/ fügt einem diener 1 schaden zu und verleiht ihm +2 angriff.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (target != null)
            {
                p.minionGetDamageOrHeal(target, 1);
                p.minionGetTempBuff(target, 2, 0);
            }

		}

	}
}