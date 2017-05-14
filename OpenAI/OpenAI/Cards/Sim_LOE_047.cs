using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_LOE_047 : SimTemplate //Tomb Spider
	{
        //Battlecry: Discover a Beast.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardIDEnum.None, own.own, true);
            //rogue= pit snake
            //warrior fierce monkey
            //hunter = all his cards? ^^
            //druid = mounted raptor, savage combatant, jungle moonkin, malorne
        }
	}
}
