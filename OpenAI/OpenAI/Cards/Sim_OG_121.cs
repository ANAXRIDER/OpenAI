using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_OG_121 : SimTemplate //* Cho'Gall
    {
        //Battlecry: The next spell you cast this turn costs Health instead of Mana.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (ownplay)
            {
                p.nextSpellThisTurnCostHealth = true;
            }
        }
    }
}