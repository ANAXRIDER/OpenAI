using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_LOE_089 : SimTemplate //wobbling runts
	{

        //    Dr: summon three 2/2 runts
        CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOE_089t);//hyena
        
        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.callKid(c, m.zonepos-1, m.own);
            p.callKid(c, m.zonepos-1, m.own);
            p.callKid(c, m.zonepos - 1, m.own);
        }

	}
}