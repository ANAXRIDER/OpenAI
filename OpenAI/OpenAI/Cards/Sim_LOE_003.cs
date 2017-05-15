using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_LOE_003 : SimTemplate //ethereal conjurer
	{
        //Battlecry: discover a spell

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardIDEnum.None, own.own, true);
        }
	}
}
