using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_113 : SimTemplate //Bright-Eyed Scout
    {

        //Battlecry: Draw a card. Change its Cost to (5).

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.drawACard(CardDB.cardIDEnum.None, ownplay);
        }

    }

}