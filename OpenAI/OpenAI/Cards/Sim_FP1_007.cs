using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_FP1_007 : SimTemplate //nerubianegg
	{
        CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.FP1_007t);//nerubian
//    todesröcheln:/ ruft einen neruber (4/4) herbei.
        public override void OnDeathrattle(Playfield p, Minion m)
        {
            
            p.callKid(c, m.zonepos-1, m.own);
        }

	}
}