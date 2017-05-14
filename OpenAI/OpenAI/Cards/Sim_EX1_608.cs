using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_608 : SimTemplate //sorcerersapprentice
	{

//    eure zauber kosten (1) weniger.
        public override void OnAuraStarts(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnSorcerersApprentice++;
            }
            else
            {
                p.anzEnemysorcerersapprentice++;
                
            }

        }

        public override void OnAuraEnds(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnSorcerersApprentice--;
            }
            else
            {
                p.anzEnemysorcerersapprentice--;
            }
        }
	}
}