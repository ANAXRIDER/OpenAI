using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_612 : SimTemplate //kirintormage
	{

//    kampfschrei:/ das nächste geheimnis/, das ihr in diesem zug ausspielt, kostet (0).

		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (own.own) p.nextSecretThisTurnCost0 = true;
		}


	}
}