using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_941 : SimTemplate //Primordial Glyph
    {

        //Discover a spell. Reduce its Cost by (2).

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.CardToHand(CardDB.cardName.unknown, ownplay);
        }

    }

}