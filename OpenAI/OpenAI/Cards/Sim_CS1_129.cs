using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS1_129 : SimTemplate //innerfire
	{

//    setzt den angriff eines dieners auf einen wert, der seinem leben entspricht.
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.minionSetAngrToHP(target);
		}

	}
}