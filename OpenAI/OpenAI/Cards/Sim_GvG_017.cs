using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_017 : SimTemplate //Call Pet
    {

        //    Draw a card. If it's a Beast, it costs (4) less.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.drawACard(CardDB.cardIDEnum.None, ownplay);
            p.evaluatePenality += (ownplay) ? -10 : 10;
        }


    }

}