using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_FP1_008 : SimTemplate //spectralknight
	{

//    kann nicht als ziel von zaubern oder heldenfähigkeiten gewählt werden.
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            own.cantBeTargetedBySpellsOrHeroPowers = true;
        }
	}
}