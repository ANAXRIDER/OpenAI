using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_058 : SimTemplate //Spellslinger
    {

        //Battlecry: Reveal a minion in each deck. If yours costs more, draw it.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            //p.drawACard(CardDB.cardIDEnum.None, own.own, true);
            if (p.ownMaxMana >= 6)
            {
                p.drawACard(CardDB.cardIDEnum.None, own.own, true);
            }
        }
    }
}