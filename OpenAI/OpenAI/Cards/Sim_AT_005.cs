using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_005 : SimTemplate//Polymorph: Boar
    {

        //Transform a minion into a 4/2 Boar with Charge

        private CardDB.Card sheep = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.AT_005t);

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionTransform(target, sheep);
        }

    }
}
