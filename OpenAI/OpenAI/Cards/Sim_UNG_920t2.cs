using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_920t2 : SimTemplate //Carnassa's Brood
    {

        //Battlecry: Draw a card.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardIDEnum.None, own.own);
        }

    }

}