using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_830 : SimTemplate //Cruel Dinomancer
    {

        //Deathrattle: Summon arandom minion youdiscarded this game.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_306);//Succubus .. assummed.

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.callKid(kid, m.zonepos - 1, m.own);
        }

    }

}