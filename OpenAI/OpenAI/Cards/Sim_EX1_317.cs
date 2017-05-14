using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_317 : SimTemplate //sensedemons
	{

//    fügt eurer hand zwei zufällige dämonen aus eurem deck hinzu.

		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            p.drawACard(CardDB.cardIDEnum.None, ownplay);
            p.drawACard(CardDB.cardIDEnum.None, ownplay);
		}
	}
}