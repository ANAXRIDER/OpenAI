using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_027t2 : SimTemplate //Pyros
    {

        //Deathrattle: Return this to_your hand as a 10/10 that costs (10).

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardIDEnum.UNG_027t4, m.own, true);
        }

    }

}