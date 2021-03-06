using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_083 : SimTemplate //Devilsaur Egg
    {

        //Deathrattle: Summon a 5/5 Devilsaur.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.UNG_083t1);//Devilsaur

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.callKid(kid, m.zonepos - 1, m.own);
        }

    }

}