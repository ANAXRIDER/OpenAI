using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_227 : SimTemplate //venturecomercenary
	{

//    eure diener kosten (3) mehr.
        public override void OnAuraStarts(Playfield p, Minion own)
		{
           if(own.own) p.anzVentureCoMercenary++;
		}

        public override void OnAuraEnds(Playfield p, Minion own)
        {
           if(own.own) p.anzVentureCoMercenary--;
        }

	}
}