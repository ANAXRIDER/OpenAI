using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_355 : SimTemplate //blessedchampion
	{

//    verdoppelt den angriff eines dieners.

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.minionGetBuffed(target, target.Angr, 0);
		}

	}
}