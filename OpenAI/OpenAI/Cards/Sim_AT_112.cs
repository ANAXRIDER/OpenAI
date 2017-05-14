using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_112 : SimTemplate//Master Jouster
    {
        //Battlecry: Reveal a minion in each deck. If yours costs more, gain Taunt and Divine Shield
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            own.taunt = true;
            own.divineshild = true;
        }
    }
}
