using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_082 : SimTemplate //Clockwork Gnome
    {

        //   Deathrattle: Add a Spare Part card to your hand.

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardIDEnum.PART_001, m.own, true);
        }

    }

}