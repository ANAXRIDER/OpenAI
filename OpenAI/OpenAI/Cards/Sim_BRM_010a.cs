using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_BRM_010a : SimTemplate //catform
	{

        //   choose 5/2 minion
        CardDB.Card cat = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BRM_010t);
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.minionTransform(own, cat);
        }

	}
}