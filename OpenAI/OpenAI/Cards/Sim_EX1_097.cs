using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_097 : SimTemplate //abomination
	{

//    spott/. todesröcheln:/ fügt allen charakteren 2 schaden zu.

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.allCharsGetDamage(2);
        }

	}
}