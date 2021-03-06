using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_KAR_044 : SimTemplate //Moroes
    {
        // Stealth At the end of your turn, summon a 1/1 Steward.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.KAR_044a);//Steward

        public override void OnTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            if (triggerEffectMinion.own == turnEndOfOwner)
            {
                int posi = triggerEffectMinion.zonepos;

                p.callKid(kid, posi, triggerEffectMinion.own);
            }
        }
    }
}