using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_014 : SimTemplate //kingmukla
	{

//    kampfschrei:/ gebt eurem gegner 2 bananen.
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            p.drawACard(CardDB.cardIDEnum.EX1_014t, !own.own, true);
            if (own.own)
            {
                p.enemycarddraw -= 1;
            }
            p.drawACard(CardDB.cardIDEnum.EX1_014t, !own.own, true);
            if (own.own)
            {
                p.enemycarddraw -= 1;
            }
		}


	}
}