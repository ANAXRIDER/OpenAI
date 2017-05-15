using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_BRM_010b : SimTemplate //bearform
	{

        //   choose 2/5 minion
        CardDB.Card bear = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BRM_010t2);
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
                p.minionTransform(own, bear);
        }

	}
}