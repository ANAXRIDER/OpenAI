using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_235 : SimTemplate //northshirecleric
	{

//    zieht jedes mal eine karte, wenn ein diener geheilt wird.

        public override void onAMinionGotHealedTrigger(Playfield p, Minion triggerEffectMinion)
        {
            p.drawACard(CardDB.cardIDEnum.None, triggerEffectMinion.own);
        }

	}
}