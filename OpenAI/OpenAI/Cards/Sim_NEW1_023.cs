using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_NEW1_023 : SimTemplate //faeriedragon
	{

//    kann nicht als ziel von zaubern oder heldenfähigkeiten gewählt werden.
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            own.cantBeTargetedBySpellsOrHeroPowers = true;
        }

	}
}