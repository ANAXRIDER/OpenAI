using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_Mekka1 : SimTemplate //homingchicken
	{

//    vernichtet zu beginn eures zuges diesen diener und zieht 3 karten.

        public override void OnTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
            if (turnStartOfOwner == triggerEffectMinion.own)
            {
                p.minionGetDestroyed(triggerEffectMinion);
                p.drawACard(CardDB.cardIDEnum.None, turnStartOfOwner);
                p.drawACard(CardDB.cardIDEnum.None, turnStartOfOwner);
                p.drawACard(CardDB.cardIDEnum.None, turnStartOfOwner);
            }
        }

	}
}