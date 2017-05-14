using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_907 : SimTemplate //Ozruk
    {

        //Taunt Battlecry: Gain +5 Healthfor each Elemental youplayed last turn.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own) p.minionGetBuffed(own, 0, p.anzOwnElementalsLastTurn * 5);
        }

    }

}