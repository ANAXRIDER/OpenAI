using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_021 : SimTemplate //Tiny Knight of Evil
    {

        //Whenever you discard a card, gain +1/+1.

        public override void OnCardWasDiscarded(Playfield p, bool wasOwnCard, Minion triggerEffectMinion)
        {
            if (triggerEffectMinion.own == wasOwnCard)
            {
                p.minionGetBuffed(triggerEffectMinion, 1, 1);
            }
        }


    }
}