using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_DS1_184 : SimTemplate //tracking
	{

//    schaut euch die drei obersten karten eures decks an. zieht eine davon und werft die anderen beiden ab.

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            int decksize = (ownplay) ? p.ownDeckSize : p.enemyDeckSize;
            if(decksize >=1) p.drawACard(CardDB.cardIDEnum.None, ownplay);
            //p.evaluatePenality += 100;
		}

	}
}