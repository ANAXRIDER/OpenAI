using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_155b : SimTemplate //markofnature
	{

//    +4 leben und spott/.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.minionGetBuffed(target, 0, 4);
            target.taunt = true;
		}

	}
}