using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_024 : SimTemplate //* Flamewreathed Faceless
	{
		//Overload: (2)
		
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (own != null)
            {
                p.changeRecall(own.own, 2);
            }
		}
	}
}