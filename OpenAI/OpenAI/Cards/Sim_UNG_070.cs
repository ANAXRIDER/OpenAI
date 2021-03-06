using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_070 : SimTemplate //Tol'vir Stoneshaper
    {

        //Battlecry: If you played anElemental last turn, gain_Taunt and Divine_Shield.
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (p.anzOwnElementalsLastTurn > 0 && own.own)
            {
                own.divineshild = true;
                own.taunt = true;
                p.anzOwnTaunt++;
            }
        }
    }

}
