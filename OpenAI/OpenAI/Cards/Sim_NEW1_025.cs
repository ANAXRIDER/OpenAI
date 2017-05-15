using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_NEW1_025 : SimTemplate //bloodsailcorsair
	{

//    kampfschrei:/ zieht 1 haltbarkeit von der waffe eures gegners ab.
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            p.lowerWeaponDurability(1, !own.own);
		}


	}
}