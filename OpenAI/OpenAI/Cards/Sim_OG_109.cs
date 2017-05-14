using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_OG_109 : SimTemplate //* Darkshire Librarian
    {
        //Battlecry: Discard a random card. Deathrattle: Draw a card.

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardName.unknown, m.own);
        }

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.discardACard(own.own);
        }
    }
}