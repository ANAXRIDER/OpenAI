using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_021 : SimTemplate //* Steam Surger
    {
        //Battlecry: If you played an Elemental last turn add a 'Flame Geyser' to your hand.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (p.anzOwnElementalsLastTurn > 0 && own.own) p.CardToHand(CardDB.cardName.flamegeyser, own.own);
        }
    }
}