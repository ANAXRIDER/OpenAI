using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_NEW1_029 : SimTemplate //millhousemanastorm
	{

//    kampfschrei:/ im nächsten zug kosten zauber für euren gegner (0) mana.
        //todo implement the nomanacosts for the enemyturn
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (own.own)
            {
                p.anzOwnMillhouseManastorm = true;
            }
            else
            {
                p.anzEnemyMillhouseManastorm = true;
            }

		}


	}
}