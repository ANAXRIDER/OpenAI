using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_042a : SimTemplate //catform
	{

        //   choose 5/2 minion
        CardDB.Card cat = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.AT_042t);
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.minionTransform(own, cat);
        }

	}
}