using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_226 : SimTemplate //frostwolfwarlord
	{

//    kampfschrei:/ erhält +1/+1 für jeden anderen befreundeten diener auf dem schlachtfeld.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            int buff = (own.own) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.minionGetBuffed(own, buff, buff);
		}

	}
}