using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_GAME_005 : SimTemplate //thecoin
	{

//    erhaltet 1 manakristall nur für diesen zug.
        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (ownplay)
            {
                if (p.mana <= 9) p.mana++;
            }
            else
            {
                p.mana++;
            }
        }

	}
}