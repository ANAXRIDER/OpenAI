using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_045 : SimTemplate //rockbiterweapon
	{

//    verleiht einem befreundeten charakter +3 angriff in diesem zug.

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.minionGetTempBuff(target, 3,0);
		}

	}
}