using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_557 : SimTemplate //natpagle
	{

//    zu beginn eures zuges besteht eine chance von 50%, dass ihr eine zusätzliche karte zieht.
        public override void OnTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
            if (triggerEffectMinion.own == turnStartOfOwner)
            {
                p.drawACard(CardDB.cardIDEnum.None, turnStartOfOwner);
            }
        }
	}
}