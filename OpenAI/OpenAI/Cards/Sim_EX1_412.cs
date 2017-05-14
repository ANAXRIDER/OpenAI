using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_412 : SimTemplate //ragingworgen
	{

//    wutanfall:/ windzorn/ und +1 angriff
        public override void OnEnrageStart(Playfield p, Minion m)
        {
            m.Angr++;
            p.minionGetWindfurry(m);
        }

        public override void OnEnrageStop(Playfield p, Minion m)
        {
            m.Angr--;
            m.windfury = false;
            if (m.numAttacksThisTurn == 1) m.Ready = false;
        }


	}
}