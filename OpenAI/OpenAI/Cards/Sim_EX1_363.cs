using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_363 : SimTemplate //blessingofwisdom
	{

//    wählt einen diener aus. zieht jedes mal eine karte, wenn er angreift.

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            if (ownplay)
            {
                target.ownBlessingOfWisdom++;
            }
            else
            {
                target.enemyBlessingOfWisdom++;
            }

		}

	}
}