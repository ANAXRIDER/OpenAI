using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_926 : SimTemplate //Cornered Sentry
    {

        //Taunt. Battlecry: Summon three 1/1 Raptors for your_opponent.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.UNG_076t1);//Raptor

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {

            int pos = (own.own) ? p.enemyMinions.Count : p.ownMinions.Count;
            p.callKid(kid, pos, !own.own);
            p.callKid(kid, pos, !own.own);
            p.callKid(kid, pos, !own.own);
        }

    }

}