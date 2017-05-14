using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_009 : SimTemplate //Rhonin
    {

        //Deathrattle: Add 3 copies of Arcane Missiles to your hand.


        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardIDEnum.EX1_277, m.own, true);
            p.drawACard(CardDB.cardIDEnum.EX1_277, m.own, true);
            p.drawACard(CardDB.cardIDEnum.EX1_277, m.own, true);
        }

    }
}