using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_241 : SimTemplate //* Possessed Villager
	{
		//Deathrattle: Summon a 1/1 Shadowbeast.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.OG_241a);

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.callKid(kid, m.zonepos-1, m.own);
        }
	}
}