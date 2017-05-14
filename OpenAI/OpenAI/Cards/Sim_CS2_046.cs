using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_046 : SimTemplate //bloodlust
	{

//    verleiht euren dienern +3 angriff in diesem zug.

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            List<Minion> temp = (ownplay) ? p.ownMinions: p.enemyMinions;
            foreach (Minion m in temp)
            {
                p.minionGetTempBuff(m, 3, 0);
            }
		}

	}
}