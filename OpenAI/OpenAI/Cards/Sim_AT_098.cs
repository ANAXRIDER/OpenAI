﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_098 : SimTemplate //Sideshow Spelleater
    {

        //Battlecry:Copy your opponent's Hero Power.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {

            if (own.own)
            {
                p.ownHeroAblility = new Handmanager.Handcard(p.enemyHeroAblility.card);
                p.ownAbilityReady = true;

            }
            else
            {
                p.enemyHeroAblility = new Handmanager.Handcard(p.ownHeroAblility.card);
                p.enemyAbilityReady = true;
            }
            p.heroPowerActivationsThisTurn = 0;

        }

       

    }
}