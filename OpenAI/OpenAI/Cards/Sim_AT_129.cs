using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_129 : SimTemplate //Fjola Lightbane
    {

        //Whenever you target this minion with a spell, gain Divine Shield

        public override void OnCardIsGoingToBePlayed(Playfield p, CardDB.Card c, bool wasOwnCard, Minion triggerEffectMinion, Minion target, int choice)
        {
            if (triggerEffectMinion.own == wasOwnCard && c.type == CardDB.cardtype.SPELL && target!=null && target.entityID == triggerEffectMinion.entityID)
            {
                triggerEffectMinion.divineshild = true;
            }
        }

       

    }
}