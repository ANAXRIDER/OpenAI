using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_KAR_005 : SimTemplate //Kindly Grandmother
    {
        // Deathrattle: Summon a 3/2 Big Bad Wolf.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.KAR_005a);//Big Bad Wolf

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.callKid(kid, m.zonepos - 1, m.own);
        }
    }
}