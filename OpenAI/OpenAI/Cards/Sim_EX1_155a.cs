using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_155a : SimTemplate //markofnature
	{

//    +4 angriff.


        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.minionGetBuffed(target, 4, 0);
		}

	}
}