using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_KAR_710 : SimTemplate //Arcanosmith
    {
        // Battlecry: Summon a 0/5 minion with Taunt.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.KAR_710m);//Animated Shield

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.callKid(kid, own.zonepos, own.own, true);
        }
    }
}