using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_013 : SimTemplate //Power Word: Glory
	{

//Choose a minion. Whenever it attacks, restore 4 Health to your hero.

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            if (ownplay)
            {
                target.ownPowerWordGlory++;
            }
            else
            {
                target.enemyPowerWordGlory++;
            }

		}

	}
}