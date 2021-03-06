using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_249 : SimTemplate //* Infested Tauren
	{
		//Taunt. Deathrattle: Summon a 2/2 Slime.
		
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.NAX11_03);
		
        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.callKid(kid, m.zonepos - 1, m.own);
        }
	}
}