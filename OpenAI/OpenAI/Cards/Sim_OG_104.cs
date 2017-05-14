using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_OG_104 : SimTemplate //* Embrace the Shadow
    {
        //This turn, your healing effects deal damage instead.

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{			
            if (ownplay)
            {
                p.embracetheshadow++;
            }
		}
	}
}