using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_371 : SimTemplate //handofprotection
	{

//    verleiht einem diener gottesschild/.

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            target.divineshild = true;
		}

	}
}