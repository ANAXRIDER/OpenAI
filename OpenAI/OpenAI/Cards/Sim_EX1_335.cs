using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_335 : SimTemplate //lightspawn
	{

//    der angriff dieses dieners entspricht immer seinem leben.
        //todo dont buff this!
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            own.Angr = own.Hp;
		}

	}
}