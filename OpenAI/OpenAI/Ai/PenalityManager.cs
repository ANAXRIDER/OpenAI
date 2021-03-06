﻿using System;
using System.Collections.Generic;

namespace OpenAI
{
    public sealed class PenalityManager
    {
        //todo acolyteofpain
        //todo better aoe-penality

        private ComboBreaker cb;
        public Dictionary<CardDB.cardName, int> anti_aoe_minion = new Dictionary<CardDB.cardName, int>();
        public Dictionary<CardDB.cardIDEnum, int> TargetAbilitysDatabase = new Dictionary<CardDB.cardIDEnum, int>();
        private Dictionary<CardDB.cardName, int> HealTargetDatabase = new Dictionary<CardDB.cardName, int>();
        private Dictionary<CardDB.cardName, int> HealHeroDatabase = new Dictionary<CardDB.cardName, int>();
        private Dictionary<CardDB.cardName, int> HealAllDatabase = new Dictionary<CardDB.cardName, int>();
        private Dictionary<CardDB.cardName, int> HealCardButCanUseFullHP = new Dictionary<CardDB.cardName, int>();

        public Dictionary<CardDB.cardName, int> DamageAllDatabase = new Dictionary<CardDB.cardName, int>();
        public Dictionary<CardDB.cardName, int> DamageHeroDatabase = new Dictionary<CardDB.cardName, int>(); //not used
        public Dictionary<CardDB.cardName, int> DamageRandomDatabase = new Dictionary<CardDB.cardName, int>();
        public Dictionary<CardDB.cardName, int> DamageAllEnemysDatabase = new Dictionary<CardDB.cardName, int>();

        private Dictionary<CardDB.cardName, int> enrageDatabase = new Dictionary<CardDB.cardName, int>();
        public Dictionary<CardDB.cardName, int> silenceDatabase = new Dictionary<CardDB.cardName, int>();

        public Dictionary<CardDB.cardName, int> heroAttackBuffDatabase = new Dictionary<CardDB.cardName, int>(); //not used
        public Dictionary<CardDB.cardName, int> attackBuffDatabase = new Dictionary<CardDB.cardName, int>();
        private Dictionary<CardDB.cardName, int> healthBuffDatabase = new Dictionary<CardDB.cardName, int>();
        private Dictionary<CardDB.cardName, int> tauntBuffDatabase = new Dictionary<CardDB.cardName, int>();

        public Dictionary<CardDB.cardName, int> lethalHelpers = new Dictionary<CardDB.cardName, int>();
        private Dictionary<CardDB.cardName, int> elementalLastTurnDependentDatabase = new Dictionary<CardDB.cardName, int>();

        private Dictionary<CardDB.cardName, int> backToHandDatabase = new Dictionary<CardDB.cardName, int>();

        private Dictionary<CardDB.cardName, int> TransformCardDatabase = new Dictionary<CardDB.cardName, int>();
        private Dictionary<CardDB.cardName, int> KillCardDatabase = new Dictionary<CardDB.cardName, int>();

        private Dictionary<CardDB.cardName, int> cardDiscardDatabase = new Dictionary<CardDB.cardName, int>();
        private Dictionary<CardDB.cardName, int> discardedCardCount = new Dictionary<CardDB.cardName, int>();
        private Dictionary<CardDB.cardName, int> destroyOwnDatabase = new Dictionary<CardDB.cardName, int>();
        public Dictionary<CardDB.cardName, int> destroyDatabase = new Dictionary<CardDB.cardName, int>();
        public Dictionary<CardDB.cardName, int> buffingMinionsDatabase = new Dictionary<CardDB.cardName, int>();
        public Dictionary<CardDB.cardName, int> buffing1TurnDatabase = new Dictionary<CardDB.cardName, int>();
        public Dictionary<CardDB.cardName, int> randomEffects = new Dictionary<CardDB.cardName, int>();

        private Dictionary<CardDB.cardName, int> silenceTargets = new Dictionary<CardDB.cardName, int>();

        private Dictionary<CardDB.cardName, int> priorityDatabase = new Dictionary<CardDB.cardName, int>(); //minions we want to keep around

        public Dictionary<CardDB.cardName, int> DamageTargetDatabase = new Dictionary<CardDB.cardName, int>();
        public Dictionary<CardDB.cardName, int> DamageTargetSpecialDatabase = new Dictionary<CardDB.cardName, int>();
        public Dictionary<CardDB.cardName, int> cardDrawBattleCryDatabase = new Dictionary<CardDB.cardName, int>();
        public Dictionary<CardDB.cardName, int> cardDrawDeathrattleDatabase = new Dictionary<CardDB.cardName, int>();
        public Dictionary<CardDB.cardName, int> priorityTargets = new Dictionary<CardDB.cardName, int>(); //enemy minions we want to kill
        public Dictionary<CardDB.cardName, int> specialMinions = new Dictionary<CardDB.cardName, int>(); //minions with cardtext, but no battlecry

        public Dictionary<CardDB.cardName, int> discoverCards = new Dictionary<CardDB.cardName, int>();

        private Dictionary<CardDB.cardName, int> strongInspireEffectMinions = new Dictionary<CardDB.cardName, int>();
        public Dictionary<CardDB.cardName, int> summonMinionSpellsDatabase = new Dictionary<CardDB.cardName, int>(); // spells/hero powers that summon minions immediately
        private Dictionary<CardDB.cardName, int> alsoEquipsWeaponDB = new Dictionary<CardDB.cardName, int>(); //cards that aren't weapons but equip one immediately

        public Dictionary<CardDB.cardName, int> AdaptDatabase = new Dictionary<CardDB.cardName, int>();

        public Dictionary<CardDB.cardName, int> NeedSleepBecauseTimingMinionsDB = new Dictionary<CardDB.cardName, int>();
        public float penaltylmanager_penalty = 0;
        private static readonly PenalityManager instance = new PenalityManager();

        private int doomsayerreturn = 0;
        private int cards = 0;

        static PenalityManager()
        {
        } // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit

        public static PenalityManager Instance
        {
            get
            {
                return instance;
            }
        }

        private PenalityManager()
        {
            setupHealDatabase();
            setupEnrageDatabase();
            setupDamageDatabase();
            setupPriorityList();
            setupsilenceDatabase();
            setupAttackBuff();
            setupHealthBuff();

            setupDiscover();
            setupCardDrawBattlecry();
            setupTransformCards();
            setupDiscardCards();
            setupDestroyOwnCards();
            setupSpecialMins();
            setupEnemyTargetPriority();
            setupBuffingMinions();
            setupRandomCards();
            setupLethalHelpMinions();
            setupSilenceTargets();
            setupTargetAbilitys();
            setupStrongInspireMinions();
            setupSummonMinionSpellsDatabase();
            setupAlsoEquipsWeaponDB();
            anti_aoe();
            setupNeedSleepBecauseTimingMinionsDB();
            AdaptMinionDB();
        }

        public void setCombos()
        {
            this.cb = ComboBreaker.Instance;
        }

        public float getAttackWithMininonPenality(Minion m, Playfield p, Minion target, bool lethal)
        {
            float pen = 0;
            pen = getAttackSecretPenality(m, p, target);
            if (!lethal && target.entityID == p.enemyHero.entityID && m.destroyOnOwnTurnEnd && p.enemyMinions.Count >= 1) pen += 50;
            if (!lethal && m.name == CardDB.cardName.bloodimp) pen += 10;
            if (m.name == CardDB.cardName.leeroyjenkins)
            {
                if (!target.own)
                {
                    if (target.name == CardDB.cardName.whelp) return 500;
                }
            }

            bool rockbiterMinion = p.playactions.Find(a => a.actionType == ActionType.PLAY_CARD && a.card.card.name == CardDB.cardName.rockbiterweapon && a.target.entityID == m.entityID) != null;
            if (!lethal && target.isHero && !target.own && rockbiterMinion)
            {
                pen += 50;
            }

            if (!m.silenced && (m.name == CardDB.cardName.acolyteofpain || ((m.name == CardDB.cardName.loothoarder || m.name == CardDB.cardName.bloodmagethalnos || m.name == CardDB.cardName.darkshirelibrarian) && !target.isHero && target.Angr >= m.Hp)))
            {
                pen += p.playactions.Count;  // penalize not utilizing the card draw as early as possible
            }

            bool freezingtrap = false;
            foreach (CardDB.cardIDEnum secretID in p.ownSecretsIDList)
            {
                if (secretID == CardDB.cardIDEnum.EX1_611) //freezing trap
                {
                    freezingtrap = true;
                }
            }

            // todo sepefeets - consider divine shield, killing off small minions, etc.
            if (freezingtrap && target.taunt) //don't kill off our own minions for no reason
            {
                int totalAngr = 0;
                foreach (Minion mnn in p.ownMinions)
                {
                    totalAngr += mnn.Angr;
                }
                if (totalAngr < target.Hp) return 500;
            }
            //Avoid wasting attacks into doomsayer
            /*// must lower HP with spells prior
            if (target.name == CardDB.cardName.doomsayer && !m.poisonous)
            {
                int totalAngr = 0;
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.Ready) totalAngr += mnn.Angr;
                }
                if (p.ownWeaponAttack >= 1) totalAngr += p.ownWeaponAttack;
                if (p.ownHero.tempAttack >= 1) totalAngr += p.ownHero.tempAttack;
                if (totalAngr < target.Hp) return 500;
            }*/

            bool juggleron = false;
            foreach (Minion mnn in p.ownMinions)
            {
                if (mnn.name == CardDB.cardName.knifejuggler && !mnn.silenced) juggleron = true;
            }
            bool acolyte = false;
            int jugglertarget = 0;
            foreach (Minion mnn in p.enemyMinions)
            {
                if (mnn.name == CardDB.cardName.acolyteofpain && !mnn.silenced && mnn.Hp >= 2) acolyte = true;
                if (mnn.Hp == 1 || mnn.divineshild) jugglertarget++;
            }

            if ((m.name == CardDB.cardName.hugetoad || m.name == CardDB.cardName.fierybat) && !m.silenced && p.enemyMinions.Count >= 2)
            {
                if (m.Hp <= target.Angr) pen += (p.playactions.Count) * 0.001f; //attack first
            }

            if (juggleron && !acolyte) pen -= (p.playactions.Count + jugglertarget) * 0.001f;  // to attack with minion later

            if (m.name == CardDB.cardName.darkshirecouncilman && !m.silenced)
            {
            }

            if (target.name == CardDB.cardName.doomsayer && !target.silenced)
            {
                if (m.Angr >= target.Hp)
                {
                    //Helpfunctions.Instance.ErrorLog("doomsayerturn" + doomsayerreturn);
                    if (p.cardsPlayedThisTurn >= 1)
                    {
                        int playmobcardscount = 0;
                        foreach (Action a in p.playactions)
                        {
                            if (a.actionType == ActionType.PLAY_CARD && (a.card.card.type == CardDB.cardtype.MOB || a.card.card.name == CardDB.cardName.callofthewild)) playmobcardscount++;
                        }
                        pen -= doomsayerreturn * playmobcardscount;
                        //Helpfunctions.Instance.ErrorLog("playmobcardscount" + playmobcardscount);
                    }
                }
            }

            if (m.destroyOnOwnTurnEnd && m.Angr > m.handcard.card.Attack)
            {
                if (m.Angr - 4 >= target.Hp) pen += 20;
            }

            if (m.destroyOnEnemyTurnStart && p.enemyMinions.Count >= 1)//corruption
            {
                int StealthMinionCount = 0;
                foreach (Minion mnn in p.enemyMinions)
                {
                    if (mnn.stealth) StealthMinionCount++;
                }
                if (StealthMinionCount < p.enemyMinions.Count)
                {
                    if (target.isHero) pen += m.Angr * 2;
                }
            }

            //if (this.anti_aoe_minion.ContainsKey(m.name))
            //{
            //    if (m.Angr < target.Hp && m.Hp <= target.Angr && !target.isHero && !target.taunt && !priorityTargets.ContainsKey(target.name) && !target.divineshild) //혼자죽음
            //    {
            //        pen += this.anti_aoe_minion[m.name];
            //        //Helpfunctions.Instance.ErrorLog("anti_aoe_minion penalty : " + this.anti_aoe_minion[m.name]);
            //    }
            //    else if (m.Angr < target.Hp && m.Hp <= target.Angr && !target.isHero) //혼자죽음
            //    {
            //        pen += this.anti_aoe_minion[m.name] / 2;
            //    }
            //}

            if (m.name == CardDB.cardName.lepergnome && !m.silenced)
            {
                if (m.Angr < target.Hp && m.Hp <= target.Angr && !target.isHero && !target.divineshild) //혼자죽음
                {
                    pen += 5;
                }
            }

            if (m.name == CardDB.cardName.zealousinitiate && !m.silenced)
            {
                if (m.Hp <= target.Angr && !target.isHero) //죽음
                {
                    pen += p.playactions.Count * 0.05f;
                }
            }

            foreach (Minion mnn in p.enemyMinions)
            {
                if (juggleron && mnn.Hp == target.Hp && !target.taunt && mnn.Angr > target.Angr && !target.isHero && m.Hp <= target.Angr && anti_aoe_minion.ContainsKey(m.name)) pen += mnn.Angr - target.Angr;
            }

            if (target.name == CardDB.cardName.chillmaw && !target.silenced)
            {
                if (m.Angr < target.Hp) pen += 0.05f;
            }

            if (m.tempAttack >= 1 && target.isHero && target.Hp >= 10 && p.enemyMinions.Count >= 1 && !m.handcard.card.isSpecialMinion) pen += m.tempAttack;

            if (m.enemyBlessingOfWisdom >= 1) pen += 10;

            //if (target.name == CardDB.cardName.acolyteofpain && !target.silenced && target.Hp >= 2)
            //{
            //    //Helpfunctions.Instance.ErrorLog("(((((((((((((((((((((((((((((((((((((((");
            //    //if (m.Angr >= target.Hp) pen -= 5;
            //}

            if (m.name == CardDB.cardName.finjatheflyingstar)
            {
                int mulroccnt = 0;
                CardDB.Card c;
                foreach (KeyValuePair<CardDB.cardIDEnum, int> cid in Hrtprozis.Instance.turnDeck)
                {
                    c = CardDB.Instance.getCardDataFromID(cid.Key);
                    if ((TAG_RACE)c.race == TAG_RACE.MURLOC)
                    {
                        mulroccnt++;
                    }
                }
                //Helpfunctions.Instance.ErrorLog("mulroccnt " + mulroccnt);
                //if (mulroccnt >= 1 && target.Hp <= m.Angr) pen -= 10;
            }

            if (p.ownMinions.Find(a => a.name == CardDB.cardName.cultmaster && !a.silenced) != null) //cultmaster draw sequence
            {
                if (!m.divineshild && m.Hp <= target.Angr || target.poisonous) pen += p.playactions.Count * 0.5f;
            }

            return pen;
        }

        private int enfacehp = -142;

        public float getAttackWithHeroPenality(Minion target, Playfield p, bool lethal)
        {
            if (enfacehp == -142) enfacehp = Settings.Instance.enfacehp;
            float retval = 0;

            bool AttacksEnemyHeroCanTrigger = false;
            foreach (SecretItem si in p.enemySecretList)
            {
                if (si.canBe_beartrap || si.canBe_explosive || si.canBe_icebarrier || si.canBe_noblesacrifice) AttacksEnemyHeroCanTrigger = true;
            }
            //Helpfunctions.Instance.ErrorLog("AttacksEnemyHeroCanTrigger" + AttacksEnemyHeroCanTrigger);

            bool rockbiterHero = p.playactions.Find(a => a.actionType == ActionType.PLAY_CARD && a.card.card.name == CardDB.cardName.rockbiterweapon && a.target.entityID == p.ownHero.entityID) != null;

            if (target.name == CardDB.cardName.doomsayer && target.Hp > p.ownHero.Angr) return 10;

            if (target.isHero && !target.own)
            {
                foreach (SecretItem si in Probabilitymaker.Instance.enemySecrets)
                {
                    //Helpfunctions.Instance.ErrorLog("pen" + si.entityId);
                    if (si.canBe_iceblock && !si.canBe_icebarrier)
                    {
                        if (target.Hp + target.armor <= p.ownHero.Angr && p.enemyHero.Hp >= 1)
                        {
                            //pen -= 10 - p.enemyHero.Hp;

                            switch (target.Hp + target.armor)
                            {
                                case 1: return -50; break;
                                case 2: return -30; break;
                                case 3: return -20; break;
                                case 4: return -10; break;
                                default: break;
                            }
                            //Helpfunctions.Instance.ErrorLog("pen" + pen);
                        }
                    }
                }
                //if (SecretItem.Instance.canBe_iceblock)
                //{
                //    if (target.Hp + target.armor <= p.ownHero.Angr && p.enemyHero.Hp >= 1)
                //    {
                //        //pen -= 10 - p.enemyHero.Hp;
                //        switch (target.Hp + target.armor)
                //        {
                //            case 1: return -50; break;
                //            case 2: return -30; break;
                //            case 3: return -20; break;
                //            case 4: return -10; break;
                //            default: break;
                //        }
                //        //Helpfunctions.Instance.ErrorLog("pen" + pen);
                //    }
                //}
            }

            float jugglerpen = 0;
            bool juggleron = false;
            foreach (Minion mnn in p.ownMinions)
            {
                if (mnn.name == CardDB.cardName.knifejuggler && !mnn.silenced) juggleron = true;
            }
            bool acolyte = false;
            foreach (Minion mnn in p.enemyMinions)
            {
                if (mnn.name == CardDB.cardName.acolyteofpain && !mnn.silenced && mnn.Hp >= 2) acolyte = true;
            }
            if (juggleron && !acolyte) jugglerpen -= (p.playactions.Count) * 0.01f;  // to attack with minion later

            if (!lethal && p.ownWeaponName == CardDB.cardName.swordofjustice)
            {
                return 28 + jugglerpen;
            }

            if (!lethal && target.entityID == p.enemyHero.entityID)
            {
                /*
                if (p.ownWeaponAttack >= 1 && p.enemyHero.Hp >= enfacehp)
                {
                    if (p.ownWeaponName == CardDB.cardName.lightsjustice && p.ownWeaponDurability >= 3) return -1;
                    if (!(p.ownHeroName == HeroEnum.thief && p.ownWeaponAttack == 1)) return 50 + p.ownWeaponAttack;
                }*/

                if (p.ownHero.tempAttack > 0 && !p.ownHero.windfury && rockbiterHero)
                {
                    return 50 + jugglerpen;
                }
            }

            if (p.ownWeaponName == CardDB.cardName.eaglehornbow)
            {
                int enemyHeroPowerDFE = 0;
                int ownHeroPowerATK = 0;

                switch (p.enemyHeroAblility.card.name)
                {
                    case CardDB.cardName.lesserheal: enemyHeroPowerDFE += 2; break;
                    case CardDB.cardName.heal: enemyHeroPowerDFE += 4; break;
                    case CardDB.cardName.shapeshift: enemyHeroPowerDFE += 1; break;
                    case CardDB.cardName.direshapeshift: enemyHeroPowerDFE += 2; break;
                    case CardDB.cardName.armorup: enemyHeroPowerDFE += 2; break;
                    case CardDB.cardName.tankup: enemyHeroPowerDFE += 4; break;
                    default: break;
                }
                switch (p.ownHeroAblility.card.name)
                {
                    case CardDB.cardName.steadyshot: ownHeroPowerATK += 2; break;
                    case CardDB.cardName.ballistashot: ownHeroPowerATK += 4; break;
                    default: break;
                }

                bool hasanotherweapon = false;
                int aontherweaponattack = 0;
                int weaponcost = 0;

                if (p.ownSecretsIDList.Count >= 1 && p.ownWeaponDurability == 1) return 3 + jugglerpen;

                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.Secret) return 3 + jugglerpen;
                    if (hc.card.type == CardDB.cardtype.WEAPON)
                    {
                        hasanotherweapon = true;
                        aontherweaponattack += hc.card.Attack;
                        weaponcost = hc.getManaCost(p);
                    }
                }

                //Helpfunctions.Instance.ErrorLog("hasanotherweapon" + hasanotherweapon);

                if (target.isHero && p.ownWeaponDurability >= 2)
                {
                    if (p.enemyHero.Hp + p.enemyHero.armor + enemyHeroPowerDFE <= p.ownWeaponAttack * 2 + ownHeroPowerATK) return 0;
                    //Helpfunctions.Instance.ErrorLog("enemyHeroPowerDFE" + enemyHeroPowerDFE);
                    if (target.isHero && hasanotherweapon)
                    {
                        if (p.enemyMinions.Find(a => a.Hp <= p.ownHero.Angr) != null) return 10;
                        return 0 + jugglerpen;
                    }
                }

                if (target.isHero && hasanotherweapon)
                {
                    if (p.enemyHero.Hp + p.enemyHero.armor + enemyHeroPowerDFE <= p.ownWeaponAttack + aontherweaponattack + ownHeroPowerATK) return 0;

                    if (weaponcost <= p.mana) return p.ownWeaponAttack;
                }

                if (target.isHero && p.enemySecretCount >= 1) return 5 + jugglerpen;
                if (target.isHero && p.ownHero.Hp <= 10) return 5 + jugglerpen;
                if (target.isHero) return 30 + jugglerpen;
                if (!target.isHero && !target.taunt && target.Hp > p.ownHero.Angr) return 2;
            }

            if (p.ownWeaponName == CardDB.cardName.doomhammer)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.name == CardDB.cardName.rockbiterweapon && hc.canplayCard(p)) return 10;
                }
                if (!target.isHero && target.Hp <= p.ownWeaponAttack && p.ownHero.tempAttack > 0) return p.ownHero.Angr - target.Hp;
                if (target.isHero && !target.own && rockbiterHero) return -1; //bonus to not waste rockbiter
            }

            if (p.ownWeaponName == CardDB.cardName.spiritclaws)
            {
                bool hasweapon = false;
                foreach (Handmanager.Handcard c in p.owncards)
                {
                    if (c.card.type == CardDB.cardtype.WEAPON || (alsoEquipsWeaponDB.ContainsKey(c.card.name) && c.card.name != CardDB.cardName.upgrade)) hasweapon = true;
                }
                if (target.isHero && p.ownWeaponAttack == 1 && target.Hp + target.armor >= 3 && !AttacksEnemyHeroCanTrigger && !hasweapon) return 7 + jugglerpen;

                if (target.isHero)
                {
                    if (AttacksEnemyHeroCanTrigger) return jugglerpen;
                    if (p.enemyMinions.Find(a => a.taunt) == null)
                    {
                        if (p.enemyMinions.Find(a => a.Hp <= p.ownHero.Angr) != null) return 5 + jugglerpen;
                    }
                    if (p.ownWeaponDurability == 1 && p.owncards.Find(hc => hc.card.type == CardDB.cardtype.WEAPON) == null && p.ownHero.Hp >= 10 && p.enemyHero.Hp > 5) return 5 + jugglerpen;
                }
                if (p.ownWeaponDurability >= 1)
                {
                    if (hasweapon) retval = 0;
                }
                //if (target.Hp <= p.ownHero.Angr || target.divineshild) retval--;

                if (p.ownHero.Angr >= 3 && target.name == CardDB.cardName.frog && p.ownMinions.Find(a => (a.Ready || !a.playedThisTurn) && a.Angr <= 2 && a.Angr >= 1) != null) retval += 5;

                //healing totem
                if (p.enemyMinions.Find(a => a.name == CardDB.cardName.healingtotem && !a.silenced) != null && target.Hp > p.ownHero.Angr && !target.isHero) retval += 3;
                if (target.Hp > p.ownHero.Angr && !target.isHero && !(target.name == CardDB.cardName.frothingberserker && !target.silenced) && (!target.taunt || (target.taunt && target.Angr >= 1 && !target.handcard.card.Enrage))) retval += 2;

                //wicked witchdoc
                if (p.ownMinions.Find(a => a.name == CardDB.cardName.wickedwitchdoctor && !a.silenced) != null)
                {
                    retval -= p.playactions.Count * 0.5f;
                }
                return retval + jugglerpen;
            }

            if (p.ownWeaponName == CardDB.cardName.jadeclaws)
            {
                bool hasweapon = false;
                foreach (Handmanager.Handcard c in p.owncards)
                {
                    if (c.card.type == CardDB.cardtype.WEAPON || alsoEquipsWeaponDB.ContainsKey(c.card.name)) hasweapon = true;
                }

                if (target.isHero && target.Hp + target.armor >= 5 && !AttacksEnemyHeroCanTrigger && !hasweapon) return 8 + jugglerpen;

                if (target.isHero)
                {
                    if (AttacksEnemyHeroCanTrigger) return jugglerpen;
                    if (p.enemyMinions.Find(a => a.taunt) == null)
                    {
                        if (p.enemyMinions.Find(a => a.Hp <= p.ownHero.Angr) != null) return 5 + jugglerpen;
                    }
                    if (p.ownWeaponDurability == 1 && p.owncards.Find(hc => hc.card.type == CardDB.cardtype.WEAPON) == null && p.ownHero.Hp >= 10 && p.enemyHero.Hp > 5) return 5 + jugglerpen;
                }
                if (p.ownWeaponDurability >= 1)
                {
                    if (hasweapon) retval = 0;
                }
                //if (target.Hp <= p.ownHero.Angr || target.divineshild) retval--;

                if (p.ownHero.Angr >= 2 && target.name == CardDB.cardName.frog && p.ownMinions.Find(a => (a.Ready || !a.playedThisTurn) && a.Angr == 1) != null) retval += 5;

                //healing totem
                if (p.enemyMinions.Find(a => a.name == CardDB.cardName.healingtotem && !a.silenced) != null && target.Hp > p.ownHero.Angr && !target.isHero) retval += 3;
                if (target.Hp > p.ownHero.Angr && !target.isHero && !(target.name == CardDB.cardName.frothingberserker && !target.silenced) && (!target.taunt || (target.taunt && target.Angr >= 1 && !target.handcard.card.Enrage))) retval += 4;
                return retval + jugglerpen;
            }

            //no penalty, but a bonus, if he has weapon on hand!
            if (target.isHero && !target.own && p.ownWeaponName == CardDB.cardName.gorehowl && p.ownWeaponAttack >= 3)
            {
                return 10 + jugglerpen;
            }

            //general weapon
            if (p.ownWeaponDurability >= 1)
            {
                int hasweapon = 0;
                foreach (Handmanager.Handcard c in p.owncards)
                {
                    if (c.card.type == CardDB.cardtype.WEAPON || alsoEquipsWeaponDB.ContainsKey(c.card.name)) hasweapon++;
                }
                if (p.ownHeroAblility.card.name == CardDB.cardName.daggermastery || p.ownHeroAblility.card.name == CardDB.cardName.poisoneddaggers) hasweapon += 2;
                //if (hasweapon && target.name != CardDB.cardName.doomsayer) retval = -p.ownWeaponAttack - 1; // so he doesnt "lose" the weapon in evaluation :D
                if (hasweapon <= 1 && target.isHero && p.ownWeaponDurability == 1 && p.ownWeaponAttack >= 2) retval += p.ownWeaponAttack;
                else if (hasweapon <= 1 && target.isHero && p.ownWeaponName == CardDB.cardName.truesilverchampion && p.ownWeaponDurability >= 2) retval += p.ownWeaponAttack;

                if (hasweapon <= 1 && target.isHero && p.ownWeaponName == CardDB.cardName.truesilverchampion) retval += 4; //another value because heal

                if (hasweapon <= 1 && target.isHero && p.ownMaxMana <= 4) retval += p.ownWeaponAttack;
                if (p.ownWeaponAttack == 1 && hasweapon >= 2)
                {
                    if (p.ownHeroAblility.card.name != CardDB.cardName.daggermastery && p.ownHeroAblility.card.name != CardDB.cardName.poisoneddaggers)
                    {
                        if (p.ownWeaponDurability > 1) retval = 0;
                        else if (p.ownWeaponDurability == 1)
                        {
                            if (target.isHero) retval = 1;
                        }
                    }
                    else // rogue abil
                    {
                        retval = 0;
                    }
                }
            }
            //Helpfunctions.Instance.ErrorLog("retval " + retval);
            // if (p.ownWeaponAttack == 1 && p.ownHeroName == HeroEnum.thief) retval += -1;
            //if (p.ownHero.tempAttack > 0) retval += -5; //bonus to not waste rockbiter

            //Avoid wasting durability into doomsayer
            // must lower HP with spells prior
            /*if (target.name == CardDB.cardName.doomsayer)
            {
                int totalAngr = 0;
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.Ready) totalAngr += mnn.Angr;
                }
                if (p.ownWeaponAttack >= 1) totalAngr += p.ownWeaponAttack;
                if (p.ownHero.tempAttack >= 1) totalAngr += p.ownHero.tempAttack;
                if (totalAngr < target.Hp) return 500;
            }*/
            return retval + jugglerpen;
        }

        // penalize overwriting current weapon for worse ones
        public int getEquipWeaponPenalty(CardDB.Card card, Playfield p, bool lethal)
        {
            if (p.ownWeaponDurability < 1) return 0;
            if (card.type != CardDB.cardtype.WEAPON && !alsoEquipsWeaponDB.ContainsKey(card.name)) return 0;
            /*
            if (card.type == CardDB.cardtype.WEAPON && card.Attack < p.ownWeaponAttack)
            {
                return 25 * ((p.ownWeaponAttack * p.ownWeaponDurability) - (2 * card.Attack)); //I think this case is already handled elsewhere
            }*/
            if (alsoEquipsWeaponDB.ContainsKey(card.name) && alsoEquipsWeaponDB[card.name] < p.ownWeaponAttack)
            {
                if (card.name == CardDB.cardName.upgrade)
                {
                    if (p.ownWeaponAttack >= 3 || p.ownWeaponDurability >= 2) return 0;
                }
                return 25 * ((p.ownWeaponAttack * p.ownWeaponDurability) - (2 * alsoEquipsWeaponDB[card.name]));
            }
            return 0;
        }

        public float getPlayCardPenality(Handmanager.Handcard hcard, Minion target, Playfield p, int choice, bool lethal)
        {
            float retval = 0;
            CardDB.Card card = hcard.card;
            CardDB.cardName name = card.name;
            //there is no reason to buff HP of minon (because it is not healed)

            int abuff = getAttackBuffPenality(hcard, target, p, choice, lethal);
            int tbuff = getTauntBuffPenality(hcard, target, p, choice);
            if (name == CardDB.cardName.markofthewild && ((abuff >= 500 && tbuff == 0) || (abuff == 0 && tbuff >= 500)))
            {
                retval = 0;
            }
            else
            {
                retval += abuff + tbuff;
            }
            retval += getHPBuffPenality(card, target, p, choice);
            retval += getSilencePenality(name, target, p, choice, lethal);
            retval += getDamagePenality(name, target, p, choice, lethal);
            retval += getHealPenality(name, target, p, choice, lethal);

            retval += getCardDrawPenality(name, target, p, choice, lethal);
            retval += getCardDrawofEffectMinions(card, p);
            retval += getCardDiscardPenality(name, p, lethal);
            retval += getDestroyOwnPenality(name, target, p, lethal);

            retval += getDestroyPenality(name, target, p, lethal);
            retval += getbackToHandPenality(name, target, p, lethal);
            retval += getSpecialCardComboPenalties(hcard, target, p, lethal, choice);
            //if (lethal) Console.WriteLine(retval+ " " + name);
            retval += getRandomPenalty(card, p, target);
            retval += getEquipWeaponPenalty(card, p, lethal);

            if (!lethal)
            {
                retval += cb.GetPenalityForDestroyingCombo(card, p);
                retval += cb.GetPlayValue(card.cardIDenum);
                retval += getPlayInspirePenalty(hcard, p);
                retval += getPlayMobPenalty(hcard, target, p, lethal);

                retval += getPlaySpellPenalty(hcard, target, p, lethal);

                if (card.name == p.ownHeroAblility.card.name) retval += getHeroPowerPenality(hcard, target, p);
            }

            retval += getPlaySecretPenalty(card, p);
            retval += getPlayCardSecretPenality(card, p);

            retval += getPlayPenalty(name, p, choice);
            retval += card.pen_card.getPlayPenalty(p, hcard, target, choice, lethal);

            retval += getPlayOrder(name, p, choice);

            //Helpfunctions.Instance.ErrorLog("retval " + retval);
            penaltylmanager_penalty += retval;

            return retval;
        }

        private int getAttackBuffPenality(Handmanager.Handcard playhc, Minion target, Playfield p, int choice, bool lethal)
        {
            CardDB.Card card = playhc.card;
            CardDB.cardName name = card.name;
            if (name == CardDB.cardName.darkwispers && choice != 1) return 0;

            if (!lethal && (card.name == CardDB.cardName.bolster))
            {
                int targets = 0;
                foreach (Minion m in p.ownMinions)
                {
                    if (m.taunt) targets++;
                }
                if (targets < 2)
                {
                    return 10;
                }
            }

            if (!lethal && (card.name == CardDB.cardName.savageroar || card.name == CardDB.cardName.bloodlust))
            {
                int pen = 20;
                int targets = 0;

                if (p.enemySecretCount == 0 && p.ownMinions.Count + p.playactions.Count >= 8) return 550;

                foreach (Minion m in p.ownMinions)
                {
                    if (m.Ready) targets++;
                    if (p.enemySecretCount == 0 && m.allreadyAttacked) return 550;
                }

                if ((p.ownHero.Ready || p.ownHero.numAttacksThisTurn == 0) && card.name == CardDB.cardName.savageroar) targets++;

                pen -= targets * 4;
                pen = Math.Max(0, pen);
                return pen;
            }

            if (!this.attackBuffDatabase.ContainsKey(name)) return 0;
            if (target == null)
            {
                //if ((p.ownMaxMana <= 2 && (p.enemyHeroName == HeroEnum.mage || p.enemyHeroName == HeroEnum.hunter)))
                //    return 10;
                if (card.type == CardDB.cardtype.MOB)
                {
                    if (card.name == CardDB.cardName.metaltoothleaper && p.ownMinions.Find(mech => mech.handcard.card.race == TAG_RACE.MECHANICAL) != null) return 0;
                    return 4 * attackBuffDatabase[name];
                }

                return 10;
            }

            if (target.own && name == CardDB.cardName.rockbiterweapon)
            {
                if (p.playactions.Find(a => a.actionType == ActionType.ATTACK_WITH_HERO) != null) return 500;
                if (!target.Ready && !target.isHero) return 500;
                if (!target.isHero && p.ownHero.windfury) return 30;
                return 10;
            }

            if (!target.isHero && !target.own)
            {
                if (card.type == CardDB.cardtype.MOB && p.ownMinions.Count == 0) return 0;
                //allow it if you have biggamehunter
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.name == CardDB.cardName.biggamehunter && target.Angr <= 6 && p.mana >= (hc.getManaCost(p) + playhc.getManaCost(p))) return 5;
                    if (hc.card.name == CardDB.cardName.shadowworddeath && target.Angr <= 4 && p.mana >= (hc.getManaCost(p) + playhc.getManaCost(p))) return 5;
                }
                if (card.name == CardDB.cardName.crueltaskmaster || card.name == CardDB.cardName.innerrage)
                {
                    Minion m = target;

                    if (m.Hp == 1)
                    {
                        return 0;
                    }

                    if (!m.wounded && (m.Angr >= 4 || m.Hp >= 5))
                    {
                        foreach (Handmanager.Handcard hc in p.owncards)
                        {
                            if (hc.card.name == CardDB.cardName.execute && p.mana >= (hc.getManaCost(p) + playhc.getManaCost(p))) return 0;
                        }
                    }

                    return 30;
                }
                else
                {
                    return 500;
                }
            }

            if (!target.isHero && target.own)
            {
                Minion m = target;
                bool hasownready = false;

                //vs mage or hunter we need board presence at early game? so we skip the minion ready-check.
                // for everyone else, we penalize buffing minions when they are not ready

                if (p.ownMaxMana > 1)// || (p.enemyHeroName != HeroEnum.mage && p.enemyHeroName != HeroEnum.hunter))
                {
                    if (card.name == CardDB.cardName.clockworkknight || card.name == CardDB.cardName.screwjankclunker)
                    {
                        // hasownready can only apply to mechs
                        hasownready = p.ownMinions.Find(mnn => mnn.handcard.card.race == TAG_RACE.MECHANICAL && mnn.Ready) != null;
                    }
                    else
                    {
                        foreach (Minion mnn in p.ownMinions)
                        {
                            if (mnn.Ready)
                            {
                                hasownready = true;
                                break;
                            }
                        }
                        if (p.playactions.Find(a => a.actionType == ActionType.ATTACK_WITH_MINION) != null) hasownready = true; //we HAD something ready but already attacked with it so penalize it still
                    }
                }

                if (this.buffing1TurnDatabase.ContainsKey(name))
                {
                    //if (m.Ready)
                    //{
                    //    //if (this.priorityDatabase.ContainsKey(m.name)) return 5 + priorityDatabase[m.name];
                    //    return m.taunt ? 0 : 0;
                    //}
                    //else
                    //{
                    //    return (hasownready) ? 50 : 0;
                    //}
                }
                if (!m.Ready)
                {
                    //if (!m.taunt && hasownready) return 5 * attackBuffDatabase[name];
                    //else return attackBuffDatabase[name];
                }

                if (m.Hp == 1 && !m.divineshild && !this.buffing1TurnDatabase.ContainsKey(name))
                {
                    if (this.healthBuffDatabase.ContainsKey(name)) return 0;  // m.Hp no longer == 1
                    if (card.type == CardDB.cardtype.MOB) return 2 * attackBuffDatabase[name] + 1;  // only 1pt worse than playing vanilla minion with same stats and no atk buff

                    return 2;
                }
                if (card.name == CardDB.cardName.blessingofmight) return 6;
            }

            return 0;
        }

        private int getHPBuffPenality(CardDB.Card card, Minion target, Playfield p, int choice)
        {
            CardDB.cardName name = card.name;
            if (name == CardDB.cardName.darkwispers && choice != 1) return 0;
            //buff enemy?
            if (!this.healthBuffDatabase.ContainsKey(name)) return 0;
            if (target == null)
            {
                // penalize for lost buff
                if (card.type == CardDB.cardtype.MOB) return healthBuffDatabase[name];
            }

            if (target != null && !target.own && !this.tauntBuffDatabase.ContainsKey(name))
            {
                return 500;
            }

            return 0;
        }

        private int getTauntBuffPenality(Handmanager.Handcard hcard, Minion target, Playfield p, int choice)
        {
            int pen = 0;
            //buff enemy?
            if (!this.tauntBuffDatabase.ContainsKey(hcard.card.name)) return 0;
            if (hcard.card.name == CardDB.cardName.markofnature && choice != 2) return 0;
            if (hcard.card.name == CardDB.cardName.darkwispers && choice != 1) return 0;
            if (target == null) return 20;
            if (!target.isHero && !target.own)
            {
                //allow it if you have black knight
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.name == CardDB.cardName.theblackknight && (p.mana >= hcard.getManaCost(p) + hc.getManaCost(p))) return 0;
                }

                // allow taunting if target is priority and others have taunt
                bool enemyhasTaunts = false;
                foreach (Minion mnn in p.enemyMinions)
                {
                    if (mnn.taunt)
                    {
                        enemyhasTaunts = true;
                        break;
                    }
                }
                if (enemyhasTaunts && this.priorityDatabase.ContainsKey(target.name) && !target.silenced && !target.taunt)
                {
                    return 0;
                }

                pen = 500;
            }

            return pen;
        }

        private float getSilencePenality(CardDB.cardName name, Minion target, Playfield p, int choice, bool lethal)
        {
            int pen = 0;
            if (name == CardDB.cardName.keeperofthegrove && choice != 2) return 0; // look at damage penality in this case

            if (target == null)
            {
                if (name == CardDB.cardName.ironbeakowl || name == CardDB.cardName.spellbreaker)
                {
                    return 30 - (3 * p.ownMaxMana);
                }
                return 0;
            }

            if (target.own)
            {
                if (this.silenceDatabase.ContainsKey(name))
                {
                    if ((!target.silenced && (target.name == CardDB.cardName.wrathguard || target.name == CardDB.cardName.darnassusaspirant || target.name == CardDB.cardName.icehowl || target.name == CardDB.cardName.venturecomercenary)))
                    {
                        return 0;
                    }

                    // no pen if own is enrage
                    if ((!target.silenced && (target.name == CardDB.cardName.darnassusaspirant ||
                        target.name == CardDB.cardName.ancientwatcher ||
                        target.name == CardDB.cardName.humongousrazorleaf ||
                        target.name == CardDB.cardName.humongousrazorleaf ||
                        target.name == CardDB.cardName.eeriestatue ||
                        target.name == CardDB.cardName.mogortheogre ||
                        target.name == CardDB.cardName.animagolem)) ||
                        target.Angr < target.handcard.card.Attack ||
                        target.maxHp < target.handcard.card.Health ||
                        (target.frozen && !target.playedThisTurn && target.numAttacksThisTurn == 0))
                    {
                        return 0;
                    }

                    if (target.destroyOnOwnTurnEnd || target.destroyOnOwnTurnStart)
                    {
                        return 0;
                    }

                    pen += 500;
                    if (name == CardDB.cardName.ironbeakowl || name == CardDB.cardName.spellbreaker)
                    {
                        return 30 - (3 * p.ownMaxMana);
                    }
                }
            }

            if (!target.own)
            {
                if (this.silenceDatabase.ContainsKey(name))
                {
                    // no pen if own is enrage
                    Minion m = target;//

                    if (!m.silenced && (m.name == CardDB.cardName.ancientwatcher || m.name == CardDB.cardName.ragnarosthefirelord))
                    {
                        return 500;
                    }

                    if ((!target.silenced && (target.name == CardDB.cardName.wrathguard || target.name == CardDB.cardName.darnassusaspirant || target.name == CardDB.cardName.icehowl)))
                    {
                        return 100;
                    }

                    if (lethal)
                    {
                        //during lethal we only silence taunt, or if its a mob (owl/spellbreaker) + we can give him charge
                        if (m.taunt || (name == CardDB.cardName.ironbeakowl && (p.ownMinions.Find(x => x.name == CardDB.cardName.tundrarhino) != null || p.owncards.Find(x => x.card.name == CardDB.cardName.charge) != null)) || (name == CardDB.cardName.spellbreaker && p.owncards.Find(x => x.card.name == CardDB.cardName.charge) != null)) return 0; // || p.ownMinions.Find(x => x.name == CardDB.cardName.warsongcommander) != null

                        return 500;
                    }

                    if (m.handcard.card.name == CardDB.cardName.dancingswords && !m.silenced)
                    {
                        return 50;
                    }

                    if (m.handcard.card.name == CardDB.cardName.venturecomercenary && !m.silenced && (m.Angr <= m.handcard.card.Attack && m.maxHp <= m.handcard.card.Health))
                    {
                        return 30;
                    }

                    if (m.handcard.card.name == CardDB.cardName.quartermaster && (p.enemyHeroAblility.card.cardIDenum != CardDB.cardIDEnum.AT_132_PALADIN && p.enemyHeroAblility.card.cardIDenum != CardDB.cardIDEnum.CS2_101))
                    {
                        return 30;
                    }

                    if (priorityDatabase.ContainsKey(m.name) && !m.silenced)
                    {
                        return 0;
                    }

                    if (this.silenceTargets.ContainsKey(m.name) && !m.silenced)
                    {
                        return 0;
                    }

                    if (m.handcard.card.deathrattle && !m.silenced)
                    {
                        if (m.handcard.card.name == CardDB.cardName.zombiechow) return 15;
                        return 0;
                    }

                    //silence nothing
                    //todo add "new" enchantments (good or bad ones)
                    if (m.Angr <= m.handcard.card.Attack && m.maxHp <= m.handcard.card.Health && !m.taunt && !m.windfury && !m.divineshild && !m.poisonous && !this.specialMinions.ContainsKey(name))
                    {
                        if (name == CardDB.cardName.keeperofthegrove) return 500;
                        return 30;
                    }

                    return 5;
                }
            }

            return pen;
        }

        private float getDamagePenality(CardDB.cardName name, Minion target, Playfield p, int choice, bool lethal)
        {
            float pen = 0;

            if (name == CardDB.cardName.shieldslam && p.ownHero.armor == 0) return 500;
            if (name == CardDB.cardName.savagery && p.ownHero.Angr == 0) return 500;
            if (name == CardDB.cardName.keeperofthegrove && choice != 1) return 0; // look at silence penality
            if (name == CardDB.cardName.livingroots && choice != 2) return 0; // look at silence penality
            //if (name == CardDB.cardName.swipe && !target.own) return -p.enemyMinions.Count; // treat it as single target spell with bonus for aoe

            if (this.DamageAllDatabase.ContainsKey(name) || (p.anzOwnAuchenaiSoulpriest >= 1 && HealAllDatabase.ContainsKey(name))) // aoe penality
            {
                if (name == CardDB.cardName.ravagingghoul)
                {
                    return 5;
                }

                if (p.enemyMinions.Count == 0) return 300;

                foreach (Minion m in p.enemyMinions)
                {
                    if ((m.Angr >= 4 || m.Hp >= 5) && !m.wounded)
                    {
                        foreach (Handmanager.Handcard hc in p.owncards)
                        {
                            if (hc.card.name == CardDB.cardName.execute) return 0;
                        }
                    }
                }

                if (name == CardDB.cardName.demonwrath)
                {
                    int ownmins = 0;
                    int enemymins = 0;

                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.handcard.card.race != TAG_RACE.DEMON) ownmins++;
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (m.handcard.card.race != TAG_RACE.DEMON) enemymins++;
                    }

                    if (enemymins <= 1 || enemymins + 1 <= ownmins || ownmins >= 3)
                    {
                        return 30;
                    }
                }

                if (name == CardDB.cardName.sleepwiththefishes)
                {
                    int ret = 12;
                    int owntargetcnt = 0;
                    int enemytargetcnt = 0;
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.wounded) owntargetcnt++;
                    }
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (m.wounded) enemytargetcnt++;
                    }
                    ret = (owntargetcnt - enemytargetcnt) * 4;
                    return ret;
                }

                if (p.enemyMinions.Count <= 2)
                {
                    return 20 * (2 - p.enemyMinions.Count);
                }
            }

            if (this.DamageAllEnemysDatabase.ContainsKey(name)) // aoe penality
            {
                if (p.enemyMinions.Count == 0) return 300;
                foreach (Minion m in p.enemyMinions)
                {
                    if ((m.Angr >= 4 || m.Hp >= 5) && !m.wounded)
                    {
                        foreach (Handmanager.Handcard hc in p.owncards)
                        {
                            if (hc.card.name == CardDB.cardName.execute) return 0;
                        }
                    }
                }

                if (name == CardDB.cardName.holynova)
                {
                    int targets = p.enemyMinions.Count;
                    foreach (Minion m in p.ownMinions)
                    {
                        if (m.wounded) targets++;
                    }
                    if (targets <= 2)
                    {
                        return 20;
                    }
                }
                if (name == CardDB.cardName.darkironskulker)
                {
                    int targets = p.enemyMinions.Count;
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (!m.wounded) targets++;
                    }
                    if (targets <= 2)
                    {
                        return 20;
                    }
                }

                if (name == CardDB.cardName.consecration)
                {
                    int targets = p.enemyMinions.Count;
                    if (p.enemyHero.Hp <= p.spellpower + 2) return 0;
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (priorityTargets.ContainsKey(m.name) && priorityTargets[m.name] >= 10) return 0;
                    }
                    if (targets <= 2)
                    {
                        return 5 + 20 * (2 - p.enemyMinions.Count);
                    }
                    else return 5;
                }

                if (name == CardDB.cardName.dreadinfernal)
                {
                    int targets = p.enemyMinions.Count;
                    if (targets == 0)
                    {
                        return 5;
                    }
                }

                if (name == CardDB.cardName.lightningstorm)
                {
                    int targets = p.enemyMinions.Count;
                    if (p.enemyHeroName == HeroEnum.shaman) return 20 + p.playactions.Count * 0.1f - targets;
                    if (p.enemyMinions.Count <= 2)
                    {
                        return 20 * (2 - p.enemyMinions.Count) + 10 + p.playactions.Count * 0.1f - targets;
                    }
                    return 10 + p.playactions.Count * 0.1f - targets;
                }

                if (name == CardDB.cardName.maelstromportal)
                {
                    if (p.enemyMinions.Count == 0) return 500;
                    if (p.enemyMinions.Count == 1 && p.ownMinions.Count >= 5) return 15 + p.playactions.Count * 0.5f;

                    int targets = p.enemyMinions.Count;

                    return 14 - targets + p.playactions.Count * 0.5f;
                }

                if (p.enemyMinions.Count <= 2)
                {
                    return 20 * (2 - p.enemyMinions.Count);
                }
                return 0;
            }

            if (target == null) return 0;

            if (target.own && target.isHero)
            {
                if (DamageTargetDatabase.ContainsKey(name) || DamageTargetSpecialDatabase.ContainsKey(name) || (p.anzOwnAuchenaiSoulpriest >= 1 && HealTargetDatabase.ContainsKey(name)))
                {
                    pen = 500;
                }
            }

            if (!lethal && !target.own && target.isHero)
            {
                if (name == CardDB.cardName.baneofdoom)
                {
                    pen = 500;
                }
                else if (name == CardDB.cardName.lavashock && p.owedRecall == 0 && p.currentRecall == 0) //todo sepefeets - eternal sentinal penalty
                {
                    pen = 20;
                }
                else if (name == CardDB.cardName.mortalstrike && p.ownHero.Hp > 12) pen = 30;
                else if (name == CardDB.cardName.soulfire)
                {
                    pen = 30;// && p.enemyHero.Hp > 4 + p.spellpower) pen = 50;
                    bool hasdiscardsynergy = false;
                    foreach (Handmanager.Handcard hdc in p.owncards)
                    {
                        if (hdc.card.name == CardDB.cardName.fistofjaraxxus || hdc.card.name == CardDB.cardName.tinyknightofevil || hdc.card.name == CardDB.cardName.silverwaregolem) hasdiscardsynergy = true;
                    }
                    if (hasdiscardsynergy) pen = 8;
                    if (target.Hp <= 10) pen = 8;
                }

                //else pen = 40;

                if (name == CardDB.cardName.quickshot && p.owncards.Count == 1)
                {
                    if (p.enemyMinions.Count == 0 || p.enemyHero.Hp + p.enemyHero.armor <= 10) pen = -10;
                    pen += 10 + p.playactions.Count * 0.1f;
                }

                if (name == CardDB.cardName.killcommand)
                {
                    pen = 30;
                    bool haspet = false;
                    foreach (Minion mnn in p.ownMinions)
                    {
                        if ((TAG_RACE)mnn.handcard.card.race == TAG_RACE.BEAST) haspet = true;
                    }
                    if (haspet && p.enemyHero.Hp + p.enemyHero.armor <= 10) pen -= 20;
                }

                if (name == CardDB.cardName.lavaburst) pen += 30;
                if (name == CardDB.cardName.lightningbolt) pen += 30;
                if (name == CardDB.cardName.jadelightning) pen += 10;
                if (name == CardDB.cardName.eviscerate) pen += 20;
            }

            if (target.own && !target.isHero)
            {
                if (DamageTargetDatabase.ContainsKey(name) || (p.anzOwnAuchenaiSoulpriest >= 1 && HealTargetDatabase.ContainsKey(name)))
                {
                    // no pen if own is enrage
                    Minion m = target;

                    //standard ones :D (mostly carddraw
                    if (enrageDatabase.ContainsKey(m.name) && !m.wounded && m.Ready)
                    {
                        return 5;
                    }

                    // no pen if we have battlerage for example
                    int dmg = this.DamageTargetDatabase.ContainsKey(name) ? this.DamageTargetDatabase[name] : this.HealTargetDatabase[name];

                    if (m.name == CardDB.cardName.madscientist && p.ownHeroName == HeroEnum.hunter) return 500;
                    if (m.name == CardDB.cardName.sylvanaswindrunner) return 0;
                    if (m.handcard.card.deathrattle) return 25;
                    if (m.Hp > dmg)
                    {
                        if (m.name == CardDB.cardName.acolyteofpain && p.owncards.Count <= 3) return 0;
                        foreach (Handmanager.Handcard hc in p.owncards)
                        {
                            if (hc.card.name == CardDB.cardName.battlerage) return pen;
                            if (hc.card.name == CardDB.cardName.rampage) return pen;
                        }
                    }

                    if (m.name == CardDB.cardName.grimpatron) return 0;

                    pen = 500;
                }

                //special cards
                if (DamageTargetSpecialDatabase.ContainsKey(name))
                {
                    int dmg = DamageTargetSpecialDatabase[name];
                    Minion m = target;
                    if ((name == CardDB.cardName.crueltaskmaster || name == CardDB.cardName.innerrage) && m.Hp >= 2) return 0;
                    if ((name == CardDB.cardName.demonfire || name == CardDB.cardName.demonheart) && (TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) return 0;
                    if (name == CardDB.cardName.earthshock && m.Hp >= 2)
                    {
                        if ((!m.silenced && (m.name == CardDB.cardName.ancientwatcher || m.name == CardDB.cardName.ragnarosthefirelord)) || m.Angr < m.handcard.card.Attack || m.maxHp < m.handcard.card.Health || (m.frozen && !m.playedThisTurn && m.numAttacksThisTurn == 0))
                            return 0;
                        if (priorityDatabase.ContainsKey(m.name) && !m.silenced)
                        {
                            return 500;
                        }
                    }
                    if (name == CardDB.cardName.earthshock)//dont silence other own minions
                    {
                        return 500;
                    }
                    if (name == CardDB.cardName.lavashock)//dont do this on own minions (you can do it on enemy hero
                    {
                        return 500;
                    }
                    // no pen if own is enrage
                    if (enrageDatabase.ContainsKey(m.name) && !m.wounded && m.Ready)
                    {
                        return pen;
                    }

                    // no pen if we have battlerage for example

                    if (m.Hp > dmg)
                    {
                        foreach (Handmanager.Handcard hc in p.owncards)
                        {
                            if (hc.card.name == CardDB.cardName.battlerage) return pen;
                            if (hc.card.name == CardDB.cardName.rampage) return pen;
                        }
                    }

                    if (p.ownMinions.Count == 7) return 5;

                    pen = 500;
                }
            }
            if (!target.own && !target.isHero)
            {
                if (target.poisonous) return 0;
                if (DamageTargetSpecialDatabase.ContainsKey(name) || DamageTargetDatabase.ContainsKey(name))
                {
                    Minion m = target;
                    if (name == CardDB.cardName.soulfire && m.maxHp <= 2 && m.Angr <= 2) pen = 10;

                    if (name == CardDB.cardName.baneofdoom && m.Hp >= 3) pen = 10;

                    if (name == CardDB.cardName.shieldslam) pen = 12;

                    if (name == CardDB.cardName.lavashock && p.owedRecall == 0 && p.currentRecall == 0) pen = 15;

                    if (name == CardDB.cardName.fireblast && !lethal && m.Hp == 1 && m.Angr >= 1) pen = -1;

                    if (name == CardDB.cardName.mortalstrike && p.ownHero.Hp > 12) pen = 12;

                    if (name == CardDB.cardName.quickshot && p.owncards.Count == 1) pen = p.playactions.Count * 0.1f;

                    if (name == CardDB.cardName.killcommand)
                    {
                        //pen = 0;
                        bool haspet = false;
                        foreach (Minion mnn in p.ownMinions)
                        {
                            if ((TAG_RACE)mnn.handcard.card.race == TAG_RACE.BEAST) haspet = true;
                        }
                        //Helpfunctions.Instance.ErrorLog("haspet" + haspet);
                        if (!haspet)
                        {
                            if (3 + p.spellpower < m.Hp) pen += 5;
                        }
                        //else
                        //{
                        //    if(target.Hp > p.spellpower + 5) pen += 10;
                        //}

                        pen += 15;

                        if (p.enemyHero.Hp + p.enemyHero.Angr <= 8) pen += 10;
                    }

                    if (name == CardDB.cardName.lavaburst)
                    {
                        pen += 15;
                    }
                    if (name == CardDB.cardName.lightningbolt)
                    {
                        pen += 8;
                        if (target.Angr == 0) pen += 2;
                        if (p.spellpower + 3 >= target.Hp && target.Angr >= 4) pen = 0;
                    }

                    if (name == CardDB.cardName.jadelightning)
                    {
                        pen += 6;
                        if (target.Angr == 0) pen += 4;
                    }

                    if (name == CardDB.cardName.eviscerate)
                    {
                        pen += 8;
                    }

                    if (name == CardDB.cardName.wrath) pen += 2;

                    if (target.name == CardDB.cardName.doomsayer && pen >= 5)
                    {
                        int enemydoomsayerMaxHp = 0;
                        foreach (Minion min in p.enemyMinions)
                        {
                            if (min.name == CardDB.cardName.doomsayer && enemydoomsayerMaxHp <= min.Hp) enemydoomsayerMaxHp = min.Hp;
                        }
                        if (DamageTargetDatabase.ContainsKey(name) && DamageTargetDatabase[name] >= enemydoomsayerMaxHp) return 0;
                        if (DamageTargetSpecialDatabase.ContainsKey(name) && DamageTargetSpecialDatabase[name] >= enemydoomsayerMaxHp) return 0;
                    }
                }
            }

            return pen;
        }

        private int getHealPenality(CardDB.cardName name, Minion target, Playfield p, int choice, bool lethal)
        {
            if (!HealTargetDatabase.ContainsKey(name) && !HealHeroDatabase.ContainsKey(name) && !HealAllDatabase.ContainsKey(name)) return 0;
            //Todo healpenality for aoe heal
            //todo auchenai soulpriest
            if (p.anzOwnAuchenaiSoulpriest >= 1) return 0;
            if (name == CardDB.cardName.ancientoflore && choice != 2) return 0;
            int pen = 0;
            int offset = 0;
            int heal = 0;
            /*if (HealHeroDatabase.ContainsKey(name))
            {
                heal = HealHeroDatabase[name];
                if (target == 200) pen = 500; // dont heal enemy
                if ((target == 100 || target == -1) && p.ownHeroHp + heal > 30) pen = p.ownHeroHp + heal - 30;
            }*/

            // small bonus if we have heal-trigger, big penalty if enemy does
            //foreach (Minion mnn in p.ownMinions)
            //{
            //    if (mnn.name == CardDB.cardName.northshirecleric) offset -= 20;
            //    if (mnn.name == CardDB.cardName.lightwarden) offset -= 20;
            //    if (mnn.name == CardDB.cardName.holychampion) offset -= 50;
            //}
            //foreach (Minion mnn in p.enemyMinions)
            //{
            //    if (mnn.name == CardDB.cardName.northshirecleric) offset += 30;
            //    if (mnn.name == CardDB.cardName.lightwarden) offset += 30;
            //    if (mnn.name == CardDB.cardName.holychampion) offset += 75;
            //}

            if (name == CardDB.cardName.treeoflife)
            {
                int mheal = 0;
                int wounded = 0;
                //int eheal = 0;
                if (p.ownHero.wounded) wounded++;
                foreach (Minion mi in p.ownMinions)
                {
                    mheal += Math.Min((mi.maxHp - mi.Hp), 4);
                    if (mi.wounded) wounded++;
                }
                //Console.WriteLine(mheal + " circle");
                if (p.ownHero.Hp < 16)
                {
                    pen += p.ownHero.Hp;
                    if (p.guessHeroDamage(false) >= p.ownHero.Hp + p.ownHero.armor) return (pen * 2) + offset;
                    else return (pen * 10) + offset;
                }
                else
                {
                    pen = (p.ownHero.Hp * 15) + 20 - mheal;
                    return pen + offset;
                }
            }

            if (name == CardDB.cardName.renojackson)
            {
                if (p.ownHero.Hp < 16)
                {
                    pen = p.ownHero.Hp;
                    if (p.guessHeroDamage(false) >= p.ownHero.Hp + p.ownHero.armor) return (pen * 2) + offset;
                    else return (pen * 10) + offset;
                }
                else
                {
                    pen = p.ownHero.Hp * 15;
                    //extra penalty if we can draw cards
                    if (p.ownAbilityReady && cardDrawBattleCryDatabase.ContainsKey(p.ownHeroAblility.card.name)) pen += 20;
                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        if (!cardDrawBattleCryDatabase.ContainsKey(hc.card.name)) continue;
                        pen += 20;
                        break;
                    }
                    return pen + offset;
                }
            }

            if (name == CardDB.cardName.circleofhealing)
            {
                int mheal = 0;
                int wounded = 0;
                //int eheal = 0;
                foreach (Minion mi in p.ownMinions)
                {
                    mheal += Math.Min((mi.maxHp - mi.Hp), 4);
                    if (mi.wounded) wounded++;
                }
                //Console.WriteLine(mheal + " circle");
                if (mheal == 0) return 500 + offset;
                if (mheal <= 7 && wounded <= 2) return 20 + offset;
            }

            if (HealTargetDatabase.ContainsKey(name) || HealHeroDatabase.ContainsKey(name))
            {
                if (HealHeroDatabase.ContainsKey(name))
                {
                    target = p.ownHero;
                    heal = HealHeroDatabase[name];
                }
                else
                {
                    if (target == null) return 10 + offset;
                    //Helpfunctions.Instance.ErrorLog("pencheck for " + name + " " + target.entitiyID + " " + target.isHero  + " " + target.own);
                    heal = HealTargetDatabase[name];
                }
                if (target.isHero && !target.own) return 510 + offset; // dont heal enemy
                //Helpfunctions.Instance.ErrorLog("pencheck for " + name + " " + target.entitiyID + " " + target.isHero + " " + target.own);
                if ((target.isHero && target.own) && p.ownHero.Hp == 30 && !HealCardButCanUseFullHP.ContainsKey(name)) return 150 + offset;
                if ((target.isHero && target.own) && p.ownHero.Hp + heal > 30 && !HealCardButCanUseFullHP.ContainsKey(name)) pen = p.ownHero.Hp + heal - 30;
                if (name == CardDB.cardName.lesserheal || name == CardDB.cardName.heal) pen = 0;
                Minion m = new Minion();

                if (!target.isHero && target.own)
                {
                    m = target;
                    int wasted = 0;
                    if (m.Hp == m.maxHp) return 500 + offset;
                    if (m.Hp + heal - 1 > m.maxHp) wasted = m.Hp + heal - m.maxHp;
                    pen = wasted;

                    if (HealCardButCanUseFullHP.ContainsKey(name)) pen = 0;
                    //if (m.taunt && wasted <= 2 && m.Hp < m.maxHp) pen -= 5; // if we heal a taunt, its good :D

                    if (m.Hp + heal <= m.maxHp) pen = -1;
                }

                if (!target.isHero && !target.own)
                {
                    m = target;
                    if (m.Hp == m.maxHp) return 500 + offset;
                    // no penality if we heal enrage enemy
                    if (enrageDatabase.ContainsKey(m.name))
                    {
                        return pen + offset;
                    }

                    // no pen if we have slam

                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        if (hc.card.name == CardDB.cardName.slam && m.Hp < 2) return pen + offset;
                        if (hc.card.name == CardDB.cardName.backstab) return pen + offset;
                    }

                    pen = 500;
                }
            }

            return pen + offset;
        }

        private float getCardDrawPenality(CardDB.cardName name, Minion target, Playfield p, int choice, bool lethal)
        {
            // penalty if carddraw is late or you have enough cards
            if (!cardDrawBattleCryDatabase.ContainsKey(name)) return 0;
            if (name == CardDB.cardName.ancientoflore && choice != 1) return 0;
            if (name == CardDB.cardName.wrath && choice != 2) return 0;
            if (name == CardDB.cardName.nourish && choice != 2) return 0;
            if (name == CardDB.cardName.grovetender && choice != 2) return 0;

            foreach (Action a in p.playactions)
            {
                if (a.actionType == ActionType.ATTACK_WITH_MINION)
                {
                    if (a.target.isHero && (a.own.Angr >= a.target.Hp + a.target.armor)) return 500;
                }

                if (a.actionType == ActionType.ATTACK_WITH_HERO)
                {
                    if (a.target.isHero && (a.own.Angr >= a.target.Hp + a.target.armor)) return 500;
                }

                //if (a.actionType == actionEnum.playcard)
                //{
                //    if (a.target != null && a.target.isHero && !a.target.own &&
                //        ((DamageTargetDatabase.ContainsKey(a.own.name) && DamageTargetDatabase[a.own.name] >= a.target.Hp + a.target.armor) ||
                //        (DamageAllDatabase.ContainsKey(a.own.name) && DamageAllDatabase[a.own.name] >= a.target.Hp + a.target.armor) ||
                //        (DamageAllEnemysDatabase.ContainsKey(a.own.name) && DamageAllEnemysDatabase[a.own.name] >= a.target.Hp + a.target.armor)))
                //    {
                //        return 500;
                //    }
                //}
            }

            int carddraw = cardDrawBattleCryDatabase[name];
            if (name == CardDB.cardName.harrisonjones)
            {
                carddraw = p.enemyWeaponDurability;
                if (carddraw == 0 && (p.enemyHeroName != HeroEnum.mage && p.enemyHeroName != HeroEnum.warlock && p.enemyHeroName != HeroEnum.priest))
                {
                    if (p.enemyDeckSize <= 15) return 5;
                    return 10;
                }
            }
            if (name == CardDB.cardName.divinefavor)
            {
                carddraw = p.enemyAnzCards - (Math.Max(p.owncards.Count - 1, 0));
                int hascanusecards = 0;
                int hasdivinefavor = 0;
                foreach (Handmanager.Handcard hcc in p.owncards)
                {
                    if (hcc.getManaCost(p) <= p.mana && hcc.card.type != CardDB.cardtype.HEROPWR) hascanusecards++;
                    if (hcc.card.name == CardDB.cardName.divinefavor) hasdivinefavor++;
                }
                if (p.ownMaxMana <= 6) return 10;
                if (carddraw <= 0) return 500;
                if (p.owncards.Count == hasdivinefavor && p.ownAbilityReady) return -5;
            }

            if (name == CardDB.cardName.battlerage)
            {
                carddraw = 0;
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.wounded) carddraw++;
                }
                if (carddraw == 0) return 500;
            }

            if (name == CardDB.cardName.slam)
            {
                Minion m = target;
                carddraw = 0;
                if (m != null && m.Hp >= 3) carddraw = 1;
                if (carddraw == 0) return 4;
            }

            if (name == CardDB.cardName.mortalcoil)
            {
                float pen = 0;
                Minion m = target;
                carddraw = 0;
                if (m != null && m.Hp <= 1 + p.spellpower) carddraw = 1;
                //if (m.own) pen = 15;
                if (p.playactions.Find(a => a.actionType == ActionType.PLAY_CARD && (a.card.card.name == CardDB.cardName.soultap || a.card.card.name == CardDB.cardName.lifetap)) != null) pen += p.playactions.Count * 12;
                if (carddraw == 0) return 5 + pen; // todo sepefeets - factor in spell dmg
            }

            if (name == CardDB.cardName.tinkertowntechnician)
            {
                carddraw = (p.ownMinions.Find(m => m.handcard.card.race == TAG_RACE.MECHANICAL) != null ? 1 : 0);
                if (carddraw == 0) return 2;
            }

            if (name == CardDB.cardName.lifetap || name == CardDB.cardName.soultap)
            {
                if (lethal) return 500; //RR no benefit for lethal check
                int minmana = 10;
                bool cardOnLimit = false;
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.manacost <= minmana)
                    {
                        minmana = hc.manacost;
                    }
                    //if (hc.getManaCost(p) == p.ownMaxMana)
                    int manac = hc.getManaCost(p);
                    if (manac > p.ownMaxMana - 2 && manac <= p.ownMaxMana)
                    {
                        cardOnLimit = true;
                    }
                }

                if ((Ai.Instance.botBase is BehaviorRush || Ai.Instance.botBase is BehaviorFace) && p.ownMaxMana <= 3 && cardOnLimit) return 6; //RR penalization for drawing the 3 first turns if we have a card in hand that we won't be able to play in Rush
                if (Ai.Instance.botBase is BehaviorAggroWarlock)
                {
                    foreach (Minion mnn in p.enemyMinions)
                    {
                        if (mnn.name == CardDB.cardName.doomsayer && !mnn.silenced) return 0;
                    }

                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        if (hc.card.type == CardDB.cardtype.MOB && hc.canplayCard(p) && p.ownMaxMana <= 5) return 14 - 2 * p.ownMaxMana; break;
                    }

                    //if (p.ownMaxMana >= 6) return 5 - p.ownMaxMana;
                    return 4;
                }

                if (p.owncards.Count + p.cardsPlayedThisTurn <= 5 && minmana > p.ownMaxMana) return 0;
                if (p.owncards.Count + p.cardsPlayedThisTurn > 5) return 25;
                return Math.Max(-carddraw + 2 * p.optionsPlayedThisTurn + p.ownMaxMana - p.mana, 0);
            }

            if (p.owncards.Count + carddraw > 10) return 15 * (p.owncards.Count + carddraw - 10);
            //if (p.owncards.Count + p.cardsPlayedThisTurn > 5)
            //{
            //    return carddraw + 1;
            //}

            //Helpfunctions.Instance.ErrorLog("drawpen = " + (-carddraw * 5 + 0.3f * p.optionsPlayedThisTurn + p.ownMaxMana - p.mana));
            return -carddraw + 0.3f * p.optionsPlayedThisTurn;
            /*pen = -carddraw + p.ownMaxMana - p.mana;
            return pen;*/
        }

        private float getCardDrawofEffectMinions(CardDB.Card card, Playfield p)
        {
            float pen = 0;
            int carddraw = 0;
            if (card.type == CardDB.cardtype.SPELL)
            {
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.name == CardDB.cardName.gadgetzanauctioneer) carddraw++;
                }
            }

            if (card.type == CardDB.cardtype.MOB && (TAG_RACE)card.race == TAG_RACE.BEAST)
            {
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.name == CardDB.cardName.starvingbuzzard) carddraw++;
                }
            }

            if (carddraw == 0) return 0;

            if (p.owncards.Count >= 5) return 0;
            //pen = -carddraw + p.ownMaxMana - p.mana + 0.3f * p.optionsPlayedThisTurn;
            pen -= carddraw;
            return pen;
        }

        public float getCardDrawDeathrattlePenality(CardDB.cardName name, Playfield p)
        {
            // penality if carddraw is late or you have enough cards
            if (!cardDrawDeathrattleDatabase.ContainsKey(name)) return 0;

            int carddraw = cardDrawDeathrattleDatabase[name];
            if (p.owncards.Count + carddraw > 10) return 15 * (p.owncards.Count + carddraw - 10);
            return 0.3f * p.optionsPlayedThisTurn;
        }

        private float getRandomPenalty(CardDB.Card card, Playfield p, Minion target)
        {
            float pen = 0;
            if (p.turnCounter >= 1)
            {
                return 0;
            }

            bool first = true;
            bool hasAuctioneer = false;
            bool hasBuzzard = false;
            bool hasJuggler = false;
            bool hasFlamewaker = false;
            bool hasMech = false;
            bool hasCouncilman = false;
            bool hasThunderbluffV = false;

            foreach (Minion mnn in p.ownMinions)
            {
                if (mnn.handcard.card.race == TAG_RACE.MECHANICAL) hasMech = true;

                if (mnn.silenced) continue;

                if (mnn.name == CardDB.cardName.gadgetzanauctioneer) hasAuctioneer = true;
                if (mnn.name == CardDB.cardName.starvingbuzzard) hasBuzzard = true;
                if (mnn.name == CardDB.cardName.knifejuggler) hasJuggler = true;
                if (mnn.name == CardDB.cardName.flamewaker) hasFlamewaker = true;
                if (mnn.name == CardDB.cardName.darkshirecouncilman) hasCouncilman = true; //not actually random but needs to be played early as if it were
                if (mnn.name == CardDB.cardName.thunderbluffvaliant) hasThunderbluffV = true; //not actually random but totemic call/slam are
            }

            //foreach (Action a in p.playactions) // penalty for "killing" combos (like had knifejuggler, traded him in last enemy-minion and then played a minion)
            //{
            //    if (a.actionType == actionEnum.attackWithMinion)
            //    {
            //        if (a.own.silenced)
            //            continue;
            //        if (a.own.name == CardDB.cardName.gadgetzanauctioneer)
            //        {
            //            if (!hasAuctioneer && card.type == CardDB.cardtype.SPELL && p.owncards.Count <= 5) pen += 20;
            //        }

            //        if (a.own.name == CardDB.cardName.starvingbuzzard)
            //        {
            //            if (!hasBuzzard && card.race == TAG_RACE.BEAST) pen += 20;
            //        }

            //        if (a.own.name == CardDB.cardName.knifejuggler)
            //        {
            //            if (!hasJuggler && (card.type == CardDB.cardtype.MOB || this.summonMinionSpellsDatabase.ContainsKey(card.name))) pen += 1;
            //        }

            //        if (a.own.name == CardDB.cardName.flamewaker)
            //        {
            //            if (!hasFlamewaker && card.type == CardDB.cardtype.SPELL) pen += 20;
            //        }

            //        if (a.own.name == CardDB.cardName.darkshirecouncilman)
            //        {
            //            if (card.type == CardDB.cardtype.MOB || this.summonMinionSpellsDatabase.ContainsKey(card.name)) pen += 20;
            //        }

            //        if (a.own.name == CardDB.cardName.tunneltrogg)
            //        {
            //            if (card.overload > 0 && card.name != CardDB.cardName.elementaldestruction) pen += 20 * card.overload;
            //        }
            //    }
            //    if (a.actionType == actionEnum.playcard)
            //    {
            //        if (a.card.card.overload > 0 && a.card.card.name != CardDB.cardName.elementaldestruction && card.name == CardDB.cardName.tunneltrogg)
            //        {
            //            pen += 25 * a.card.card.overload;
            //        }

            //        if (card.name == CardDB.cardName.knifejuggler && (a.card.card.type == CardDB.cardtype.MOB || this.summonMinionSpellsDatabase.ContainsKey(a.card.card.name))) //prioritize jugglers 1st
            //        {
            //            if (a.card.card.name != CardDB.cardName.knifejuggler) pen += 1;
            //        }

            //        if (card.name == CardDB.cardName.darkshirecouncilman && (a.card.card.type == CardDB.cardtype.MOB || this.summonMinionSpellsDatabase.ContainsKey(a.card.card.name))) //and councilman 2nd
            //        {
            //            if (a.card.card.name != CardDB.cardName.knifejuggler && a.card.card.name != CardDB.cardName.darkshirecouncilman) pen += 20;
            //        }
            //    }
            //}

            // Don't penalize for cases that don't actually have random outcomes
            // TODO: Add Lightning Storm + Elemental Destruction if all enemies hp < the minimum damage?

            if (!this.randomEffects.ContainsKey(card.name)
                && !this.cardDrawBattleCryDatabase.ContainsKey(card.name)
                && !(hasJuggler && (card.type == CardDB.cardtype.MOB || this.summonMinionSpellsDatabase.ContainsKey(card.name)) && p.enemyMinions.Count > 0)
                && !(hasCouncilman && (card.type == CardDB.cardtype.MOB || this.summonMinionSpellsDatabase.ContainsKey(card.name)))
                && !(hasAuctioneer && card.type == CardDB.cardtype.SPELL)
                && !(hasFlamewaker && card.type == CardDB.cardtype.SPELL && p.enemyMinions.Count > 0)
                && !(hasThunderbluffV && (TAG_RACE)card.race == TAG_RACE.TOTEM)
                && !(hasBuzzard && (TAG_RACE)card.race == TAG_RACE.BEAST))
            {
                return pen;
            }

            if (card.name == CardDB.cardName.brawl || ((card.name == CardDB.cardName.bouncingblade && p.enemyMinions.Count + p.ownMinions.Count == 1)
                || ((card.name == CardDB.cardName.goblinblastmage || card.name == CardDB.cardName.tinkertowntechnician) && !hasMech)
                || (card.name == CardDB.cardName.coghammer && p.ownMinions.Count == 1)))
            {
                return pen;
            }

            // Don't penalize for cases that don't actually have random outcomes
            // TODO: Add Lightning Storm + Elemental Destruction if all enemies hp < the minimum damage?

            if (p.enemyMinions.Count == 2 && (card.name == CardDB.cardName.cleave
                || card.name == CardDB.cardName.multishot
                || card.name == CardDB.cardName.forkedlightning
                || card.name == CardDB.cardName.darkbargain))
            {
                return pen;
            }

            if (card.name == CardDB.cardName.deadlyshot)
            {
                foreach (Minion mm in p.enemyMinions)
                {
                    if (!(mm.Angr >= 4 && mm.Hp >= 4 || mm.Hp >= 5 || mm.Angr >= 5 || (mm.divineshild && mm.Angr >= 4) || (this.priorityTargets.ContainsKey(mm.name) && this.priorityTargets[mm.name] >= 6))) pen = 20;
                    else pen = 3;
                }
            }

            if (p.enemyMinions.Count == 1 && (card.name == CardDB.cardName.deadlyshot
                || card.name == CardDB.cardName.flamecannon
                || card.name == CardDB.cardName.bomblobber))
            {
                return pen;
            }

            if (p.enemyMinions.Count == 0 && (card.name == CardDB.cardName.arcanemissiles
                || card.name == CardDB.cardName.avengingwrath
                || card.name == CardDB.cardName.goblinblastmage
                || card.name == CardDB.cardName.flamejuggler))
            {
                pen = 5;
                return pen;
            }

            int cards = this.randomEffects.ContainsKey(card.name) ? this.randomEffects[card.name] : (this.cardDrawBattleCryDatabase.ContainsKey(card.name) ? this.cardDrawBattleCryDatabase[card.name] : 0);

            int mobsAfterKnife = 0;
            int mobsAfterCouncilman = 0;
            foreach (Action a in p.playactions) // penalize for any non-random actions taken before playing this random one
            {
                if (a.actionType == ActionType.ATTACK_WITH_HERO)
                {
                    first = false;
                    continue;
                }

                if (a.actionType == ActionType.USE_HERO_POWER && (p.ownHeroName != HeroEnum.pala && p.ownHeroName != HeroEnum.shaman && p.ownHeroName != HeroEnum.warlock))
                {
                    first = false;
                    continue;
                }

                if (a.actionType == ActionType.ATTACK_WITH_MINION)
                {
                    first = false;
                    continue;
                }

                if (a.actionType == ActionType.PLAY_CARD)
                {
                    if (this.cardDrawBattleCryDatabase.ContainsKey(a.card.card.name)) continue;
                    if (this.lethalHelpers.ContainsKey(a.card.card.name)) continue;
                    if (this.randomEffects.ContainsKey(a.card.card.name)) continue;
                    if (a.card.card.name == CardDB.cardName.thecoin) continue; //no penalty for using coin first
                    if (hasBuzzard && a.card.card.race == TAG_RACE.BEAST) continue;
                    if (hasThunderbluffV && a.card.card.race == TAG_RACE.TOTEM) continue;

                    // no penalty for spells or other cards that obtain bonuses from playing spells
                    if ((hasAuctioneer || hasFlamewaker) && (a.card.card.type == CardDB.cardtype.SPELL
                        || a.card.card.name == CardDB.cardName.gadgetzanauctioneer || a.card.card.name == CardDB.cardName.flamewaker
                        || a.card.card.name == CardDB.cardName.manawyrm || a.card.card.name == CardDB.cardName.manaaddict
                        || a.card.card.name == CardDB.cardName.questingadventurer || a.card.card.name == CardDB.cardName.wildpyromancer
                        || a.card.card.name == CardDB.cardName.violetteacher || a.card.card.name == CardDB.cardName.archmageantonidas))
                    {
                        continue;
                    }

                    //todo sepefeets - does this even do anything?
                    if (hasJuggler && (card.type == CardDB.cardtype.MOB || this.summonMinionSpellsDatabase.ContainsKey(card.name))) //and others
                    {
                        if (card.name == CardDB.cardName.knifejuggler && mobsAfterKnife >= 1)
                        {
                            first = false;   // penalize playing 2nd knife juggler after other mobs
                        }
                        else
                        {
                            if (a.card.card.type == CardDB.cardtype.MOB && a.card.card.name != CardDB.cardName.knifejuggler) mobsAfterKnife++;
                            continue;
                        }
                    }

                    if (hasCouncilman && (card.type == CardDB.cardtype.MOB || this.summonMinionSpellsDatabase.ContainsKey(card.name))) //and others
                    {
                        if (card.name == CardDB.cardName.darkshirecouncilman && mobsAfterCouncilman >= 1)
                        {
                            first = false;   // penalize playing 2nd darkshire councilman after other mobs
                        }
                        else
                        {
                            if (a.card.card.type == CardDB.cardtype.MOB && a.card.card.name != CardDB.cardName.darkshirecouncilman) mobsAfterCouncilman++;
                            continue;
                        }
                    }
                }
                cards += 2;
            }

            if (first == false)
            {
                pen += (cards + p.playactions.Count) * 0.005f;
            }

            return pen;
        }

        private float getCardDiscardPenality(CardDB.cardName name, Playfield p, bool lethal)
        {
            if (p.owncards.Count <= 1) return 0;
            float pen = 0;
            //if (this.cardDiscardDatabase.ContainsKey(name))
            //{
            //    int newmana = p.mana - cardDiscardDatabase[name];
            //    bool canplayanothercard = false;
            //    foreach (Handmanager.Handcard hc in p.owncards)
            //    {
            //        if (this.cardDiscardDatabase.ContainsKey(hc.card.name)) continue;
            //        if (hc.card.getManaCost(p, hc.manacost) <= newmana)
            //        {
            //            canplayanothercard = true;
            //        }
            //    }
            //    if (canplayanothercard) pen += 10;

            //}
            //Helpfunctions.Instance.ErrorLog("discardpen " + pen);

            int hasMalimp = 0;
            foreach (Minion m in p.ownMinions)
            {
                if (m.name == CardDB.cardName.malchezaarsimp) hasMalimp++;
            }

            bool hasdiscardsynergy = false;
            foreach (Handmanager.Handcard hdc in p.owncards)
            {
                if (hdc.card.name == CardDB.cardName.fistofjaraxxus || hdc.card.name == CardDB.cardName.tinyknightofevil ||
                    hdc.card.name == CardDB.cardName.silverwaregolem || hdc.card.name == CardDB.cardName.clutchmotherzavas) hasdiscardsynergy = true;
            }

            if (this.discardedCardCount.ContainsKey(name))
            {
                int discardedCount = discardedCardCount[name];
                if (p.owncards.Count == 2 && discardedCount >= 2) discardedCount = 1;
                if (discardedCount >= 1) pen = 10;

                if (discardedCount >= 1 && (hasMalimp >= 1 || hasdiscardsynergy)) pen = -1 - (hasMalimp);
                if (discardedCount >= 1 && hasMalimp >= 1) pen -= p.playactions.Count * 0.5f;
                else if (discardedCount >= 1) pen -= p.playactions.Count * 0.5f;

                if (lethal && discardedCount >= 1) pen -= p.playactions.Count * 100;
            }

            return pen;
        }

        private int getDestroyOwnPenality(CardDB.cardName name, Minion target, Playfield p, bool lethal)
        {
            if (!this.destroyOwnDatabase.ContainsKey(name)) return 0;
            int pen = 0;

            if (name == CardDB.cardName.brawl || name == CardDB.cardName.deathwing || name == CardDB.cardName.twistingnether || name == CardDB.cardName.doomsayer || name == CardDB.cardName.doom)
            {
                int penalty = 0;
                if (p.mobsPlayedThisTurn >= 1 && name != CardDB.cardName.brawl) return 20;
                if (name == CardDB.cardName.brawl && p.ownMinions.Count + p.enemyMinions.Count <= 1) return 500;
                int highminion = 0;
                int veryhighminion = 0;
                int readyAngr = 0;

                int minionvalue = 0;

                foreach (Minion m in p.enemyMinions)
                {
                    if (!m.frozen)
                    {
                        readyAngr += m.Angr;
                        if (enrageDatabase.ContainsKey(m.name) && m.wounded)
                        {
                            readyAngr -= enrageDatabase[m.name];
                        }
                    }

                    if (enrageDatabase.ContainsKey(m.name) && !m.wounded)
                    {
                        if (m.Angr + enrageDatabase[m.name] >= 5) highminion++;
                        if (m.Angr + enrageDatabase[m.name] >= 8) veryhighminion++;
                    }
                    else
                    {
                        if (m.Angr >= 5 || m.Hp >= 5 && m.Angr >= 3) highminion++;
                        if (m.Angr >= 8 || m.Hp >= 8 && m.Angr >= 5) veryhighminion++;
                    }

                    minionvalue += m.Angr * 2 + m.Hp;
                    if (priorityTargets.ContainsKey(m.name)) minionvalue += priorityTargets[m.name];
                }

                if (name == CardDB.cardName.doomsayer)
                {
                    if (readyAngr > 5) return 500;
                    else if (p.enemyMinions.Count <= 2 || p.enemyMinions.Count + 2 <= p.ownMinions.Count || p.ownMinions.Count >= 3) return 30;
                    else return 0;
                }

                penalty = 35;

                if (highminion >= 3 || veryhighminion >= 2 || readyAngr >= 9)
                {
                    penalty = 15;
                }

                if (name == CardDB.cardName.brawl) penalty -= p.ownMinions.Count;
                if (name == CardDB.cardName.brawl && p.ownMinions.Find(a => a.name == CardDB.cardName.dirtyrat && a.playedThisTurn) != null && p.enemyDeckSize >= 1) penalty -= 5;

                return penalty;
            }
            if (target == null) return 0;
            if (target.own && !target.isHero)
            {
                // dont destroy owns ;_; (except mins with deathrattle effects)

                Minion m = target;
                if (m.handcard.card.deathrattle) return 30;
                if (lethal && name == CardDB.cardName.sacrificialpact)
                {
                    int demons = 0;
                    foreach (Minion mm in p.ownMinions)
                    {
                        if (mm.Ready && mm.handcard.card.race == TAG_RACE.DEMON) demons++;
                    }
                    if (demons == 0) return 500;
                }
                else
                {
                    return 500;
                }
            }

            return pen;
        }

        private int getDestroyPenality(CardDB.cardName name, Minion target, Playfield p, bool lethal)
        {
            if (!this.destroyDatabase.ContainsKey(name) || lethal) return 0;
            int pen = 0;
            if (target == null) return 0;
            if (name == CardDB.cardName.shatter && !target.frozen) return 500; //todo sepefeets - remove after CardDB.getTargetsForCard() updated/merged?

            //if (target.own && !target.isHero){} //handled in getDestroyOwnPenality()

            if (!target.own && !target.isHero)
            {
                // destroy others
                Minion m = target;

                /*if (m.allreadyAttacked)//doesnt make sence :D
                {
                    return 50;
                }*/

                if (name == CardDB.cardName.shadowwordpain || name == CardDB.cardName.bookwyrm)
                {
                    if (this.specialMinions.ContainsKey(m.name) || m.Angr == 3 || m.Hp >= 4)
                    {
                        return 0;
                    }

                    if (m.Angr == 2) return 5;

                    return 10;
                }

                if (name == CardDB.cardName.theblackknight) return 0;

                if (m.Angr >= 4 || m.Hp >= 4 || priorityDatabase.ContainsKey(m.name))
                {
                    pen = 10; // so we dont destroy cheap ones :D
                    if (p.ownHero.Hp <= 10) pen = 0;
                }
                else
                {
                    pen = 20 - m.Angr - m.Hp;
                    if (p.ownHero.Hp <= 10) pen = 0;
                }

                if (m.poisonous && p.ownMinions.Count >= 1) pen = 0;

                if (m.name == CardDB.cardName.doomsayer)
                {
                    pen = 0;
                }

                if (name == CardDB.cardName.mindcontrol && (m.name == CardDB.cardName.direwolfalpha || m.name == CardDB.cardName.raidleader || m.name == CardDB.cardName.flametonguetotem) && p.enemyMinions.Count == 1)
                {
                    pen = 50;
                }
            }
            //Helpfunctions.Instance.ErrorLog("pen = " + pen);
            return pen;
        }

        private int getPlayInspirePenalty(Handmanager.Handcard playhc, Playfield p)
        {
            // Penalize for playing Inspire minions without Inspire effect

            CardDB.Card card = playhc.card;
            CardDB.cardName name = card.name;

            if (!this.strongInspireEffectMinions.ContainsKey(name)) return 0;

            // check already used hero power
            if (p.playactions.Find(a => a.actionType == ActionType.USE_HERO_POWER) != null)
                return 1 + (5 * strongInspireEffectMinions[name]);

            int ownplaycost = playhc.getManaCost(p);
            int heropowercost = p.ownHeroAblility.card.getManaCost(p, 2);

            // check not enough mana to gain Inspire buff
            if (p.mana < ownplaycost + heropowercost) return 2 * strongInspireEffectMinions[name];

            return 0;
        }

        private int getbackToHandPenality(CardDB.cardName name, Minion target, Playfield p, bool lethal)
        {
            if (!this.backToHandDatabase.ContainsKey(name) || lethal) return 0;
            int pen = 0;

            if (name == CardDB.cardName.vanish)
            {
                //dont vanish if we have minons on board wich are ready
                bool haveready = false;
                foreach (Minion mins in p.ownMinions)
                {
                    if (mins.Ready) haveready = true;
                }
                if (haveready) pen += 10;
            }

            if (target == null) return 0;

            if (target.own && !target.isHero)
            {
                if (p.turnCounter >= 1 && !target.handcard.card.Charge) return 500;
                // dont destroy owns ;_; (except mins with deathrattle effects, with battlecry, or to heal)
                Minion m = target;
                pen = 500;

                if (m.handcard.card.deathrattle || m.handcard.card.battlecry || m.handcard.card.Charge || ((m.maxHp - m.Hp) >= 4))
                {
                    pen = 0;
                }
                if (m.handcard.card.deathrattle || m.handcard.card.battlecry || m.handcard.card.Charge || ((m.maxHp - m.Hp) >= 4))
                {
                    pen = 0;
                }
                if (m.shadowmadnessed)
                {
                    pen = -20;
                }

                if (m.Ready) pen += 10;
            }
            if (!target.own && !target.isHero)
            {
                Minion m = target;

                if (m.allreadyAttacked || m.shadowmadnessed) //dont sap shadow madness
                {
                    return 50;
                }

                if (m.name == CardDB.cardName.theblackknight)
                {
                    return 50;
                }

                if (m.Angr >= 5 || m.Hp >= 5)
                {
                    pen = 0; // so we dont destroy cheap ones :D
                }
                else
                {
                    pen = 30;
                }

                if (this.cardDrawBattleCryDatabase.ContainsKey(m.name)) pen += 10 * this.cardDrawBattleCryDatabase[m.name];
            }

            return pen;
        }

        private float getHeroPowerPenality(Handmanager.Handcard hcard, Minion target, Playfield p)
        {
            CardDB.Card card = hcard.card;
            CardDB.cardName name = card.name;

            // penalize playing hero power after spell dmg cards
            if (name == CardDB.cardName.totemiccall || name == CardDB.cardName.totemicslam || name == CardDB.cardName.tuskarrtotemic)  // shaman
            {
                return p.playactions.FindAll(a => a.actionType == ActionType.PLAY_CARD && a.card.card.type == CardDB.cardtype.SPELL
                    && (DamageTargetSpecialDatabase.ContainsKey(a.card.card.name) || DamageTargetDatabase.ContainsKey(a.card.card.name)
                        || DamageAllEnemysDatabase.ContainsKey(a.card.card.name) || DamageHeroDatabase.ContainsKey(a.card.card.name)
                        || DamageRandomDatabase.ContainsKey(a.card.card.name) || a.card.card.name == CardDB.cardName.elementaldestruction)).Count * 9;
            }

            if (name == CardDB.cardName.lifetap || name == CardDB.cardName.soultap)
            {
                float pen = 0;
                float prevpen;

                foreach (Action a in p.playactions)
                {
                    if (a.actionType == ActionType.ATTACK_WITH_MINION)
                    {
                        if (a.target.isHero && (a.own.Angr >= a.target.Hp + a.target.armor)) return 500;
                    }

                    if (a.actionType == ActionType.ATTACK_WITH_HERO)
                    {
                        if (a.target.isHero && (a.own.Angr >= a.target.Hp + a.target.armor)) return 500;
                    }

                    //if (a.actionType == actionEnum.playcard)
                    //{
                    //    if (a.target != null && a.target.isHero && !a.target.own &&
                    //        ((DamageTargetDatabase.ContainsKey(a.own.name) && DamageTargetDatabase[a.own.name] >= a.target.Hp + a.target.armor) ||
                    //        (DamageAllDatabase.ContainsKey(a.own.name) && DamageAllDatabase[a.own.name] >= a.target.Hp + a.target.armor) ||
                    //        (DamageAllEnemysDatabase.ContainsKey(a.own.name) && DamageAllEnemysDatabase[a.own.name] >= a.target.Hp + a.target.armor)))
                    //    {
                    //        return 500;
                    //    }
                    //}
                }

                //if (lethal) return 500;
                if (p.ownDeckSize == 0) return 200;

                if (p.ownMaxMana <= 5) pen += 12 - 2 * p.ownMaxMana;

                bool doomsayeron = false;
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.name == CardDB.cardName.doomsayer) doomsayeron = true;
                }
                foreach (Minion mnn in p.enemyMinions)
                {
                    if (mnn.name == CardDB.cardName.doomsayer) doomsayeron = true;
                }
                if (doomsayeron) return 0;

                //int manacost = p.mana;
                foreach (Handmanager.Handcard hcc in p.owncards)
                {
                    if (this.cardDiscardDatabase.ContainsKey(hcc.card.name) && p.mana >= hcc.getManaCost(p)) return this.cardDiscardDatabase[hcc.card.name];
                }

                if (p.enemyHeroName == HeroEnum.mage)
                {
                    // frozen mage
                    int iceblockmage = 0;
                    foreach (KeyValuePair<CardDB.cardIDEnum, int> e in Probabilitymaker.Instance.enemyGraveyard)
                    {
                        if (e.Key == CardDB.cardIDEnum.NEW1_021) iceblockmage++; //doomsayer NEW1_021
                        if (e.Key == CardDB.cardIDEnum.CS2_026) iceblockmage++; //frostnova CS2_026
                        if (e.Key == CardDB.cardIDEnum.EX1_007) iceblockmage++; //Acolyte of Pain EX1_007
                        if (e.Key == CardDB.cardIDEnum.EX1_096) iceblockmage++; //loothoader EX1_096
                    }
                    //Helpfunctions.Instance.logg("iceblockmage " + " " + iceblockmage);
                    if (iceblockmage >= 1)
                    {
                        int frozencount = 0;
                        foreach (Minion mnn in p.ownMinions)
                        {
                            if (mnn.frozen) frozencount++;
                        }
                        if (frozencount >= 5 || p.ownMinions.Count == 6 || p.enemyHero.immune)
                        {
                            if (card.name == CardDB.cardName.lifetap) pen += 40;
                        }
                    }
                }

                if (p.playactions.Find(a => a.actionType == ActionType.PLAY_CARD && a.card.card.name == CardDB.cardName.mortalcoil) != null) return pen;
                if (p.playactions.Count == 0) pen -= 5;

                return pen + p.playactions.Count * 0.5f;
            }
            //// penalize playing shapeshift after other moves
            //if (name == CardDB.cardName.shapeshift || name == CardDB.cardName.direshapeshift)
            //{
            //    return p.playactions.Count * 0.5f;
            //}

            if (name == CardDB.cardName.armorup || name == CardDB.cardName.tankup)
            {
                if (name == CardDB.cardName.armorup) return 0.5f;
                if (name == CardDB.cardName.tankup) return 1;
            }

            bool juggleron = false;
            foreach (Minion mnn in p.ownMinions)
            {
                if (mnn.name == CardDB.cardName.knifejuggler && !mnn.silenced) juggleron = true;
            }
            bool acolyte = false;
            foreach (Minion mnn in p.enemyMinions)
            {
                if (mnn.name == CardDB.cardName.acolyteofpain && !mnn.silenced) acolyte = true;
            }

            if (juggleron && !acolyte
                && name == CardDB.cardName.reinforce || name == CardDB.cardName.thesilverhand
                && name == CardDB.cardName.totemiccall || name == CardDB.cardName.totemicslam) return p.playactions.Count * 0.1f;  // to attack with minion later

            if (name == CardDB.cardName.steadyshot || name == CardDB.cardName.ballistashot)
            {
                if (p.manaTurnEnd >= p.ownHeroAblility.card.getManaCost(p, 2)) return 5;
                return 8;
            }

            if (name == CardDB.cardName.lesserheal || name == CardDB.cardName.heal)
            {
                if (p.ownMinions.Find(m => m.name == CardDB.cardName.northshirecleric && !m.silenced) != null)
                {
                    return p.playactions.Count * 0.5f;
                }
            }

            if (name == CardDB.cardName.dieinsect)
            {
                return p.playactions.Count * 0.05f;
            }

            if (name == CardDB.cardName.thesilverhand || name == CardDB.cardName.reinforce || name == CardDB.cardName.thetidalhand)
            {
                return 0;
            }

            return 2 - p.playactions.Count * 0.0003f;
            //return 0;
        }

        private int getPlayMobPenalty(Handmanager.Handcard card, Minion target, Playfield p, bool lethal)
        {
            if (card.card.type != CardDB.cardtype.MOB && !this.summonMinionSpellsDatabase.ContainsKey(card.card.name)) return 0;
            int retval = 0;

            if (card.card.name == CardDB.cardName.doomcaller && p.diedMinions != null && p.diedMinions.Find(ct => ct.own && ct.cardid == CardDB.cardIDEnum.OG_279).cardid == CardDB.cardIDEnum.OG_279) retval += 5; //OG_279 = cthun; penalize playing if cthun is not dead
            if (p.ownMinions.Find(m => m.name == CardDB.cardName.doomsayer && !m.silenced) != null || p.enemyMinions.Find(m => m.name == CardDB.cardName.doomsayer && !m.silenced) != null)
            {
                // penalize playing minions into doomsayer
                // todo sepefeets - is this good enough? figure out way to penalize boards that play cards but don't kill it
                // also check up on buff penalty, some buffs missing, and others penalized twice elsewhere?

                int enemydoomsayerMaxHp = 0;
                foreach (Minion min in p.enemyMinions)
                {
                    if (min.name == CardDB.cardName.doomsayer && enemydoomsayerMaxHp <= min.Hp) enemydoomsayerMaxHp = min.Hp;
                }
                //Helpfunctions.Instance.ErrorLog("enemydoomsayerMaxHp " + enemydoomsayerMaxHp);

                if (this.silenceDatabase.ContainsKey(card.card.name)) return 0;
                if (card.card.name == CardDB.cardName.crazedalchemist) return 0;
                if (card.card.name == CardDB.cardName.stampedingkodo) return 0;
                if (card.card.Charge) doomsayerreturn = 50;//doomsayerreturn += card.card.getManaCost(p,10); //just estimate the value by the cost
                else if (card.card.name == CardDB.cardName.callofthewild) doomsayerreturn = 100;
                else if (card.card.name == CardDB.cardName.lifetap || card.card.name == CardDB.cardName.soultap) return 0;
                else if (this.buffing1TurnDatabase.ContainsKey(card.card.name) || this.attackBuffDatabase.ContainsKey(card.card.name) || this.buffingMinionsDatabase.ContainsKey(card.card.name)) doomsayerreturn = 100;
                else if (card.card.type == CardDB.cardtype.HEROPWR) return 0;
                else if (this.summonMinionSpellsDatabase.ContainsKey(card.card.name))
                {
                    if (DamageAllDatabase.ContainsKey(card.card.name) && DamageAllDatabase[card.card.name] >= enemydoomsayerMaxHp) return 5;
                    else if (DamageAllEnemysDatabase.ContainsKey(card.card.name) && DamageAllEnemysDatabase[card.card.name] >= enemydoomsayerMaxHp) return 5;
                    else if (DamageTargetDatabase.ContainsKey(card.card.name) && DamageTargetDatabase[card.card.name] >= enemydoomsayerMaxHp) return 5;
                    else if (DamageTargetSpecialDatabase.ContainsKey(card.card.name) && DamageTargetSpecialDatabase[card.card.name] >= enemydoomsayerMaxHp) return 5;
                    else if (heroAttackBuffDatabase.ContainsKey(card.card.name) && heroAttackBuffDatabase[card.card.name] + p.ownWeaponAttack >= enemydoomsayerMaxHp && !p.ownHero.frozen) return 5;
                }
                else if (specialMinions.ContainsKey(card.card.name))
                {
                    doomsayerreturn = 100;
                }
                else return 500;
                if (p.diedMinions != null && p.diedMinions.Find(ct => !ct.own && ct.cardid == CardDB.cardIDEnum.NEW1_021).cardid == CardDB.cardIDEnum.NEW1_021) cards = 0;//doomsayerreturn = 0;

                //cards = doomsayerreturn;

                //doomsayerreturn = cards * 50;

                //Helpfunctions.Instance.ErrorLog("doomsayerturn" + doomsayerreturn);
                cards = 0;

                return doomsayerreturn;
            }
            //else

            if (p.ownMinions.Find(m => m.name == CardDB.cardName.muklaschampion && !m.silenced) != null && p.playactions.Find(a => a.actionType == ActionType.USE_HERO_POWER) != null)
            {
                // penalize playing minions after mukla's +1/+1 buff
                retval += 5;
            }

            int buffs = 0;
            foreach (Action a in p.playactions)
            {
                if (a.card == null || a.actionType != ActionType.PLAY_CARD) continue;

                if (a.card.card.name == CardDB.cardName.everyfinisawesome) buffs++;
                if (a.card.card.name == CardDB.cardName.savageroar) buffs++;
                if (a.card.card.name == CardDB.cardName.bloodlust) buffs++;
                if (a.card.card.name == CardDB.cardName.souloftheforest) buffs++;
                if (a.card.card.name == CardDB.cardName.powerofthewild && a.druidchoice == 1) buffs++;
                if (a.card.card.name == CardDB.cardName.cenarius && a.druidchoice == 1) buffs++;
                if (a.card.card.name == CardDB.cardName.metaltoothleaper) buffs++;
                if (a.card.card.name == CardDB.cardName.enhanceomechano) buffs++;
                if (card.card.tank && a.card.card.name == CardDB.cardName.bolster) buffs++;
            }

            if (buffs >= 1)
            {
                retval += 5 * buffs;
            }

            if (p.enemyHeroName == HeroEnum.mage)
            {
                // frozen mage
                int iceblockmage = 0;
                foreach (KeyValuePair<CardDB.cardIDEnum, int> e in Probabilitymaker.Instance.enemyGraveyard)
                {
                    if (e.Key == CardDB.cardIDEnum.NEW1_021) iceblockmage++; //doomsayer NEW1_021
                    if (e.Key == CardDB.cardIDEnum.CS2_026) iceblockmage++; //frostnova CS2_026
                    if (e.Key == CardDB.cardIDEnum.EX1_007) iceblockmage++; //Acolyte of Pain EX1_007
                    if (e.Key == CardDB.cardIDEnum.EX1_096) iceblockmage++; //loothoader EX1_096
                }
                //Helpfunctions.Instance.logg("iceblockmage " + " " + iceblockmage);
                if (iceblockmage >= 1)
                {
                    int frozencount = 0;
                    foreach (Minion mnn in p.ownMinions)
                    {
                        if (mnn.frozen) frozencount++;
                    }
                    if (frozencount >= 5 || p.ownMinions.Count == 6 || p.enemyHero.immune)
                    {
                        if (!card.card.Charge && !card.card.deathrattle && !card.card.Shield && card.card.Health <= 5 && card.card.type != CardDB.cardtype.HEROPWR) retval += 40;
                        if (card.card.name == CardDB.cardName.lifetap) retval += 40;
                    }
                }
            }

            if (card.card.Charge) retval += card.card.Attack;

            return retval;
        }

        private int getPlaySpellPenalty(Handmanager.Handcard card, Minion target, Playfield p, bool lethal)
        {
            if (card.card.type != CardDB.cardtype.SPELL) return 0;
            int retval = 0;

            //always use spell when lethal
            if (DamageAllDatabase.ContainsKey(card.card.name) && DamageAllDatabase[card.card.name] >= p.enemyHero.Hp + p.enemyHero.armor) return 0;
            else if (DamageAllEnemysDatabase.ContainsKey(card.card.name) && DamageAllEnemysDatabase[card.card.name] >= p.enemyHero.Hp + p.enemyHero.armor) return 0;
            else if (DamageTargetDatabase.ContainsKey(card.card.name) && DamageTargetDatabase[card.card.name] >= p.enemyHero.Hp + p.enemyHero.armor && target.isHero) return 0;
            else if (DamageTargetSpecialDatabase.ContainsKey(card.card.name) && DamageTargetSpecialDatabase[card.card.name] >= p.enemyHero.Hp + p.enemyHero.armor && target.isHero) return 0;
            else if (heroAttackBuffDatabase.ContainsKey(card.card.name) && heroAttackBuffDatabase[card.card.name] + p.ownWeaponAttack >= p.enemyHero.Hp + p.enemyHero.armor && !p.ownHero.frozen) return 0;

            if (p.ownMinions.Find(m => m.name == CardDB.cardName.doomsayer && !m.silenced) != null || p.enemyMinions.Find(m => m.name == CardDB.cardName.doomsayer && !m.silenced) != null)
            {
                if (card.card.type == CardDB.cardtype.HEROPWR) return 0;

                int enemydoomsayerMaxHp = 0;
                foreach (Minion min in p.enemyMinions)
                {
                    if (min.name == CardDB.cardName.doomsayer && enemydoomsayerMaxHp <= min.Hp) enemydoomsayerMaxHp = min.Hp;
                }

                if (p.ownMinions.Count == 0) return 10;

                if (target != null)
                {
                    if (silenceDatabase.ContainsKey(card.card.name) && target.name == CardDB.cardName.doomsayer && !target.silenced) return 0;
                    if (TransformCardDatabase.ContainsKey(card.card.name) && target.name == CardDB.cardName.doomsayer && !target.silenced) return 0;
                    if (backToHandDatabase.ContainsKey(card.card.name) && target.name == CardDB.cardName.doomsayer && !target.silenced) return 0;
                    if ((destroyDatabase.ContainsKey(card.card.name) ||
                    destroyOwnDatabase.ContainsKey(card.card.name)) && target.name == CardDB.cardName.doomsayer && !target.silenced) return 0;
                }
                else
                {
                    if (silenceDatabase.ContainsKey(card.card.name) ||
                        TransformCardDatabase.ContainsKey(card.card.name) ||
                        backToHandDatabase.ContainsKey(card.card.name) ||
                        destroyDatabase.ContainsKey(card.card.name) ||
                        destroyOwnDatabase.ContainsKey(card.card.name) ||
                        KillCardDatabase.ContainsKey(card.card.name)) return 5;
                }

                if (DamageAllDatabase.ContainsKey(card.card.name) && DamageAllDatabase[card.card.name] >= enemydoomsayerMaxHp) return 5;
                else if (DamageAllEnemysDatabase.ContainsKey(card.card.name) && DamageAllEnemysDatabase[card.card.name] >= enemydoomsayerMaxHp) return 5;
                else if (DamageTargetDatabase.ContainsKey(card.card.name) && DamageTargetDatabase[card.card.name] >= enemydoomsayerMaxHp) return 5;
                else if (DamageTargetSpecialDatabase.ContainsKey(card.card.name) && DamageTargetSpecialDatabase[card.card.name] >= enemydoomsayerMaxHp) return 5;
                else if (heroAttackBuffDatabase.ContainsKey(card.card.name) && heroAttackBuffDatabase[card.card.name] + p.ownWeaponAttack >= enemydoomsayerMaxHp && !p.ownHero.frozen) return 1;
                else if (card.card.name == CardDB.cardName.shieldslam && p.ownHero.armor >= enemydoomsayerMaxHp) return 5;
                else return 100;
            }

            //if (p.enemyHeroName == HeroEnum.mage)
            //{
            //    // frozen mage
            //    int iceblockmage = 0;
            //    foreach (KeyValuePair<CardDB.cardIDEnum, int> e in Probabilitymaker.Instance.enemyGraveyard)
            //    {
            //        if (e.Key == CardDB.cardIDEnum.NEW1_021) iceblockmage++; //doomsayer NEW1_021
            //        if (e.Key == CardDB.cardIDEnum.CS2_026) iceblockmage++; //frostnova CS2_026
            //        if (e.Key == CardDB.cardIDEnum.EX1_007) iceblockmage++; //Acolyte of Pain EX1_007
            //        if (e.Key == CardDB.cardIDEnum.EX1_096) iceblockmage++; //loothoader EX1_096
            //    }
            //    //Helpfunctions.Instance.logg("iceblockmage " + " " + iceblockmage);
            //    if (iceblockmage >= 1)
            //    {
            //        int frozencount = 0;
            //        foreach (Minion mnn in p.ownMinions)
            //        {
            //            if (mnn.frozen) frozencount++;
            //        }
            //        if (frozencount >= 5 || p.ownMinions.Count == 6 || p.enemyHero.immune)
            //        {
            //            if (!card.card.Charge && !card.card.deathrattle && !card.card.Shield && card.card.Health <= 5) retval += 40;
            //        }
            //    }
            //}

            return retval;
        }

        private float getSpecialCardComboPenalties(Handmanager.Handcard playedhcard, Minion target, Playfield p, bool lethal, int choice)
        {
            CardDB.Card card = playedhcard.card;
            CardDB.cardName name = card.name;

            if (lethal && card.type == CardDB.cardtype.MOB)
            {
                if (this.lethalHelpers.ContainsKey(name))
                {
                    return 0;
                }

                if (this.DamageTargetDatabase.ContainsKey(name) || this.DamageAllDatabase.ContainsKey(name))
                {
                    return 0;
                }

                if (this.buffingMinionsDatabase.ContainsKey(name))
                {
                    if (name == CardDB.cardName.timberwolf || name == CardDB.cardName.houndmaster)
                    {
                        int beasts = 0;
                        foreach (Minion mm in p.ownMinions)
                        {
                            if (mm.Ready && (TAG_RACE)mm.handcard.card.race == TAG_RACE.BEAST) beasts++;
                        }
                        if (beasts == 0) return 500;
                    }

                    if (name == CardDB.cardName.warsongcommander)
                    {
                        int chargers = 0;
                        foreach (Minion mm in p.ownMinions)
                        {
                            if (mm.charge >= 1) chargers++;
                        }
                        if (chargers == 0) return 500;
                    }

                    if (name == CardDB.cardName.southseacaptain)
                    {
                        int pirates = 0;
                        foreach (Minion mm in p.ownMinions)
                        {
                            if (mm.Ready && (TAG_RACE)mm.handcard.card.race == TAG_RACE.PIRATE) pirates++;
                        }
                        if (pirates == 0) return 500;
                    }
                    if (name == CardDB.cardName.murlocwarleader || name == CardDB.cardName.grimscaleoracle || name == CardDB.cardName.coldlightseer)
                    {
                        int murlocs = 0;
                        foreach (Minion mm in p.ownMinions)
                        {
                            if (mm.Ready && (TAG_RACE)mm.handcard.card.race == TAG_RACE.MURLOC) murlocs++;
                        }
                        if (murlocs == 0) return 500;
                    }

                    if (name == CardDB.cardName.warhorsetrainer)
                    {
                        int recruits = 0;
                        foreach (Minion mm in p.ownMinions)
                        {
                            if (mm.Ready && mm.name == CardDB.cardName.silverhandrecruit) recruits++;
                        }
                        if (recruits == 0) return 500;
                    }

                    if (name == CardDB.cardName.malganis)
                    {
                        int demons = 0;
                        foreach (Minion mm in p.ownMinions)
                        {
                            if (mm.Ready && mm.handcard.card.race == TAG_RACE.DEMON) demons++;
                        }
                        if (demons == 0) return 500;
                    }
                }
                else
                {
                    if (name == CardDB.cardName.theblackknight)
                    {
                        int taunts = 0;
                        foreach (Minion mm in p.enemyMinions)
                        {
                            if (mm.taunt) taunts++;
                        }
                        if (taunts == 0) return 500;
                    }
                    else
                    {
                        if ((this.HealTargetDatabase.ContainsKey(name) || this.HealHeroDatabase.ContainsKey(name) || this.HealAllDatabase.ContainsKey(name)))
                        {
                            int wardens = 0;
                            foreach (Minion mm in p.ownMinions)
                            {
                                if (mm.Ready && mm.handcard.card.name == CardDB.cardName.lightwarden) wardens++;
                                if (mm.Ready && mm.handcard.card.name == CardDB.cardName.holychampion) wardens++;
                            }
                            if (wardens == 0) return 500;
                        }
                        else
                        {
                            //ignore that minion if it does not have charge, or we can give him charge ---> warsong was deleted ;_;
                            if (!(name == CardDB.cardName.nightblade || card.Charge || this.silenceDatabase.ContainsKey(name) || ((TAG_RACE)card.race == TAG_RACE.BEAST && p.ownMinions.Find(x => x.name == CardDB.cardName.tundrarhino) != null) || p.owncards.Find(x => x.card.name == CardDB.cardName.charge) != null))
                            {
                                return 500;
                            }
                        }
                    }
                }
            }

            //lethal end########################################################
            /*
            //bonus for early threat
            if (p.ownMaxMana == 1)
            {
                if (card.name == CardDB.cardName.shrinkmeister) return 20; //don't play early
                if (card.Attack >= 3 && card.Health >= 4) return -20;
                if (card.Attack >= 3 && card.Health >= 3) return -10;
                if (card.Attack >= 3 && card.Health >= 2) return -5;
                if (card.Health > 0) p.evaluatePenality += -2; //-card.Attack - card.Health; //nudge any minion playable
            }*/

            //if (p.ownMaxMana == 2 || (p.mana == 2 && p.ownMaxMana == 1)) //todo sepefeets - does coin raise ownMaxMana?
            //{
            //    if (card.name == CardDB.cardName.nerubianegg) return -2;
            //    //if (card.name == CardDB.cardName.wildgrowth) return -150; //bonus for turn 1 coin+growth or turn 2 growth but not innervate+growth
            //}

            if (card.name == CardDB.cardName.flamewaker && p.turnCounter == 0)
            {
                int number = 0;
                foreach (Action a in p.playactions)
                {
                    if (a.card != null && a.card.card.type == CardDB.cardtype.SPELL) number++;
                }
                return number;
            }

            switch (card.name)
            {
                case CardDB.cardName.twilightdarkmender:
                case CardDB.cardName.twinemperorveklor:
                case CardDB.cardName.klaxxiamberweaver:
                case CardDB.cardName.ancientshieldbearer:
                    if ((p.anzOgOwnCThunAngrBonus + 6) <= 9) return 5;
                    break;
            }

            //if (card.name == CardDB.cardName.unstableportal && p.owncards.Count <= 9) return -15;

            if (card.name == CardDB.cardName.daggermastery)
            {
                if (p.ownWeaponAttack >= 2 || p.ownWeaponDurability >= 2) return 5;
            }

            if (card.name == CardDB.cardName.upgrade)
            {
                int ret = 0;

                bool canplayweapon = false;
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.type == CardDB.cardtype.WEAPON) canplayweapon = true;
                    if (hc.card.name == CardDB.cardName.nzothsfirstmate) canplayweapon = true;
                }

                //if (p.playactions.Find(a => a.actionType == actionEnum.attackWithHero) != null) ret = 5;
                if (p.ownWeaponDurability == 0) ret = 5;

                if (canplayweapon) ret += 5;

                return ret;
            }

            if (card.name == CardDB.cardName.baronrivendare)
            {
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.name == CardDB.cardName.deathlord || mnn.name == CardDB.cardName.zombiechow || mnn.name == CardDB.cardName.dancingswords) return 30;
                }
            }

            //rule for coin on early game
            if (p.ownMaxMana < 3 && card.name == CardDB.cardName.thecoin) //1,2턴
            {
                bool WGON = false;
                int has1manacard = 0;
                int has2manacard = 0;
                int has3manacard = 0;
                bool hasweapon = false;
                bool hasPalSecret = false;
                bool hasvaluable3costminion = false;
                bool hasabusive = false;
                bool hastotemgolem = false;
                bool has1costSpellAndEnemyMinion = false;
                bool hasMaelstrom = false;
                foreach (Handmanager.Handcard hcc in p.owncards)
                {
                    if (hcc.card.name == CardDB.cardName.abusivesergeant) hasabusive = true;
                    if (hcc.card.name == CardDB.cardName.wildgrowth || hcc.card.name == CardDB.cardName.darnassusaspirant) WGON = true;
                    if (hcc.manacost == 1 && hcc.card.type == CardDB.cardtype.MOB && hcc.card.name != CardDB.cardName.abusivesergeant) has1manacard++;
                    if (hcc.card.name == CardDB.cardName.totemgolem) hastotemgolem = true;

                    if (hcc.card.Secret && hcc.getManaCost(p) == 1) hasPalSecret = true;
                    if (hcc.manacost == 2 && hcc.card.type != CardDB.cardtype.HEROPWR && hcc.card.type == CardDB.cardtype.MOB && (hcc.card.name != CardDB.cardName.bloodmagethalnos && hcc.card.name != CardDB.cardName.flametonguetotem && hcc.card.name != CardDB.cardName.ironbeakowl && hcc.card.name != CardDB.cardName.knifejuggler && hcc.card.name != CardDB.cardName.direwolfalpha && hcc.card.name != CardDB.cardName.scavenginghyena && hcc.card.name != CardDB.cardName.crazedalchemist && hcc.card.name != CardDB.cardName.acidicswampooze && hcc.card.name != CardDB.cardName.darkshirelibrarian)) has2manacard++;
                    if (hcc.manacost == 3 && (hcc.card.type == CardDB.cardtype.MOB || hcc.card.name == CardDB.cardName.animalcompanion || hcc.card.name == CardDB.cardName.eaglehornbow) && hcc.card.name != CardDB.cardName.biggamehunter) has3manacard++;
                    if (hcc.card.name == CardDB.cardName.impgangboss || hcc.card.name == CardDB.cardName.darkshirecouncilman || hcc.card.name == CardDB.cardName.animalcompanion || hcc.card.name == CardDB.cardName.carriongrub || hcc.card.name == CardDB.cardName.feralspirit || hcc.card.name == CardDB.cardName.tarcreeper) hasvaluable3costminion = true;
                    //check 2 mana weapon situation
                    if (hcc.card.type == CardDB.cardtype.WEAPON && p.enemyMinions.Count >= 1) hasweapon = true;
                    //Helpfunctions.Instance.ErrorLog("" + hcc.manacost + hcc.card.name);
                    //spell 1cost - and enemyminion
                    if (p.enemyMinions.Count >= 1 && hcc.manacost == 1 && hcc.card.type == CardDB.cardtype.SPELL && p.mana == 0) has1costSpellAndEnemyMinion = true;
                    if (hcc.card.name == CardDB.cardName.maelstromportal) hasMaelstrom = true;
                }
                //Helpfunctions.Instance.ErrorLog(has1manacard +" " + has2manacard + " " +has3manacard);
                bool hassecretkepper = false;
                bool hasownready = false;
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.name == CardDB.cardName.secretkeeper) hassecretkepper = true;
                    if (mnn.Ready && mnn.numAttacksThisTurn == 0) hasownready = true;
                }
                if (hasvaluable3costminion && p.enemyMinions.Count == 0 && p.ownMaxMana == 1) return 15;

                if (p.ownHeroName == HeroEnum.druid && WGON) return 2;
                if (has1manacard >= 2 && p.ownMaxMana == 1) return 2;
                if (has2manacard >= 2 && p.ownMaxMana == 1) return 2;
                if (hastotemgolem && p.ownMaxMana == 1) return 2;
                if (has3manacard >= 2 && p.ownMaxMana == 2) return 2;

                if ((has3manacard == 2 || (has1manacard == 1 && has2manacard == 1)) && p.ownMaxMana == 2) return 2;

                if (hasPalSecret && hassecretkepper && (p.enemyMinions.Count >= 1 || p.enemyWeaponAttack >= 2)) return 2;

                if (hasMaelstrom && p.enemyMinions.Count >= 2) return 2;
                if (has1manacard >= 1 && p.ownMinions.Find(b => b.name == CardDB.cardName.manawyrm && !b.silenced) != null) return 2;
                if (has2manacard >= 1 && p.ownMinions.Find(b => b.name == CardDB.cardName.manawyrm && !b.silenced) != null && p.mana == 1) return 2;
                if (has1manacard >= 1 && p.mana == 0) return 2;
                if (has1costSpellAndEnemyMinion) return 2;

                if (hasweapon) return 3.5f;
                if (hasvaluable3costminion && p.ownMaxMana == 2) return 5;
                if (hasownready && hasabusive && p.enemyMinions.Count >= 1) return 5;

                // if (!WGON || (has2manamob <= 1 && p.ownMaxMana == 1) || (has2manamob+has3manamob <= 1 && p.ownMaxMana == 2)) return 500;   //몹없고, 무기있고 적몹있는경우 아니거나, 급속성장 없으면 페널티.
                return 20;
            }

            if (card.name == CardDB.cardName.thecoin)
            {
                if (p.ownMinions.Find(a => a.name == CardDB.cardName.gadgetzanauctioneer && !a.silenced) != null)
                {
                    return 0;
                }

                if (p.ownMaxMana >= 6) return 2;
                return 10 - p.playactions.Count * 0.3f;
            }

            if (card.name == CardDB.cardName.innervate)
            {
                if (p.ownMinions.Find(a => a.name == CardDB.cardName.gadgetzanauctioneer && !a.silenced) != null)
                {
                    return 0;
                }
                if (p.mana >= 9) return 100;
                return 6;
            }

            /*//rule for coin on early game
            if (p.ownMaxMana < 3 && card.name == CardDB.cardName.thecoin)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.type == CardDB.cardtype.MOB && hc.canplayCard(p)) return 5;
                }
            }*/

            // spare parts need a base penalty so the bot does not waste them
            // TODO: move this a better location, and break reversing switch into atk buff and hp buff components
            //if (name == CardDB.cardName.finickycloakfield || name == CardDB.cardName.emergencycoolant || name == CardDB.cardName.reversingswitch)
            //    return 20;

            if (name == CardDB.cardName.flare || name == CardDB.cardName.kezanmystic || name == CardDB.cardName.eaterofsecrets)
            {
                int ret = 0;
                if (p.enemyHeroName == HeroEnum.hunter || p.enemyHeroName == HeroEnum.mage || p.enemyHeroName == HeroEnum.pala)
                {
                    if (p.enemySecretCount == 0 && p.enemyDeckSize >= 14) ret = 50;
                    if (p.enemySecretCount >= 1) ret = 0;
                    if (p.enemySecretCount >= 1 && p.playactions.Count == 0) ret = -10;
                }

                return ret;
            }

            //some effects, which are bad :D
            int pen = 0;
            if (name == CardDB.cardName.houndmaster)
            {
                //if (target == null || !target.Ready) return 50;

                //foreach (Handmanager.Handcard hcc in p.owncards)
                //{
                //    if (hcc.getManaCost(p) <= p.mana)
                //    {
                //        if ((TAG_RACE)hcc.card.race == TAG_RACE.BEAST) pen += 5;
                //        if (hcc.card.name == CardDB.cardName.animalcompanion) pen += 5;
                //        if (hcc.card.type == CardDB.cardtype.MOB) pen++;
                //    }
                //}

                //if (target == null && p.ownMinions.Count == 0)
                //{
                //    return (p.enemyWeaponAttack >= 3 ? 10: 7);
                //}
                if (target == null) return 12;
                if (target != null)
                {
                    if (!target.Ready) pen += 1;
                }
                return pen;
            }

            if ((card.name == CardDB.cardName.biggamehunter) && (target == null || target.own))
            {
                return 40;
            }
            if (name == CardDB.cardName.aldorpeacekeeper && target == null)
            {
                return 30;
            }

            //if (name == CardDB.cardName.sylvanaswindrunner && p.enemyMinions.Count == 0)
            //{
            //    return 50;
            //}

            if (name == CardDB.cardName.cabalshadowpriest && target == null)
            {
                return 40;
            }

            if (name == CardDB.cardName.emergencycoolant && target != null && target.own)//dont freeze own minions
            {
                return 500;
            }

            if (name == CardDB.cardName.shatteredsuncleric && target == null) { pen = 10; }
            if (name == CardDB.cardName.argentprotector)
            {
                if (target == null) { return 20; }
                else
                {
                    if (!target.own) { return 500; }
                    if (target.divineshild) { return 15; }
                    if (!target.Ready && !target.handcard.card.isSpecialMinion) { return 10; }
                    if (!target.Ready && !target.handcard.card.isSpecialMinion && target.Angr <= 2 && target.Hp <= 2) { return 15; }
                }
            }

            if (name == CardDB.cardName.facelessmanipulator)
            {
                if (target == null)
                {
                    return 50;
                }
                if (target.Angr >= 5 || target.handcard.card.cost >= 5 || (target.handcard.card.rarity == 5 && target.handcard.card.cost >= 3))
                {
                    return 0;
                }
                return 49;
            }

            if (name == CardDB.cardName.ancientofwar)
            {
                if (choice == 1) return 10;
                //if (p.enemyMinions.Count == 0 && choice == 2) return 50;
            }

            if (name == CardDB.cardName.druidoftheflame) //choice1 -> 5/2 choice2 -> 2/5
            {
                if (choice == 1)//5/2
                {
                    if (p.enemyMinions.Find(a => a.Angr >= 2) != null) return 10;
                    if (p.enemyWeaponAttack >= 5) return 0;
                    if (p.enemyWeaponAttack >= 2) return 15;
                }

                if (choice == 2)
                {
                    if (p.enemyMinions.Find(a => a.Angr >= 5 && a.Hp >= 3) != null) return 4;
                    if (p.enemyWeaponAttack >= 5) return 15;
                }

                //if (p.enemyMinions.Count > 0 && choice == 2) return 40;
                //if ((p.enemyMinions.Count == 0 || p.enemyWeaponAttack >= 2) && choice == 1 ) return 40;
            }

            if (name == CardDB.cardName.gangup && target != null)
            {
                if (target.handcard.card.isToken) return 20;
                if (target.handcard.card.isSpecialMinion) return -20;
            }

            if (name == CardDB.cardName.theblackknight)
            {
                float ret = 30;

                if (p.enemyDeckSize <= 12) ret = 5;

                if (target != null)
                {
                    ret = 5;
                }

                foreach (Minion mnn in p.enemyMinions)
                {
                    if (mnn.taunt && (target.Angr >= 3 || target.Hp >= 3)) ret = 0;
                }

                return ret;
            }

            if (name == CardDB.cardName.repentance)
            {
                int ret = 0;
                bool canuse = false;
                foreach (Handmanager.Handcard hcc in p.owncards)
                {
                    if (hcc.card.name == CardDB.cardName.repentance && p.manaTurnEnd >= hcc.getManaCost(p)) canuse = true;
                }
                if (canuse && p.ownMaxMana <= 3) ret += 6 - 2 * p.ownMaxMana;
                if (canuse && p.ownMaxMana >= 4) ret -= (p.ownMaxMana - 2) / 2; //4 = 1. 5=1, 6 = 2...
                return ret;
            }

            if (name == CardDB.cardName.noblesacrifice)
            {
                int ret = 0;
                bool canuse = false;
                bool hassecretkeeper = false;
                foreach (Handmanager.Handcard hcc in p.owncards)
                {
                    if (hcc.card.name == CardDB.cardName.noblesacrifice && p.manaTurnEnd >= hcc.getManaCost(p)) canuse = true;
                    if (hcc.card.name == CardDB.cardName.secretkeeper) hassecretkeeper = true;
                }
                if (canuse) ret = -1;
                if (p.ownMaxMana == 1 && !hassecretkeeper) ret--;
                if (hassecretkeeper) ret += 3;
                return ret;
            }

            if (name == CardDB.cardName.avenge)
            {
                int ret = 0;
                bool canuse = false;
                bool hassecretkeeper = false;
                foreach (Handmanager.Handcard hcc in p.owncards)
                {
                    if (hcc.card.name == CardDB.cardName.avenge && p.manaTurnEnd >= hcc.getManaCost(p)) canuse = true;
                    if (hcc.card.name == CardDB.cardName.secretkeeper) hassecretkeeper = true;
                }
                if (canuse) ret = -1;
                if (p.ownMaxMana == 1 && !hassecretkeeper) ret--;
                if (hassecretkeeper) ret += 3;
                return ret;
            }

            if (name == CardDB.cardName.redemption)
            {
                int ret = 0;
                bool canuse = false;
                bool hassecretkeeper = false;
                foreach (Handmanager.Handcard hcc in p.owncards)
                {
                    if (hcc.card.name == CardDB.cardName.redemption && p.manaTurnEnd >= hcc.getManaCost(p)) canuse = true;
                    if (hcc.card.name == CardDB.cardName.secretkeeper) hassecretkeeper = true;
                }
                if (canuse) ret = -1;
                if (hassecretkeeper) ret += 2;
                return ret;
            }

            //------------------------------------------------------------------------------------------------------
            Minion m = target;

            if (card.name == CardDB.cardName.reincarnate)
            {
                if (m.own)
                {
                    if (m.handcard.card.deathrattle || m.ancestralspirit >= 1 || m.spikeridgedteed >= 1 || m.souloftheforest >= 1 || m.enemyBlessingOfWisdom >= 1 || m.explorershat >= 1) return 0;
                    if (m.handcard.card.Charge && ((m.numAttacksThisTurn == 1 && !m.windfury) || (m.numAttacksThisTurn == 2 && m.windfury))) return 0;
                    if (m.wounded || m.Angr < m.handcard.card.Attack || (m.silenced && instance.specialMinions.ContainsKey(m.name))) return 0;

                    bool hasOnMinionDiesMinion = false;
                    foreach (Minion mnn in p.ownMinions)
                    {
                        if (mnn.name == CardDB.cardName.scavenginghyena && m.handcard.card.race == TAG_RACE.BEAST) hasOnMinionDiesMinion = true;
                        if (mnn.name == CardDB.cardName.flesheatingghoul || mnn.name == CardDB.cardName.cultmaster) hasOnMinionDiesMinion = true;
                    }
                    if (hasOnMinionDiesMinion) return 0;

                    return 500;
                }
                else
                {
                    if (m.name == CardDB.cardName.nerubianegg && m.Angr <= 4 && !m.taunt) return 500;
                    if (m.taunt && !m.handcard.card.tank) return 0;
                    if (m.enemyBlessingOfWisdom >= 1) return 0;
                    if (m.Angr > m.handcard.card.Attack || m.Hp > m.handcard.card.Health) return 0;
                    if (m.name == CardDB.cardName.abomination || m.name == CardDB.cardName.zombiechow || m.name == CardDB.cardName.unstableghoul || m.name == CardDB.cardName.dancingswords) return 0;
                    return 500;
                }
            }

            if (name == CardDB.cardName.madbomber || name == CardDB.cardName.madderbomber)
            {
                //penalize for any own minions with health equal to potential attack amount
                //to lessen risk of losing your own minion
                int maxAtk = 3;
                if (name == CardDB.cardName.madderbomber) maxAtk = 6;
                if (maxAtk >= p.ownHero.Hp && maxAtk < p.enemyHero.Hp) return 500;//we could be killed, but not enemy >_< .. otherwise YOLO
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.Hp <= maxAtk)
                    {
                        if (mnn.Ready) pen += 20;
                    }
                }

                /*
                int numTargets = 2 + p.ownMinions.Count + p.enemyMinions.Count;
                int numOwnTargets = 1 + p.ownMinions.Count;
                int numEnemyTargets = numTargets-numOwnTargets;
                double dmgpertarget = ((double)maxAtk)/((double)numTargets);
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.Hp <= dmgpertarget)
                    {
                        pen += 10;
                    }
                }
                */
            }

            if (name == CardDB.cardName.mechwarper)
            {
                List<Handmanager.Handcard> mechCards = p.owncards.FindAll(hc => hc != playedhcard && hc.card.race == TAG_RACE.MECHANICAL);
                mechCards.Sort((a, b) => a.getManaCost(p).CompareTo(b.getManaCost(p)));  // increasing mana cost

                int maxMechsNextTurnWithoutWarper = 0, maxMechsNextTurnWithWarper = 0;
                int manaNextTurnWithoutWarper = p.ownMaxMana + 1, manaNextTurnWithWarper = p.ownMaxMana + 1;

                for (int i = 0; i < mechCards.Count; i++)
                {
                    int cost = mechCards[i].getManaCost(p);
                    if (manaNextTurnWithoutWarper > cost)
                    {
                        maxMechsNextTurnWithoutWarper++;
                        manaNextTurnWithoutWarper -= cost;
                    }
                    if (manaNextTurnWithWarper > (cost - 1))
                    {
                        maxMechsNextTurnWithWarper++;
                        manaNextTurnWithWarper -= (cost - 1);
                    }
                }

                return -3 * (maxMechsNextTurnWithWarper - maxMechsNextTurnWithoutWarper);  // +1 mana in savings per additional mech
            }

            if (name == CardDB.cardName.goblinblastmage)
            {
                bool mechOnField = false;

                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.handcard.card.race == TAG_RACE.MECHANICAL)
                    {
                        mechOnField = true;
                        break;
                    }
                }

                if (!mechOnField)  // penalize if we can play a mech this turn
                {
                    int lowestCostMechInHand = 1000;
                    foreach (Handmanager.Handcard hc in p.owncards)
                    {
                        if (hc.card.race == TAG_RACE.MECHANICAL && hc.getManaCost(p) < lowestCostMechInHand) lowestCostMechInHand = hc.getManaCost(p);
                    }

                    if (p.mana >= (playedhcard.getManaCost(p) + lowestCostMechInHand)) return 50;
                    if (p.mana >= lowestCostMechInHand) return 20;

                    return 0;
                }
                else  // penalize for randomness of battlecry
                {
                    bool hasNerubianEgg = false;
                    foreach (Minion mnn in p.enemyMinions)
                    {
                        if (mnn.handcard.card.name == CardDB.cardName.nerubianegg && !mnn.silenced && mnn.Hp <= 2)
                        {
                            hasNerubianEgg = true;
                            break;
                        }
                    }

                    // less minions = less randomness to penalize, but more minions = less chance to kill egg, so egg penalty is constant
                    return (hasNerubianEgg ? 10 : p.enemyMinions.Count);
                }
            }

            if (card.name == CardDB.cardName.knifejuggler)
            {
                float retval = 3;
                int hasmob = 0;
                bool canMFBcombo = false;
                int jugglermanacost = 2;
                int MFBmanacost = 3;
                foreach (Handmanager.Handcard hcc in p.owncards)
                {
                    if (hcc.manacost == 1 && hcc.card.name != CardDB.cardName.abusivesergeant) hasmob++;
                    if (hcc.manacost == 2 && (hcc.card.type == CardDB.cardtype.MOB && hcc.card.name != CardDB.cardName.ironbeakowl && hcc.card.name != CardDB.cardName.knifejuggler && hcc.card.name != CardDB.cardName.crazedalchemist && hcc.card.name != CardDB.cardName.direwolfalpha)) hasmob++;
                    if (hcc.card.name == CardDB.cardName.musterforbattle) MFBmanacost = hcc.manacost; canMFBcombo = true;
                    if (hcc.card.name == CardDB.cardName.knifejuggler) jugglermanacost = hcc.manacost;
                }
                //bool enemyhas3hpminions = false;
                //foreach (Minion mnn in p.enemyMinions)
                //{
                //    if (mnn.Hp >= 3) enemyhas3hpminions = true;
                //}
                //if (p.ownMaxMana <= 3 && hasmob >= 1) retval += 5;
                //if ((p.mobsplayedThisTurn >= 1) ||
                //   (canMFBcombo && jugglermanacost + MFBmanacost <= p.mana) ||
                //   ((p.ownHeroAblility.card.name == CardDB.cardName.thesilverhand ||
                //   p.ownHeroAblility.card.name == CardDB.cardName.reinforce ||
                //   p.ownHeroAblility.card.name == CardDB.cardName.totemiccall ||
                //   p.ownHeroAblility.card.name == CardDB.cardName.totemicslam ||
                //   p.ownHeroAblility.card.name == CardDB.cardName.inferno) && !p.ownAbilityReady)) retval += 5;

                //Helpfunctions.Instance.ErrorLog("Juggler penalty = "+ retval);

                if (p.ownMaxMana == 1) retval += 10;

                if (p.ownMaxMana <= 2 && p.enemyWeaponAttack >= 2) retval += 2;

                if (p.mobsPlayedThisTurn >= 1) retval += 0.5f;

                return retval;
            }
            //if (card.name == CardDB.cardName.knifejuggler && (p.mobsPlayedThisTurn > 1 || (p.ownAbilityReady == false && this.summonMinionSpellsDatabase.ContainsKey(p.ownHeroAblility.card.name))))
            // {
            //     return 20;
            //}

            if (card.name == CardDB.cardName.darkshirecouncilman)// && (p.mobsPlayedThisTurn > 0 || (p.ownAbilityReady == false && this.summonMinionSpellsDatabase.ContainsKey(p.ownHeroAblility.card.name))))
            {
                int ret = 0;
                bool hasimpgangboss = false;

                if (p.ownMaxMana <= 2) ret -= 10;
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (p.ownMaxMana <= 3 && hc.card.name == CardDB.cardName.impgangboss && p.enemySecretCount == 0)
                    {
                        ret += 5; hasimpgangboss = true; break;
                    }
                }
                //int enemypotentialattack = 0;
                //foreach (Minion mnn in p.enemyMinions)
                //{
                //    if (mnn.Hp >=2 ) enemypotentialattack += mnn.Angr;
                //}
                //enemypotentialattack += p.enemyWeaponAttack;

                //foreach (Minion mnn in p.ownMinions)
                //{
                //}

                //if (enemypotentialattack >= 5) ret += 8;

                //if (p.enemyMinions.Count == 0 && !hasimpgangboss && p.ownMaxMana <= 3 && p.enemyMinionsDiedTurn == 0) ret -= 10;

                //if ((p.mobsPlayedThisTurn > 0 || (p.ownAbilityReady == false && this.summonMinionSpellsDatabase.ContainsKey(p.ownHeroAblility.card.name)))) ret += 2;
                return ret;
            }

            if (card.name == CardDB.cardName.impgangboss)// && (p.mobsPlayedThisTurn > 0 || (p.ownAbilityReady == false && this.summonMinionSpellsDatabase.ContainsKey(p.ownHeroAblility.card.name))))
            {
                int ret = 0;

                if (p.ownMaxMana <= 2) ret -= 10;

                if ((p.enemyHeroName == HeroEnum.hunter || p.enemyHeroName == HeroEnum.mage) && p.ownMaxMana <= 3 && p.enemySecretCount == 0) ret -= 12;
                return ret;
            }

            if (card.name == CardDB.cardName.flametonguetotem)
            {
                if (p.ownMinions.Count == 0) return 100;
                int ret = 3;
                int readycount = p.ownMinions.Count;
                bool hastaunt = false;
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.allreadyAttacked || mnn.exhausted || (mnn.playedThisTurn && mnn.charge == 0)) readycount--;
                    if (mnn.taunt) hastaunt = true;
                }
                if (p.enemyMinions.Count == 0 && p.ownMaxMana <= 4 && readycount <= 1) ret += 5;
                //if (readycount == 1) ret += 5;
                if (readycount == 0) ret += 5;
                if (readycount == 0 && p.ownMinions.Count >= 1 && p.enemyMinions.Count == 0 && p.ownMaxMana >= 5) ret -= 3;
                //if (hastaunt || (p.enemyMinions.Count == 0 && p.enemyMinionsDiedTurn == 0) && p.ownMaxMana >= 5) ret -= 6;

                //if (zonepos)

                return ret;
            }

            if (card.name == CardDB.cardName.stampedingkodo)
            {
                bool found = false;
                foreach (Minion mi in p.enemyMinions)
                {
                    if (mi.Angr <= 2) found = true;
                }
                if (!found) return 20 - p.ownMaxMana * 2;
            }

            if (name == CardDB.cardName.windfury)
            {
                if (!m.own) return 500;
                if (m.own && !m.Ready) return 500;
            }

            if (name == CardDB.cardName.wildgrowth)
            {
                if (p.ownMaxMana > 5 && p.ownMaxMana < 10 || p.ownMaxMana <= 1) return 500;
            }

            if (name == CardDB.cardName.nourish)
            {
                if (choice == 1 && p.ownMaxMana > 6) return 500;
                if (choice == 1)
                {
                    int penalty = 20;
                    if (p.owncards.Count - 3 >= 5) penalty += (p.owncards.Count - 3 - 10) * 2;
                    return penalty;
                }
            }

            if (card.name == CardDB.cardName.excessmana && card.getManaCost(p, 0) == 0)
            {
                if (p.ownMinions.Find(a => a.name == CardDB.cardName.gadgetzanauctioneer && !a.silenced) != null)
                {
                    return -10;
                }
                return -4;
            }

            if (name == CardDB.cardName.ancestralspirit)
            {
                if (!target.own && !target.isHero)
                {
                    if (m.name == CardDB.cardName.deathlord || m.name == CardDB.cardName.zombiechow || m.name == CardDB.cardName.dancingswords) return 0;
                    return 500;
                }
                if (target.own && !target.isHero)
                {
                    if (this.specialMinions.ContainsKey(m.name)) return -5;
                    return 0;
                }
            }

            if (name == CardDB.cardName.sylvanaswindrunner)
            {
                if (p.enemyMinions.Count == 0)
                {
                    return 10;
                }
            }

            if (name == CardDB.cardName.betrayal && !target.own && !target.isHero)
            {
                if (m.Angr == 0) return 30;
                if (p.enemyMinions.Count == 1) return 30;
            }

            if (heroAttackBuffDatabase.ContainsKey(name))
            {
                return ((p.ownHero.numAttacksThisTurn == 0) && !p.ownHero.frozen) ? heroAttackBuffDatabase[name] * 2 : 20;
            }

            if (name == CardDB.cardName.deadlypoison)
            {
                return -p.ownWeaponDurability * 2;
            }

            if (name == CardDB.cardName.shadydealer)
            {
                bool haspirate = false;
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.handcard.card.race == TAG_RACE.PIRATE)
                    {
                        haspirate = true;
                        break;
                    }
                }
                if (haspirate) return 0;
                else return 10;
            }

            if (name == CardDB.cardName.coldblood)
            {
                if (lethal) return 0;
                return 25;
            }

            if (name == CardDB.cardName.frostbolt)
            {
                return 8;
            }
            if (name == CardDB.cardName.fireball)
            {
                return 16;
            }
            if (name == CardDB.cardName.arcaneblast)
            {
                return 8;
            }
            if (name == CardDB.cardName.shadowstrike) return 8;

            if (!lethal && choice == 1 && name == CardDB.cardName.druidoftheclaw)
            {
                return 10;
            }

            if (name == CardDB.cardName.poweroverwhelming || name == CardDB.cardName.nightmare)
            {
                int ret = 15;
                if (target.own && !target.isHero && !m.Ready)
                {
                    if (target.name == CardDB.cardName.voidcaller) return 50;
                    if (target.name == CardDB.cardName.sylvanaswindrunner) return 100;
                    return 500;
                }

                if (p.enemyMinions.Find(ms => ms.name == CardDB.cardName.doomsayer && !ms.silenced) != null)
                {
                    ret = 5;
                }

                foreach (Minion mnn in p.enemyMinions)
                {
                    if (this.priorityTargets.ContainsKey(mnn.name) && !mnn.silenced)
                    {
                        ret -= this.priorityTargets[mnn.name];
                        //Helpfunctions.Instance.ErrorLog("발견"); break;
                    }
                    else if (mnn.Hp >= 4 || mnn.Angr >= 4) ret -= 5;
                }
                //if (target.own && target.Hp <= 4) ret -= 4 - target.Hp;
                if (target.divineshild) ret += 5;
                //Helpfunctions.Instance.ErrorLog("ret " + ret);
                return ret;
            }

            if (name == CardDB.cardName.frothingberserker)
            {
                //if (p.cardsPlayedThisTurn >= 1 || p.playactions.Find(a => a.actionType == actionEnum.attackWithHero || a.actionType == actionEnum.attackWithMinion) != null) return 15;
            }

            if (name == CardDB.cardName.bloodsailcultist)
            {
                if (p.ownWeaponAttack == 0) return 2;
            }

            if (name == CardDB.cardName.handofprotection)
            {
                if (m.Hp == 1) return 15;
            }

            if (lethal)
            {
                if (name == CardDB.cardName.corruption)
                {
                    int beasts = 0;
                    foreach (Minion mm in p.ownMinions)
                    {
                        if (mm.Ready && (mm.handcard.card.name == CardDB.cardName.questingadventurer || mm.handcard.card.name == CardDB.cardName.archmageantonidas || mm.handcard.card.name == CardDB.cardName.manaaddict || mm.handcard.card.name == CardDB.cardName.manawyrm || mm.handcard.card.name == CardDB.cardName.wildpyromancer)) beasts++;
                    }
                    if (beasts == 0) return 500;
                }
            }

            if (name == CardDB.cardName.divinespirit)
            {
                if (lethal)
                {
                    if (!target.own && !target.isHero)
                    {
                        if (!m.taunt)
                        {
                            return 500;
                        }
                        else
                        {
                            // combo for killing with innerfire and biggamehunter
                            if (p.owncards.Find(x => x.card.name == CardDB.cardName.biggamehunter) != null && p.owncards.Find(x => x.card.name == CardDB.cardName.innerfire) != null && (m.Hp >= 4 || (p.owncards.Find(x => x.card.name == CardDB.cardName.divinespirit) != null && m.Hp >= 2)))
                            {
                                return 0;
                            }
                            return 500;
                        }
                    }
                }
                else
                {
                    if (!target.own && !target.isHero)
                    {
                        // combo for killing with innerfire and biggamehunter
                        if (p.owncards.Find(x => x.card.name == CardDB.cardName.biggamehunter) != null && p.owncards.Find(x => x.card.name == CardDB.cardName.innerfire) != null && m.Hp >= 4)
                        {
                            return 0;
                        }
                        return 500;
                    }
                }

                if (target.own && !target.isHero)
                {
                    if (m.Hp >= 4)
                    {
                        return 0;
                    }
                    return 15;
                }
            }

            if ((name == CardDB.cardName.polymorph || name == CardDB.cardName.hex))
            {
                if (target.own && !target.isHero) return 50;
                //foreach (Action a in p.playactions)
                //{
                //    if (a.target != null && a.target == target) return 500;
                //    if (a.card != null && (DamageAllDatabase.ContainsKey(a.card.card.name) || DamageAllEnemysDatabase.ContainsKey(a.card.card.name)
                //        || DamageRandomDatabase.ContainsKey(a.card.card.name) || randomEffects.ContainsKey(a.card.card.name))) return 500;
                //}

                if (!target.own && !target.isHero)
                {
                    int hexpen = 25;  // base penalty so we don't waste the spell on small minions
                    if (priorityTargets.ContainsKey(target.name) && priorityTargets[target.name] >= 5) hexpen -= 15;
                    else if (target.Hp >= 4 && target.Angr >= 4) hexpen -= 15;
                    else if (target.Hp >= 5) hexpen -= 15;
                    else if (target.Angr >= 5) hexpen -= 15;
                    else if (anti_aoe_minion.ContainsKey(target.name)) hexpen -= 15;
                    else if (target.Angr >= 3 && target.divineshild) hexpen -= 15;

                    if (!target.silenced && priorityTargets.ContainsKey(target.name) && priorityTargets[target.name] >= 11) hexpen = 0;
                    if (!target.silenced && anti_aoe_minion.ContainsKey(target.name) && anti_aoe_minion[target.name] >= 8) hexpen = 0;

                    //Helpfunctions.Instance.ErrorLog("target.name   " + target.name + "  " + anti_aoe_minion.ContainsKey(target.name) + anti_aoe_minion[target.name]);

                    return hexpen;
                }
            }

            if (name == CardDB.cardName.defenderofargus || name == CardDB.cardName.sunfuryprotector)
            {
                switch (p.ownMinions.Count)
                {
                    case 0: return 15;
                    case 1: return 5;
                    default: return 0;
                }
            }

            if (card.name == CardDB.cardName.unleashthehounds)
            {
                //if (p.enemyMinions.Count <= 1)
                //{
                //    return 20;
                //}

                bool synergyavailable = false;
                foreach (Handmanager.Handcard hcc in p.owncards)
                {
                    if (hcc.card.name == CardDB.cardName.houndmaster && hcc.getManaCost(p) + card.getManaCost(p, 3) <= p.mana)
                    {
                        synergyavailable = true;
                    }
                    if (hcc.card.name == CardDB.cardName.killcommand && hcc.getManaCost(p) + card.getManaCost(p, 3) <= p.mana)
                    {
                        synergyavailable = true;
                    }
                    if (hcc.card.name == CardDB.cardName.huntersmark && hcc.getManaCost(p) + card.getManaCost(p, 3) <= p.mana)
                    {
                        synergyavailable = true;
                    }
                }

                if (synergyavailable && p.enemyMinions.Count <= 3) return 14 - 2 * p.enemyMinions.Count;

                if (p.enemyMinions.Count <= 4) return 30 - p.enemyMinions.Count * 6;
            }

            if (name == CardDB.cardName.equality) // aoe penality
            {
                if (p.enemyMinions.Find(a => a.name == CardDB.cardName.doomsayer) != null) return 0;

                if (p.mana == card.getManaCost(p, 2)) return 500; //do not use equality last card. to avoid 8mana 3drawcard -> play equality

                int hpdestroyed = 0;
                foreach (Minion mini in p.enemyMinions)
                {
                    hpdestroyed += (mini.Hp - 1);
                }

                if (p.enemyMinions.Count <= 2 || hpdestroyed <= 4)
                {
                    return 20;
                }
                return 5;
            }

            if (name == CardDB.cardName.bloodsailraider && p.ownWeaponDurability == 0)
            {
                int ret = 0;
                //if you have bloodsailraider and no weapon equiped, but own a weapon:
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.type == CardDB.cardtype.WEAPON) ret = hc.card.Attack;
                }
                return ret;
            }

            if (name == CardDB.cardName.innerfire)
            {
                if (m.name == CardDB.cardName.lightspawn) return 500;
            }

            if (name == CardDB.cardName.huntersmark)
            {
                if (target.own && !target.isHero) return 500; // dont use on own minions
                if (!target.own && !target.isHero && (target.Hp <= 4) && target.Angr <= 4 && !priorityTargets.ContainsKey(target.name)) // only use on strong minions
                {
                    return 15;
                }
                else return 5;
            }

            if ((name == CardDB.cardName.aldorpeacekeeper || name == CardDB.cardName.humility))
            {
                if (target != null)
                {
                    if (target.own && target.Angr >= 2) pen = 500; // dont use on own minions
                    if (!target.own && (target.Angr <= 1 || target.Angr == 2 && target.Hp <= 2)) // only use on strong minions
                    {
                        return 30;
                    }
                    if (target.name == CardDB.cardName.tarcreeper && target.Angr == 3 ||
                        target.name == CardDB.cardName.tarlord && target.Angr == 5 ||
                        target.name == CardDB.cardName.tarlurker && target.Angr == 4 ||
                        target.name == CardDB.cardName.lightspawn && !target.silenced)
                    {
                        return 6;
                    }
                }
                else
                {
                    return 10;
                }
            }

            if (name == CardDB.cardName.defiasringleader && p.cardsPlayedThisTurn == 0)
            { pen = 10; }
            if (name == CardDB.cardName.bloodknight)
            {
                int shilds = 0;
                foreach (Minion min in p.ownMinions)
                {
                    if (min.divineshild)
                    {
                        shilds++;
                    }
                }
                foreach (Minion min in p.enemyMinions)
                {
                    if (min.divineshild)
                    {
                        shilds++;
                    }
                }
                if (shilds == 0)
                {
                    return 10;
                }
            }
            if (name == CardDB.cardName.direwolfalpha)
            {
                int ret = 4;
                int ready = 0;
                //foreach (Minion min in p.ownMinions)
                //{
                //    if (min.Ready) ready++;
                //}
                //if (ready == 0) ret += 4;
                //if (ready == 1) ret += 2;

                //if (p.enemyMinions.Count == 0 && p.ownMaxMana <= 6) ret += 4;

                if (p.ownMaxMana == 1) ret = 45;
                if (p.ownMaxMana <= 2 && p.enemyWeaponAttack >= 2) ret += 2;
                if (p.ownMaxMana >= 7) ret = 0;
                return ret;
            }
            if (name == CardDB.cardName.abusivesergeant || name == CardDB.cardName.darkirondwarf)
            {
                Playfield tmpPf = new Playfield();
                int ret = 5;

                if (name == CardDB.cardName.darkirondwarf && p.mana == 4 && p.enemyMinions.Count == 0) return 3;

                if (p.enemyMinions.Count == 0) ret += 2;
                if (target == null) ret += 15 - p.ownMaxMana;

                //if (target.angr )

                //if (target != null)
                //{
                //    if (p.playactions.Find(a => a.actionType == actionEnum.attackWithMinion && a.own.entityID == target.entityID && a.target.entityID == tmpPf.enemyHero.entityID) != null)
                //    {
                //        ret += 5;
                //    }
                //    //if (!target.own) ret += 1;
                //}
                //Helpfunctions.Instance.ErrorLog("ret" + ret);

                return ret; //lethal
            }

            if (name == CardDB.cardName.bilefintidehunter)
            {
                if (p.ownMinions.Count == 6) return 20;
            }

            if (p.turnCounter >= 1 && name == CardDB.cardName.reversingswitch && target.Angr == target.Hp) return 500;

            if (name == CardDB.cardName.voidwalker && p.ownMaxMana == 1 && p.enemyMinions.Count == 0)
            {
                foreach (Handmanager.Handcard mnn in p.owncards)
                {
                    if (mnn.card.name == CardDB.cardName.argentsquire || mnn.card.name == CardDB.cardName.possessedvillager) return 5;
                }
            }

            if (name == CardDB.cardName.argentsquire && p.ownMaxMana == 1)
            {
                foreach (Handmanager.Handcard mnn in p.owncards)
                {
                    if (mnn.card.name == CardDB.cardName.possessedvillager) return 2;
                }
            }

            if (name == CardDB.cardName.soulfire)
            {
                if (target.isHero)
                {
                    if (target.Hp <= 4 + p.spellpower) return -1;
                }
                return -p.playactions.Count * 0.1f;
            }

            if (name == CardDB.cardName.quickshot)
            {
                float ret = 7;
                if (!target.isHero && priorityTargets.ContainsKey(target.name) && priorityTargets[target.name] >= 6 && target.Hp <= 3 + p.spellpower) ret = 2;
                else if (target.Angr >= 4 && target.Hp <= 3 + p.spellpower) ret = 5;

                if (p.owncards.Count == 1) ret = -1;

                return ret;
            }

            if (name == CardDB.cardName.kingselekk)
            {
                return p.playactions.Count * 0.5f;
            }

            if (name == CardDB.cardName.barnes)
            {
                float ret = 0;

                bool hasinfested = false;
                foreach (Handmanager.Handcard hccc in p.owncards)
                {
                    if (hccc.card.name == CardDB.cardName.infestedwolf && hccc.getManaCost(p) == card.getManaCost(p, 4)) hasinfested = true; break;
                }

                if (hasinfested) ret += 1;

                if (p.ownMaxMana == 4 && !hasinfested) ret -= 2;

                if (p.ownMinions.Count >= 6) ret += 5;

                return ret + p.playactions.Count * 0.1f;
            }

            if (name == CardDB.cardName.kindlygrandmother)
            {
                if (p.ownMaxMana <= 2)
                {
                    foreach (Minion mnn in p.enemyMinions)
                    {
                        if (mnn.Hp == 1 && mnn.maxHp == 1) return -5;
                    }
                    if (p.enemyWeaponAttack >= 1) return -5;
                }
            }

            if (name == CardDB.cardName.animalcompanion)
            {
                float ret = 0;
                foreach (Handmanager.Handcard hcc in p.owncards)
                {
                    if (hcc.card.name == CardDB.cardName.carriongrub && p.ownMinions.Count == 0) ret += 2;
                }

                int readycount = 0;
                foreach (Minion mn in p.ownMinions)
                {
                    if (mn.Ready) readycount++;
                }

                if (readycount >= 3)
                {
                    ret -= readycount * 0.5f;
                }

                return ret + p.playactions.Count * 0.4f;
            }

            if (name == CardDB.cardName.scavenginghyena)
            {
                return 5;
            }

            if (name == CardDB.cardName.savannahhighmane)
            {
                if (p.ownMaxMana <= 6) return -2;
            }

            if (name == CardDB.cardName.argenthorserider)
            {
                return 3;
            }

            if (name == CardDB.cardName.flamewreathedfaceless)
            {
                if (p.ownMaxMana <= 5) return -5;
            }

            if (name == CardDB.cardName.tuskarrtotemic)
            {
                float ret = 0;

                if (p.ownMinions.Count >= 6) ret += 10;

                return p.playactions.Count * 0.5f + ret;
            }

            if (name == CardDB.cardName.totemicmight)
            {
                float ret = 15;
                int totemcount = 0;
                int specialtotemcount = 0;
                bool haswitch = false;
                foreach (Minion mn in p.ownMinions)
                {
                    if (!mn.silenced && (TAG_RACE)mn.handcard.card.race == TAG_RACE.TOTEM) totemcount++;
                    if (!mn.silenced && (mn.name == CardDB.cardName.flametonguetotem || mn.name == CardDB.cardName.manatidetotem || mn.name == CardDB.cardName.primalfintotem)) specialtotemcount++;
                    if (!mn.silenced && mn.name == CardDB.cardName.wickedwitchdoctor) haswitch = true;
                }

                ret -= totemcount * 2;
                ret -= specialtotemcount * 3;
                if (haswitch) ret -= 5;
                return ret;
            }

            if (name == CardDB.cardName.primalfusion)
            {
                float ret = 15;
                int totemcount = 0;
                int specialtotemcount = 0;
                bool haswitch = false;
                foreach (Minion mn in p.ownMinions)
                {
                    if (!mn.silenced && (TAG_RACE)mn.handcard.card.race == TAG_RACE.TOTEM) totemcount++;
                    if (!mn.silenced && (mn.name == CardDB.cardName.flametonguetotem || mn.name == CardDB.cardName.manatidetotem || mn.name == CardDB.cardName.primalfintotem)) specialtotemcount++;
                    if (!mn.silenced && mn.name == CardDB.cardName.wickedwitchdoctor) haswitch = true;
                }

                ret -= totemcount * 2;
                ret -= specialtotemcount * 3;
                if (haswitch) ret -= 5;

                switch (target.name)
                {
                    case CardDB.cardName.healingtotem: ret--; break;
                    case CardDB.cardName.flametonguetotem: ret--; break;
                    case CardDB.cardName.manatidetotem: ret--; break;
                    default: break;
                }

                //if (p.ownHeroAblility.card.name == CardDB.cardName.totemiccall && p.ownAbilityReady && p.ownMinions.Count <= 6 && p.mana >= p.ownHeroAblility.card.getManaCost(p, 2)) ret += 3;

                if (ret <= 0) ret = 0;
                //Helpfunctions.Instance.ErrorLog("ret" + ret);
                return ret;
            }

            if (name == CardDB.cardName.blackwingcorruptor)
            {
                float ret = 0;
                if (target != null)
                {
                    if (target.isHero) ret += 6;
                    if (target.isHero && p.enemyMinions.Count >= 1) ret += 3;
                    if (target.Angr == 0 && target.Hp <= 2 &&
                        p.ownMinions.Find(a => !a.frozen && a.canAttackNormal && a.Angr <= 2 && target.Hp <= a.Angr && (!a.playedThisTurn || a.charge >= 1)) != null) ret += 5;
                    if (target.Hp >= 4 && !target.isHero) ret += 0.5f;
                }
                return ret;
            }

            if (name == CardDB.cardName.fireelemental)
            {
                float ret = 0;
                if (target.isHero) ret += 6;
                if (target.isHero && p.enemyMinions.Count >= 1) ret += 3;
                if (target.Angr == 0 && target.Hp <= 2 &&
                        p.ownMinions.Find(a => !a.frozen && a.canAttackNormal && a.Angr <= 2 && target.Hp <= a.Angr && (!a.playedThisTurn || a.charge >= 1)) != null) ret += 5;
                if (target.Hp >= 4 && !target.isHero) ret += 0.5f;
                return ret;
            }

            if (name == CardDB.cardName.dispatchkodo)
            {
                float ret = 0;
                if (target.isHero) ret += 4;
                if (target.isHero && p.enemyMinions.Count >= 1) ret += 2;

                if (target.Angr == 0 && target.Hp <= 2 && p.ownMinions.Find(a => a.Ready && a.Angr <= 2) != null) ret += 5;
                if (target.Hp >= 4 && !target.isHero) ret += 0.5f;
                return ret;
            }

            if (card.type == CardDB.cardtype.WEAPON)
            {
                float ret = card.Attack * 0.5f;
                if (card.Attack <= p.ownWeaponAttack || (p.ownWeaponDurability >= 2 && !p.ownHero.allreadyAttacked))
                {
                    ret += p.ownWeaponAttack * p.ownWeaponDurability;
                    if (card.Attack == p.ownWeaponAttack && p.ownHero.Ready) ret += card.Attack;
                }

                return ret;
            }

            if (name == CardDB.cardName.wickedwitchdoctor)
            {
                if (p.ownMaxMana <= 3) return 5;
            }

            if (name == CardDB.cardName.deathwing)
            {
                return (p.owncards.Count - 1) * 15;
            }

            if (name == CardDB.cardName.potionofmadness)
            {
                return 10;
            }

            if (name == CardDB.cardName.dreadcorsair)
            {
                float penalty = 0;
                return penalty;
            }

            if (name == CardDB.cardName.smalltimebuccaneer && p.ownMaxMana <= 1)
            {
                bool canplayweapon = false;
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.type == CardDB.cardtype.WEAPON && p.ownMaxMana <= 2) canplayweapon = true;
                    if (hc.card.name == CardDB.cardName.nzothsfirstmate) canplayweapon = true;
                }
                if (canplayweapon && p.enemyMinions.Count == 0) return -2;
            }

            if (name == CardDB.cardName.southseadeckhand)
            {
                int hasweaponIndecks = 0;
                CardDB.Card c;
                foreach (KeyValuePair<CardDB.cardIDEnum, int> cid in Hrtprozis.Instance.turnDeck)
                {
                    c = CardDB.Instance.getCardDataFromID(cid.Key);
                    if (c.name == CardDB.cardName.patchesthepirate)
                    {
                        return 0;
                    }
                    if (c.type == CardDB.cardtype.WEAPON) hasweaponIndecks++;
                }

                //foreach (Handmanager.Handcard hc in Hrtprozis.Instance.deckCard)
                //{
                //    if (hc.card.type == CardDB.cardtype.WEAPON) hasweaponIndecks++;
                //}

                if (p.ownWeaponAttack <= 0 && hasweaponIndecks >= 1)
                {
                    if (p.ownDeckSize >= 1)
                    {
                        if (100 * hasweaponIndecks / p.ownDeckSize >= 15) return 3; // weapon draw chance 15% small penalty
                    }
                }
                return 0;
            }

            if (name == CardDB.cardName.markofyshaarj)
            {
                if ((TAG_RACE)target.handcard.card.race != TAG_RACE.BEAST)
                {
                    return 4;
                }
                else return -4;
            }
            if (name == CardDB.cardName.livingroots)
            {
                if (choice == 1) return 6;
                if (choice == 2) return 4;
            }

            if (name == CardDB.cardName.shadowwordpain)
            {
                if (target.Hp <= 2 && target.Hp <= 2) pen = 15;
                if (target.Hp >= 4) pen = 0;
            }

            if (name == CardDB.cardName.emperorthaurissan)
            {
                return -p.owncards.Count;
            }

            if (name == CardDB.cardName.shieldblock)
            {
                return 5;
            }

            if (name == CardDB.cardName.powerwordshield)
            {
                int ret = 5;
                return ret;
            }

            if (name == CardDB.cardName.powerofthewild && choice == 1)
            {
                if (p.ownMinions.Count <= 2) return 4;
            }

            if (name == CardDB.cardName.menageriewarden)
            {
                bool hasBeast = false;
                bool hasBeastIndecks = false;
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.race == TAG_RACE.BEAST) hasBeast = true;
                }
                if (hasBeast)
                {
                    if (target == null) return 10;
                }
                else
                {
                    foreach (Handmanager.Handcard hc in Hrtprozis.Instance.deckCard)
                    {
                        if ((TAG_RACE)hc.card.race == TAG_RACE.BEAST) hasBeastIndecks = true;
                    }
                }
                if (hasBeastIndecks) return 10;
            }

            if (name == CardDB.cardName.blubberbaron)
            {
                int bccount = 0;
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.battlecry) bccount++;
                }
                return 3 * bccount;
            }

            if (name == CardDB.cardName.freezingpotion) return 10;
            if (name == CardDB.cardName.icelance) return 10;
            if (name == CardDB.cardName.backstab) return 2;
            if (name == CardDB.cardName.innerfire) return 4;
            if (name == CardDB.cardName.forbiddenflame) return 5;

            if (name == CardDB.cardName.openthewaygate ||
                name == CardDB.cardName.awakenthemakers ||
                name == CardDB.cardName.themarshqueen ||
                name == CardDB.cardName.lakkarisacrifice ||
                name == CardDB.cardName.thelastkaleidosaur ||
                name == CardDB.cardName.unitethemurlocs ||
                name == CardDB.cardName.junglegiants ||
                name == CardDB.cardName.fireplumesheart ||
                name == CardDB.cardName.thecavernsbelow)
            {
                if (p.ownMaxMana == 1 && p.mana == 1) return -15; //always use quest first
                else return -10;
            }

            if (name == CardDB.cardName.dirtyrat) return -1 + p.playactions.Count * 0.01f;

            if (name == CardDB.cardName.grimscalechum)
            {
                bool hasmurloc = false;
                float penalty = 0;
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.race == TAG_RACE.MURLOC)
                    {
                        hasmurloc = true;
                        break;
                    }
                }
                if (hasmurloc) penalty -= 2;
                return penalty;
            }

            if (name == CardDB.cardName.spiritecho)
            {
                float ret = 12;
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.handcard.card.isSpecialMinion) ret -= 7;
                    else if (mnn.handcard.card.battlecry || mnn.handcard.card.deathrattle) ret -= 6;
                    else if (!mnn.handcard.card.isToken) ret -= 4;
                    else ret -= 2;
                }
                return ret;
            }

            if (name == CardDB.cardName.cultmaster)
            {
                float ret = 0;
                if (p.enemyMinions.Count == 0) ret += 4;

                return ret;
            }

            if (name == CardDB.cardName.murlocwarleader)
            {
                float ret = 0;
                if (p.ownMinions.Find(a => a.handcard.card.race == TAG_RACE.MURLOC && a.Ready) != null) ret = 3;
                return ret;
            }

            if (name == CardDB.cardName.rockpoolhunter)
            {
                float ret = 0;
                if (p.ownMinions.Find(a => a.handcard.card.race == TAG_RACE.MURLOC) == null) ret = 2;
                return ret;
            }

            if (name == CardDB.cardName.gentlemegasaur)
            {
                float ret = 2;
                if (p.ownMinions.Find(a => a.handcard.card.race == TAG_RACE.MURLOC) == null) ret = 4;

                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.handcard.card.race == TAG_RACE.MURLOC && (mnn.Ready || mnn.playedThisTurn && mnn.charge >= 1)) ret -= 0.5f;
                }

                return ret;
            }

            return pen;
        }

        private int getPlaySecretPenalty(CardDB.Card card, Playfield p)
        {
            int pen = 0;
            if (card.Secret)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    //penalty if we play secret and have playable kirintormage
                    if (hc.card.name == CardDB.cardName.kirintormage && p.mana >= hc.getManaCost(p))
                    {
                        pen = 500;
                    }

                    if (hc.card.name == CardDB.cardName.secretkeeper && p.mana >= card.cost)
                    {
                        pen += 500;
                    }
                }

                //int smallAngr = 0;
                //foreach (Minion m in p.enemyMinions)
                //{
                //    if (m.Angr < 4)
                //    {
                //        smallAngr++;
                //    }
                //}
                //if (smallAngr > 0) pen += 50; //not 500 because we might need to protect ourself from lethal
            }

            return pen;
        }

        //not used-----------------------------------------------------------------------
        ///secret strategys pala
        /// -Attack lowest enemy. If you can’t, use noncombat means to kill it.
        /// -attack with something able to withstand 2 damage.
        /// -Then play something that had low health to begin with to dodge Repentance.
        ///
        ///secret strategys hunter
        /// - kill enemys with your minions with 2 or less heal.
        ///  - Use the smallest minion available for the first attack
        ///  - Then smack them in the face with whatever’s left.
        ///  - If nothing triggered until then, it’s a Snipe, so throw something in front of it that won’t die or is expendable.
        ///
        ///secret strategys mage
        /// - Play a small minion to trigger Mirror Entity.
        /// Then attack the mage directly with the smallest minion on your side.
        /// If nothing triggered by that point, it’s either Spellbender or Counterspell, so hold your spells until you can (and have to!) deal with either.

        private float getPlayCardSecretPenality(CardDB.Card c, Playfield p)
        {
            float pen = 0;
            if (p.enemySecretCount == 0)
            {
                return 0;
            }

            bool hasHighHealthMinion = false;

            if (c.name == CardDB.cardName.flare || c.name == CardDB.cardName.kezanmystic || c.name == CardDB.cardName.eaterofsecrets)
            {
                if (p.playactions.Count >= 1) return 100;
                return 0;
            }
            else
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.name == CardDB.cardName.flare && hc.canplayCard(p)) return 100 * p.enemySecretCount;
                    if (hc.card.name == CardDB.cardName.kezanmystic && hc.canplayCard(p)) return 50;
                    if (hc.card.name == CardDB.cardName.eaterofsecrets && hc.canplayCard(p)) return 100 * p.enemySecretCount;

                    if (hc.card.type == CardDB.cardtype.MOB && hc.card.Health > 4 && hc.canplayCard(p)) hasHighHealthMinion = true;
                }
            }

            int attackedbefore = 0;
            int canattack = 0;

            foreach (Minion mnn in p.ownMinions)
            {
                if (mnn.numAttacksThisTurn >= 1)
                {
                    attackedbefore++;
                }
                if (mnn.Ready && mnn.Angr > 0 && !mnn.frozen)
                {
                    canattack++;
                }
            }

            // only penalize for playing cards if we have better options
            if (p.enemyHeroName == HeroEnum.hunter)
            {
                if (c.type == CardDB.cardtype.MOB || this.summonMinionSpellsDatabase.ContainsKey(c.name) || (c.type == CardDB.cardtype.HEROPWR && (c.name == CardDB.cardName.totemiccall || c.name == CardDB.cardName.totemicslam || c.name == CardDB.cardName.silverhandrecruit || c.name == CardDB.cardName.thesilverhand)))
                {
                    Minion m = new Minion
                    {
                        Hp = c.Health,
                        maxHp = c.Health,
                        Angr = c.Attack,
                        taunt = c.tank,
                        name = c.name
                    };
                    bool haspetunder2hp = false;
                    foreach (Minion mnn in p.ownMinions)
                    {
                        if ((TAG_RACE)mnn.handcard.card.race == TAG_RACE.BEAST) haspetunder2hp = true;
                    }

                    foreach (SecretItem si in Probabilitymaker.Instance.enemySecrets)
                    {
                        if (si.canBe_explosive)
                        {
                            if (canattack > 0 && p.enemyMinions.Find(a => a.taunt) == null && p.playactions.Find(a => (a.actionType == ActionType.ATTACK_WITH_HERO || a.actionType == ActionType.ATTACK_WITH_MINION) && a.target.isHero) == null) pen += 20;
                            if (this.healthBuffDatabase.ContainsKey(c.name) || c.name == CardDB.cardName.defenderofargus) pen = 0;
                            if (haspetunder2hp && c.name == CardDB.cardName.houndmaster) pen = 0;
                        }
                        else if (si.canBe_snipe && p.mobsPlayedThisTurn == 0)
                        {
                            pen += c.calculateManaCost(p);
                            if (c.cost <= 3) pen = 0;
                            if (c.deathrattle && c.name != CardDB.cardName.darnassusaspirant && c.name != CardDB.cardName.dancingswords || c.Shield) pen -= 1;
                        }
                    }

                    //if (c.Health <= 4 && c.deathrattle && c.name != CardDB.cardName.darnassusaspirant && c.name != CardDB.cardName.dancingswords) pen -= 5;
                }
            }

            if (p.enemyHeroName == HeroEnum.mage)
            {
                foreach (SecretItem si in Probabilitymaker.Instance.enemySecrets)
                {
                    if (si.canBe_mirrorentity)
                    {
                        if (c.type == CardDB.cardtype.MOB)
                        {
                            Minion m = new Minion
                            {
                                Hp = c.Health,
                                maxHp = c.Health,
                                Angr = c.Attack,
                                taunt = c.tank,
                                name = c.name
                            };

                            // play first the small minion:
                            if ((!this.isOwnLowestInHand(m, p) && p.mobsPlayedThisTurn == 0)
                                || (p.mobsPlayedThisTurn == 0 && attackedbefore >= 1))
                            {
                                pen += 0.01f;
                            }
                        }
                    }

                    if (si.canBe_iceblock)
                    {
                        int ownHeroPowerATK = 0;
                        switch (c.name)
                        {
                            case CardDB.cardName.steadyshot: ownHeroPowerATK += 2; break;
                            case CardDB.cardName.ballistashot: ownHeroPowerATK += 4; break;
                            default: break;
                        }

                        if (p.enemyHero.Hp <= ownHeroPowerATK && p.enemyHero.Hp >= 1)
                        {
                            switch (p.enemyHero.Hp)
                            {
                                case 1: pen -= 50; break;
                                case 2: pen -= 30; break;
                                default: break;
                            }
                        }
                    }
                }

                // should not penalize playing spells unless 1) we know we have other options, and 2) we know it's a high value spell
                //if (c.type == CardDB.cardtype.SPELL && p.cardsPlayedThisTurn == p.mobsplayedThisTurn)
                //{
                //    pen += 10;
                //}
            }

            if (p.enemyHeroName == HeroEnum.pala)
            {
                if (c.type == CardDB.cardtype.MOB)
                {
                    Minion m = new Minion
                    {
                        Hp = c.Health,
                        maxHp = c.Health,
                        Angr = c.Attack,
                        taunt = c.tank,
                        name = c.name
                    };
                    if ((!this.isOwnLowestInHand(m, p) && p.mobsPlayedThisTurn == 0) || (attackedbefore == 0 && canattack > 0))
                    {
                        pen += 1;
                    }
                }
            }

            return pen;
        }

        private float getAttackSecretPenality(Minion m, Playfield p, Minion target)
        {
            if (p.enemySecretCount == 0)
            {
                return 0;
            }

            foreach (Handmanager.Handcard hc in p.owncards)
            {
                if (hc.card.name == CardDB.cardName.flare && hc.canplayCard(p)) return 100 * p.enemySecretCount;
                if (hc.card.name == CardDB.cardName.kezanmystic && hc.canplayCard(p)) return 50;
                if (hc.card.name == CardDB.cardName.eaterofsecrets && hc.canplayCard(p)) return 100 * p.enemySecretCount;
            }

            float pen = 0;

            int attackedbefore = 0;

            foreach (Minion mnn in p.ownMinions)
            {
                if (mnn.numAttacksThisTurn >= 1) attackedbefore++;
            }

            if (p.enemyHeroName == HeroEnum.hunter)
            {
                bool islow = isOwnLowest(m, p);

                foreach (SecretItem si in Probabilitymaker.Instance.enemySecrets)
                {
                    if (si.canBe_freezing)
                    {
                        //if (p.ownMinions.Find(a => m.Angr + m.Hp > a.Angr + a.Hp) != null) pen += 10;

                        //if (hasMinionsWithLowHeal(p)) pen += 20; //penality if we didn't attack minions before
                        if (attackedbefore == 0 && islow) pen -= 1;
                        //if (attackedbefore == 0 && !islow) pen += 10;
                    }

                    if (si.canBe_explosive)
                    {
                        bool hasHPbuff = false;
                        foreach (Handmanager.Handcard hcc in p.owncards)
                        {
                            if ((healthBuffDatabase.ContainsKey(hcc.card.name) || hcc.card.name == CardDB.cardName.defenderofargus) && hcc.getManaCost(p) >= p.mana) hasHPbuff = true;
                        }

                        if (target.isHero && !target.own && hasHPbuff)
                        {
                            foreach (Minion mnn in p.ownMinions)
                            {
                                if (!mnn.divineshild && !mnn.hasDeathrattle() && mnn.Hp <= 2) pen += 10;
                            }
                        }
                        if (target.isHero && !target.own)
                        {
                            int willbediedminionscount = 0;
                            foreach (Minion mnn in p.ownMinions)
                            {
                                if (!mnn.divineshild && !mnn.hasDeathrattle() && mnn.Hp <= 2) willbediedminionscount += 1;
                                //if (!mnn.divineshild && !mnn.hasDeathrattle() && mnn.Hp <= 2 && mnn.Ready && p.enemyMinions.Count >= 1) pen += 5;
                            }
                            if (willbediedminionscount >= 2) pen += willbediedminionscount;
                            if (willbediedminionscount == 1) pen -= 4; // to trigger enemy explosive trap
                        }
                    }
                }
            }

            if (p.enemyHeroName == HeroEnum.mage)
            {
                if (p.mobsPlayedThisTurn == 0) pen += 1;

                bool islow = isOwnLowest(m, p);

                if (target.isHero && !target.own)
                {
                    foreach (SecretItem si in Probabilitymaker.Instance.enemySecrets)
                    {
                        if (si.canBe_vaporize)
                        {
                            if (target.isHero && !target.own && m.handcard.card.cost <= 3)
                            {
                                pen -= 2;
                            }
                        }

                        if (si.canBe_iceblock && !si.canBe_icebarrier)
                        {
                            if (target.Hp + target.armor <= m.Angr && p.enemyHero.Hp >= 1)
                            {
                                //pen -= 10 - p.enemyHero.Hp;

                                switch (target.Hp + target.armor)
                                {
                                    case 1: pen -= 50; break;
                                    case 2: pen -= 30; break;
                                    case 3: pen -= 20; break;
                                    case 4: pen -= 10; break;
                                    default: break;
                                }
                                if (target.isHero && !target.own && !islow)
                                {
                                    pen += 15;
                                }
                                //Helpfunctions.Instance.ErrorLog("pen" + pen);
                            }
                        }
                        else if (si.canBe_iceblock)
                        {
                            if (target.Hp + target.armor <= m.Angr && p.enemyHero.Hp >= 1)
                            {
                                switch (target.Hp + target.armor)
                                {
                                    case 1: pen -= 30; break;
                                    case 2: pen -= 20; break;
                                    case 3: pen -= 15; break;
                                    case 4: pen -= 5; break;
                                    default: pen -= 3; break;
                                }
                            }
                        }
                    }
                }
            }

            if (p.enemyHeroName == HeroEnum.pala)
            {
                foreach (SecretItem si in Probabilitymaker.Instance.enemySecrets)
                {
                    if (si.canBe_noblesacrifice)
                    {
                        if (!target.own && !m.isHero && m.Hp <= 2 && !m.divineshild && m.Angr >= 3)
                        {
                            pen += 5;
                        }
                    }
                }
            }

            return pen;
        }

        private int getPlayPenalty(CardDB.cardName name, Playfield p, int choice)
        {
            bool hasdragon = false;
            int HandDragonCount = 0;
            bool IsPlayDragon = false;
            bool hasdragonIndecks = false;
            int Deckdragoncount = 0;
            foreach (Handmanager.Handcard hc in p.owncards)
            {
                if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON)
                {
                    hasdragon = true;
                    HandDragonCount++;
                    if (hc.card.name == name) IsPlayDragon = true;
                }
            }

            CardDB.Card c;
            foreach (KeyValuePair<CardDB.cardIDEnum, int> cid in Hrtprozis.Instance.turnDeck)
            {
                c = CardDB.Instance.getCardDataFromID(cid.Key);
                if ((TAG_RACE)c.race == TAG_RACE.DRAGON)
                {
                    hasdragonIndecks = true;
                    Deckdragoncount += cid.Value;
                }
            }

            //Dragon - Hand - synergy cards
            if (hasdragonIndecks && !hasdragon)
            {
                int dragonPen = 0;
                switch (name)
                {
                    case CardDB.cardName.blackwingtechnician:
                        dragonPen = 2; break;
                    case CardDB.cardName.blackwingcorruptor:
                        dragonPen = 6;
                        if (p.enemyHero.Hp + p.enemyHero.armor <= 3) dragonPen += 20;
                        break;

                    case CardDB.cardName.bookwyrm:
                        dragonPen = 8; break;
                    case CardDB.cardName.wyrmrestagent:
                        dragonPen = 4; break;
                    case CardDB.cardName.rendblackhand:
                        dragonPen = 8; break;
                    case CardDB.cardName.alexstraszaschampion:
                        dragonPen = 2; break;
                    case CardDB.cardName.drakonidoperative:
                        dragonPen = 6; break;
                    case CardDB.cardName.nightbanetemplar:
                        dragonPen = 4; break;
                    case CardDB.cardName.netherspitehistorian:
                        dragonPen = 12; break;
                    case CardDB.cardName.twilightwhelp:
                        dragonPen = 10; break;
                    case CardDB.cardName.twilightguardian:
                        dragonPen = 4; break;
                    default:
                        break;
                }
                if (dragonPen >= 1) return dragonPen;
            }

            //last dragon
            if (HandDragonCount <= 1 && IsPlayDragon)
            {
                int dragonPen = 0;
                switch (name)
                {
                    case CardDB.cardName.bookwyrm:
                        dragonPen = 10; break;
                    case CardDB.cardName.drakonidoperative:
                        dragonPen = 10; break;
                    case CardDB.cardName.twilightwhelp:
                        dragonPen = 10; break;
                    default:
                        break;
                }
                if (dragonPen >= 1) return dragonPen;
                //10% draw chance
                if (p.ownDeckSize >= 1)
                {
                    if (100 * Deckdragoncount / p.ownDeckSize >= 10) return 5;
                }
                else return 0;
            }

            switch (name)
            {
                case CardDB.cardName.brannbronzebeard: //play brann before good battlecries
                    return p.playactions.Find(a => a.actionType == ActionType.PLAY_CARD && a.card.card.battlecry && a.card.card.name != CardDB.cardName.flameimp) != null ? 20 : 0;

                case CardDB.cardName.dream:
                    return 6;

                case CardDB.cardName.reliquaryseeker:
                    return (p.ownMinions.Count == 6) ? 0 : 5;
                //case CardDB.cardName.lifetap:
                //    return p.playactions.Count * 2;
                //case CardDB.cardName.soultap:
                //    return p.playactions.Count * 2;
                case CardDB.cardName.silverwaregolem:
                    return 8;

                case CardDB.cardName.malchezaarsimp:
                    return 3;

                case CardDB.cardName.bloodmagethalnos:
                    {
                        if (p.ownMaxMana >= 10) return 2;
                        if (p.ownWeaponName == CardDB.cardName.spiritclaws && p.ownWeaponAttack == 1 && p.enemyMinions.Count == 0)
                        {
                            return 12;
                        }

                        return 8;
                    }

                case CardDB.cardName.thunderbluffvaliant:
                    return 0;

                case CardDB.cardName.jadeidol:
                    if (p.ownHeroName == HeroEnum.druid && p.anzOwnJadeGolem >= 1)
                    {
                        if (choice == 1 && !Hrtprozis.Instance.turnDeck.ContainsKey(CardDB.cardIDEnum.CFM_602)) return 50; //don't waste our last copy
                        if (choice == 2 && Hrtprozis.Instance.turnDeck.ContainsKey(CardDB.cardIDEnum.CFM_602)) return 50; //don't shuffle more in if some still in deck
                    }
                    return 0;

                case CardDB.cardName.arcanemissiles:
                    return 10;

                default:
                    return 0;
            }
        }

        private float getPlayOrder(CardDB.cardName name, Playfield p, int choice)
        {
            float ret = 0;
            if (AdaptDatabase.ContainsKey(name)) ret += p.playactions.Count * 0.1f;

            return ret;
        }

        private int getValueOfMinion(Minion m)
        {
            int ret = 0;
            ret += 2 * m.Angr + m.Hp;
            if (m.taunt) ret++;
            //if (this.priorityDatabase.ContainsKey(m.name)) ret += 20 + priorityDatabase[m.name];
            return ret;
        }

        private bool isOwnLowest(Minion mnn, Playfield p)
        {
            bool ret = true;
            int val = getValueOfMinion(mnn);
            foreach (Minion m in p.ownMinions)
            {
                if (m.playedThisTurn && m.charge == 0 || m.Angr == 0 || !m.Ready && m.allreadyAttacked) continue;
                if (getValueOfMinion(m) < val) ret = false;
            }
            return ret;
        }

        private bool isOwnLowestInHand(Minion mnn, Playfield p)
        {
            bool ret = true;
            Minion m = new Minion();
            int val = getValueOfMinion(mnn);
            foreach (Handmanager.Handcard card in p.owncards)
            {
                if (card.card.type != CardDB.cardtype.MOB) continue;
                CardDB.Card c = card.card;
                m.Hp = c.Health;
                m.maxHp = c.Health;
                m.Angr = c.Attack;
                m.taunt = c.tank;
                m.name = c.name;
                if (getValueOfMinion(m) < val) ret = false;
            }
            return ret;
        }

        private int getValueOfEnemyMinion(Minion m)
        {
            int ret = 0;
            ret += m.Hp;
            if (m.taunt) ret -= 2;
            return ret;
        }

        private bool isEnemyLowest(Minion mnn, Playfield p)
        {
            bool ret = true;
            List<Minion> litt = p.getAttackTargets(true);
            int val = getValueOfEnemyMinion(mnn);
            foreach (Minion m in p.enemyMinions)
            {
                if (litt.Find(x => x.entityID == m.entityID) == null) continue;
                if (getValueOfEnemyMinion(m) < val) ret = false;
            }
            return ret;
        }

        private bool hasMinionsWithLowHeal(Playfield p)
        {
            bool ret = false;
            foreach (Minion m in p.ownMinions)
            {
                if (m.Hp <= 2 && ((m.Ready && m.Angr > 0 && !m.frozen) || this.priorityDatabase.ContainsKey(m.name))) ret = true;
            }
            return ret;
        }

        public int guessTotalSpellDamage(Playfield p, CardDB.cardName name, bool ownplay)
        {
            int dmg = 0;
            if (this.DamageTargetDatabase.ContainsKey(name)) dmg = this.DamageTargetDatabase[name];
            else if (this.DamageTargetSpecialDatabase.ContainsKey(name)) dmg = this.DamageTargetSpecialDatabase[name];
            else if (this.DamageRandomDatabase.ContainsKey(name)) dmg = this.DamageRandomDatabase[name];
            else if (this.DamageHeroDatabase.ContainsKey(name)) dmg = this.DamageHeroDatabase[name];
            else if (this.DamageAllDatabase.ContainsKey(name)) dmg = (p.ownMinions.Count * this.DamageAllDatabase[name] + p.enemyMinions.Count * this.DamageAllDatabase[name]) * 7 / 10;
            else if (this.DamageAllEnemysDatabase.ContainsKey(name)) dmg = p.enemyMinions.Count * this.DamageAllEnemysDatabase[name] * 7 / 10;
            else if (p.anzOwnAuchenaiSoulpriest >= 1)
            {
                if (this.HealAllDatabase.ContainsKey(name)) dmg = (p.ownMinions.Count * this.HealAllDatabase[name] + p.enemyMinions.Count * this.HealAllDatabase[name]) * 7 / 10;
                else if (this.HealTargetDatabase.ContainsKey(name)) dmg = Math.Min(this.HealTargetDatabase[name], 29);
            }

            if (dmg != 0) dmg = (ownplay) ? p.getSpellDamageDamage(dmg) : p.getEnemySpellDamageDamage(dmg);
            return dmg;
        }

        private void setupEnrageDatabase()
        {
            enrageDatabase.Add(CardDB.cardName.amaniberserker, 0);
            enrageDatabase.Add(CardDB.cardName.angrychicken, 0);
            enrageDatabase.Add(CardDB.cardName.grommashhellscream, 0);
            enrageDatabase.Add(CardDB.cardName.ragingworgen, 0);
            enrageDatabase.Add(CardDB.cardName.spitefulsmith, 0);
            enrageDatabase.Add(CardDB.cardName.taurenwarrior, 0);
            enrageDatabase.Add(CardDB.cardName.warbot, 0);
            enrageDatabase.Add(CardDB.cardName.aberrantberserker, 2);
            enrageDatabase.Add(CardDB.cardName.bloodhoofbrave, 3);
        }

        private void setupHealDatabase()
        {
            HealAllDatabase.Add(CardDB.cardName.holynova, 2);//to all friends
            HealAllDatabase.Add(CardDB.cardName.circleofhealing, 4);//all minions
            HealAllDatabase.Add(CardDB.cardName.darkscalehealer, 2);//all friends
            HealAllDatabase.Add(CardDB.cardName.treeoflife, 100);//all chars to max

            HealHeroDatabase.Add(CardDB.cardName.drainlife, 2);//tohero
            HealHeroDatabase.Add(CardDB.cardName.guardianofkings, 6);//tohero
            HealHeroDatabase.Add(CardDB.cardName.holyfire, 5);//tohero
            HealHeroDatabase.Add(CardDB.cardName.priestessofelune, 4);//tohero
            HealHeroDatabase.Add(CardDB.cardName.sacrificialpact, 5);//tohero
            HealHeroDatabase.Add(CardDB.cardName.siphonsoul, 3); //tohero
            HealHeroDatabase.Add(CardDB.cardName.sealoflight, 4); //tohero
            HealHeroDatabase.Add(CardDB.cardName.antiquehealbot, 8); //tohero
            HealHeroDatabase.Add(CardDB.cardName.renojackson, 30); //tohero
            HealHeroDatabase.Add(CardDB.cardName.tuskarrjouster, 7);
            HealHeroDatabase.Add(CardDB.cardName.tournamentmedic, 2);
            HealHeroDatabase.Add(CardDB.cardName.refreshmentvendor, 4);
            HealHeroDatabase.Add(CardDB.cardName.cultapothecary, 2);
            HealHeroDatabase.Add(CardDB.cardName.twilightdarkmender, 10);
            HealHeroDatabase.Add(CardDB.cardName.ivoryknight, 5); //heal = mana cost of discovered spell
            HealHeroDatabase.Add(CardDB.cardName.bindingheal, 5);

            HealTargetDatabase.Add(CardDB.cardName.lightofthenaaru, 3);
            HealTargetDatabase.Add(CardDB.cardName.ancestralhealing, 3);
            HealTargetDatabase.Add(CardDB.cardName.ancientsecrets, 5);
            HealTargetDatabase.Add(CardDB.cardName.holylight, 6);
            HealTargetDatabase.Add(CardDB.cardName.earthenringfarseer, 3);
            HealTargetDatabase.Add(CardDB.cardName.healingtouch, 8);
            HealTargetDatabase.Add(CardDB.cardName.layonhands, 8);
            HealTargetDatabase.Add(CardDB.cardName.lesserheal, 2);
            HealTargetDatabase.Add(CardDB.cardName.voodoodoctor, 2);
            HealTargetDatabase.Add(CardDB.cardName.willofmukla, 8);
            HealTargetDatabase.Add(CardDB.cardName.ancientoflore, 5);
            HealTargetDatabase.Add(CardDB.cardName.healingwave, 14);
            HealTargetDatabase.Add(CardDB.cardName.heal, 4);
            HealTargetDatabase.Add(CardDB.cardName.flashheal, 5);
            HealTargetDatabase.Add(CardDB.cardName.darkshirealchemist, 5);
            HealTargetDatabase.Add(CardDB.cardName.forbiddenhealing, 2);//heal = 2x mana spent
            HealTargetDatabase.Add(CardDB.cardName.moongladeportal, 5);
            HealTargetDatabase.Add(CardDB.cardName.greaterhealingpotion, 12); //to friendly char
            HealTargetDatabase.Add(CardDB.cardName.jinyuwaterspeaker, 6);
            HealTargetDatabase.Add(CardDB.cardName.gadgetzansocialite, 2);
            HealTargetDatabase.Add(CardDB.cardName.hozenhealer, 30); //minion to full hp
            //HealTargetDatabase.Add(CardDB.cardName.divinespirit, 2);
            HealTargetDatabase.Add(CardDB.cardName.bindingheal, 5);

            HealCardButCanUseFullHP.Add(CardDB.cardName.gadgetzansocialite, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.moongladeportal, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.earthenringfarseer, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.voodoodoctor, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.layonhands, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.darkshirealchemist, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.priestessofelune, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.guardianofkings, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.cultapothecary, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.jinyuwaterspeaker, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.refreshmentvendor, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.hozenhealer, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.tuskarrjouster, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.drainlife, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.holyfire, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.siphonsoul, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.tournamentmedic, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.twilightdarkmender, 0);
            HealCardButCanUseFullHP.Add(CardDB.cardName.ivoryknight, 0);
        }

        public void setupDamageDatabase()
        {
            //DamageAllDatabase.Add(CardDB.cardName.flameleviathan, 2);
            DamageAllDatabase.Add(CardDB.cardName.abomination, 2);
            DamageAllDatabase.Add(CardDB.cardName.barongeddon, 2);
            DamageAllDatabase.Add(CardDB.cardName.demonwrath, 2);
            DamageAllDatabase.Add(CardDB.cardName.dreadinfernal, 1);
            DamageAllDatabase.Add(CardDB.cardName.dreadscale, 1);
            DamageAllDatabase.Add(CardDB.cardName.elementaldestruction, 4);
            DamageAllDatabase.Add(CardDB.cardName.excavatedevil, 3);
            DamageAllDatabase.Add(CardDB.cardName.explosivesheep, 2);
            DamageAllDatabase.Add(CardDB.cardName.hellfire, 3);
            DamageAllDatabase.Add(CardDB.cardName.lava, 2);
            DamageAllDatabase.Add(CardDB.cardName.lightbomb, 5);
            DamageAllDatabase.Add(CardDB.cardName.magmapulse, 1);
            DamageAllDatabase.Add(CardDB.cardName.revenge, 1);
            DamageAllDatabase.Add(CardDB.cardName.scarletpurifier, 2);
            DamageAllDatabase.Add(CardDB.cardName.unstableghoul, 1);
            DamageAllDatabase.Add(CardDB.cardName.whirlwind, 1);
            DamageAllDatabase.Add(CardDB.cardName.yseraawakens, 5);
            DamageAllDatabase.Add(CardDB.cardName.anomalus, 8);
            DamageAllDatabase.Add(CardDB.cardName.ravagingghoul, 1);
            DamageAllDatabase.Add(CardDB.cardName.tentacleofnzoth, 1);
            DamageAllDatabase.Add(CardDB.cardName.volcanicpotion, 2);
            DamageAllDatabase.Add(CardDB.cardName.dragonfirepotion, 5);
            DamageAllDatabase.Add(CardDB.cardName.felfirepotion, 5);
            DamageAllDatabase.Add(CardDB.cardName.abyssalenforcer, 3);
            DamageAllDatabase.Add(CardDB.cardName.sleepwiththefishes, 3);

            DamageAllEnemysDatabase.Add(CardDB.cardName.arcaneexplosion, 1);
            DamageAllEnemysDatabase.Add(CardDB.cardName.bladeflurry, 1);
            DamageAllEnemysDatabase.Add(CardDB.cardName.blizzard, 2);
            DamageAllEnemysDatabase.Add(CardDB.cardName.consecration, 2);
            DamageAllEnemysDatabase.Add(CardDB.cardName.fanofknives, 1);
            DamageAllEnemysDatabase.Add(CardDB.cardName.flamestrike, 4);
            DamageAllEnemysDatabase.Add(CardDB.cardName.holynova, 2);
            DamageAllEnemysDatabase.Add(CardDB.cardName.lightningstorm, 2);
            DamageAllEnemysDatabase.Add(CardDB.cardName.locustswarm, 3);
            DamageAllEnemysDatabase.Add(CardDB.cardName.shadowflame, 2);
            DamageAllEnemysDatabase.Add(CardDB.cardName.sporeburst, 1);
            DamageAllEnemysDatabase.Add(CardDB.cardName.starfall, 2);
            DamageAllEnemysDatabase.Add(CardDB.cardName.stomp, 2);
            DamageAllEnemysDatabase.Add(CardDB.cardName.swipe, 1);
            DamageAllEnemysDatabase.Add(CardDB.cardName.darkironskulker, 2);
            DamageAllEnemysDatabase.Add(CardDB.cardName.livingbomb, 5);
            DamageAllEnemysDatabase.Add(CardDB.cardName.poisoncloud, 1);//todo 1 or 2
            DamageAllEnemysDatabase.Add(CardDB.cardName.cthun, 1);
            DamageAllEnemysDatabase.Add(CardDB.cardName.twilightflamecaller, 1);
            DamageAllEnemysDatabase.Add(CardDB.cardName.maelstromportal, 1);

            DamageHeroDatabase.Add(CardDB.cardName.curseofrafaam, 2);
            DamageHeroDatabase.Add(CardDB.cardName.headcrack, 2);
            DamageHeroDatabase.Add(CardDB.cardName.lepergnome, 2);
            DamageHeroDatabase.Add(CardDB.cardName.mindblast, 5);
            DamageHeroDatabase.Add(CardDB.cardName.nightblade, 3);
            DamageHeroDatabase.Add(CardDB.cardName.purecold, 8);
            DamageHeroDatabase.Add(CardDB.cardName.shadowbomber, 3);
            DamageHeroDatabase.Add(CardDB.cardName.sinisterstrike, 3);
            DamageHeroDatabase.Add(CardDB.cardName.frostblast, 3);
            DamageHeroDatabase.Add(CardDB.cardName.necroticaura, 3);
            DamageHeroDatabase.Add(CardDB.cardName.backstreetleper, 2);

            DamageRandomDatabase.Add(CardDB.cardName.arcanemissiles, 1);
            DamageRandomDatabase.Add(CardDB.cardName.avengingwrath, 1);
            DamageRandomDatabase.Add(CardDB.cardName.bomblobber, 4);
            DamageRandomDatabase.Add(CardDB.cardName.boombot, 1);
            DamageRandomDatabase.Add(CardDB.cardName.bouncingblade, 1);
            DamageRandomDatabase.Add(CardDB.cardName.cleave, 2);
            DamageRandomDatabase.Add(CardDB.cardName.demolisher, 2);
            DamageRandomDatabase.Add(CardDB.cardName.flamecannon, 4);
            DamageRandomDatabase.Add(CardDB.cardName.flamejuggler, 1);
            DamageRandomDatabase.Add(CardDB.cardName.flamewaker, 2);
            DamageRandomDatabase.Add(CardDB.cardName.forkedlightning, 2);
            DamageRandomDatabase.Add(CardDB.cardName.goblinblastmage, 1);
            DamageRandomDatabase.Add(CardDB.cardName.hugetoad, 1);
            DamageRandomDatabase.Add(CardDB.cardName.knifejuggler, 1);
            DamageRandomDatabase.Add(CardDB.cardName.madbomber, 1);
            DamageRandomDatabase.Add(CardDB.cardName.madderbomber, 1);
            DamageRandomDatabase.Add(CardDB.cardName.multishot, 3);
            DamageRandomDatabase.Add(CardDB.cardName.ragnarosthefirelord, 8);
            DamageRandomDatabase.Add(CardDB.cardName.rumblingelemental, 1);
            DamageRandomDatabase.Add(CardDB.cardName.shadowboxer, 1);
            DamageRandomDatabase.Add(CardDB.cardName.shipscannon, 2);
            DamageRandomDatabase.Add(CardDB.cardName.boombotjr, 1);
            DamageRandomDatabase.Add(CardDB.cardName.dieinsect, 8);
            DamageRandomDatabase.Add(CardDB.cardName.dieinsects, 8);
            DamageRandomDatabase.Add(CardDB.cardName.throwrocks, 3);
            DamageRandomDatabase.Add(CardDB.cardName.fierybat, 1);
            DamageRandomDatabase.Add(CardDB.cardName.spreadingmadness, 1);
            DamageRandomDatabase.Add(CardDB.cardName.greaterarcanemissiles, 9);

            DamageTargetDatabase.Add(CardDB.cardName.keeperofthegrove, 2); // or silence
            DamageTargetDatabase.Add(CardDB.cardName.arcaneblast, 2);
            DamageTargetDatabase.Add(CardDB.cardName.arcaneshot, 2);
            DamageTargetDatabase.Add(CardDB.cardName.backstab, 2);
            DamageTargetDatabase.Add(CardDB.cardName.barreltoss, 2);
            DamageTargetDatabase.Add(CardDB.cardName.betrayal, 2);
            DamageTargetDatabase.Add(CardDB.cardName.blackwingcorruptor, 3);//if dragon in hand
            DamageTargetDatabase.Add(CardDB.cardName.blizzard, 2);
            DamageTargetDatabase.Add(CardDB.cardName.cobrashot, 3);
            DamageTargetDatabase.Add(CardDB.cardName.coneofcold, 1);
            DamageTargetDatabase.Add(CardDB.cardName.crackle, 3);
            DamageTargetDatabase.Add(CardDB.cardName.damage1, 1);
            DamageTargetDatabase.Add(CardDB.cardName.damage5, 5);
            DamageTargetDatabase.Add(CardDB.cardName.darkbomb, 3);
            DamageTargetDatabase.Add(CardDB.cardName.dragonsbreath, 4);
            DamageTargetDatabase.Add(CardDB.cardName.drainlife, 2);
            DamageTargetDatabase.Add(CardDB.cardName.elvenarcher, 1);
            DamageTargetDatabase.Add(CardDB.cardName.eviscerate, 2);
            DamageTargetDatabase.Add(CardDB.cardName.explosiveshot, 5);
            DamageTargetDatabase.Add(CardDB.cardName.felcannon, 2);
            DamageTargetDatabase.Add(CardDB.cardName.fireball, 6);
            DamageTargetDatabase.Add(CardDB.cardName.fireblast, 1);
            DamageTargetDatabase.Add(CardDB.cardName.fireblastrank2, 2);
            DamageTargetDatabase.Add(CardDB.cardName.fireelemental, 3);
            DamageTargetDatabase.Add(CardDB.cardName.flamelance, 5);// i know its 8 :D
            DamageTargetDatabase.Add(CardDB.cardName.forgottentorch, 3);
            DamageTargetDatabase.Add(CardDB.cardName.frostbolt, 3);
            DamageTargetDatabase.Add(CardDB.cardName.frostshock, 1);
            DamageTargetDatabase.Add(CardDB.cardName.gormoktheimpaler, 4);
            DamageTargetDatabase.Add(CardDB.cardName.hoggersmash, 4);
            DamageTargetDatabase.Add(CardDB.cardName.holyfire, 5);
            DamageTargetDatabase.Add(CardDB.cardName.holysmite, 2);
            DamageTargetDatabase.Add(CardDB.cardName.icelance, 4);//only if iced
            DamageTargetDatabase.Add(CardDB.cardName.implosion, 2);
            DamageTargetDatabase.Add(CardDB.cardName.ironforgerifleman, 1);
            DamageTargetDatabase.Add(CardDB.cardName.killcommand, 3);//or 5
            DamageTargetDatabase.Add(CardDB.cardName.lavaburst, 5);
            DamageTargetDatabase.Add(CardDB.cardName.lightningbolt, 3);
            DamageTargetDatabase.Add(CardDB.cardName.lightningjolt, 2);
            DamageTargetDatabase.Add(CardDB.cardName.livingroots, 2);//choice 1
            DamageTargetDatabase.Add(CardDB.cardName.mindshatter, 3);
            DamageTargetDatabase.Add(CardDB.cardName.mindspike, 2);
            DamageTargetDatabase.Add(CardDB.cardName.moonfire, 1);
            DamageTargetDatabase.Add(CardDB.cardName.mortalcoil, 1);
            DamageTargetDatabase.Add(CardDB.cardName.mortalstrike, 4);
            DamageTargetDatabase.Add(CardDB.cardName.northseakraken, 4);
            DamageTargetDatabase.Add(CardDB.cardName.perditionsblade, 1);
            DamageTargetDatabase.Add(CardDB.cardName.powershot, 2);
            DamageTargetDatabase.Add(CardDB.cardName.pyroblast, 10);
            DamageTargetDatabase.Add(CardDB.cardName.quickshot, 3);//brmcard
            DamageTargetDatabase.Add(CardDB.cardName.roaringtorch, 6);
            DamageTargetDatabase.Add(CardDB.cardName.shadowbolt, 4);
            DamageTargetDatabase.Add(CardDB.cardName.shadowform, 2);
            DamageTargetDatabase.Add(CardDB.cardName.shotgunblast, 1);
            DamageTargetDatabase.Add(CardDB.cardName.si7agent, 2);
            DamageTargetDatabase.Add(CardDB.cardName.starfall, 5);//2 to all enemy
            DamageTargetDatabase.Add(CardDB.cardName.starfire, 5);//draw a card
            DamageTargetDatabase.Add(CardDB.cardName.steadyshot, 2);//or 1 + card
            DamageTargetDatabase.Add(CardDB.cardName.stormpikecommando, 2);
            DamageTargetDatabase.Add(CardDB.cardName.swipe, 4);//1 to others
            DamageTargetDatabase.Add(CardDB.cardName.undercityvaliant, 1);
            DamageTargetDatabase.Add(CardDB.cardName.wrath, 1);//todo 3 or 1+card
            DamageTargetDatabase.Add(CardDB.cardName.sonicbreath, 3);
            DamageTargetDatabase.Add(CardDB.cardName.ballistashot, 3);
            DamageTargetDatabase.Add(CardDB.cardName.unbalancingstrike, 3);
            DamageTargetDatabase.Add(CardDB.cardName.discipleofcthun, 2);
            DamageTargetDatabase.Add(CardDB.cardName.firebloomtoxin, 2);
            DamageTargetDatabase.Add(CardDB.cardName.forbiddenflame, 1); //dmg = mana spent
            DamageTargetDatabase.Add(CardDB.cardName.onthehunt, 1);
            DamageTargetDatabase.Add(CardDB.cardName.shadowstrike, 5);
            DamageTargetDatabase.Add(CardDB.cardName.stormcrack, 4);
            DamageTargetDatabase.Add(CardDB.cardName.firelandsportal, 5);
            DamageTargetDatabase.Add(CardDB.cardName.dispatchkodo, 2); //dmg = ap
            DamageTargetDatabase.Add(CardDB.cardName.jadeshuriken, 2);
            DamageTargetDatabase.Add(CardDB.cardName.jadelightning, 4);
            DamageTargetDatabase.Add(CardDB.cardName.blowgillsniper, 1);
            DamageTargetDatabase.Add(CardDB.cardName.bombsquad, 5);
            DamageTargetDatabase.Add(CardDB.cardName.tidalsurge, 4);
            DamageTargetDatabase.Add(CardDB.cardName.razorpetalvolley, 1);
            DamageTargetDatabase.Add(CardDB.cardName.meteor, 15);
            DamageTargetDatabase.Add(CardDB.cardName.grievousbite, 2);
            DamageTargetDatabase.Add(CardDB.cardName.feedingtime, 3);

            DamageTargetSpecialDatabase.Add(CardDB.cardName.bash, 3); //+3 armor
            DamageTargetSpecialDatabase.Add(CardDB.cardName.crueltaskmaster, 1); // gives 2 attack
            DamageTargetSpecialDatabase.Add(CardDB.cardName.deathbloom, 5);
            DamageTargetSpecialDatabase.Add(CardDB.cardName.demonfire, 2); // friendly demon get +2/+2
            DamageTargetSpecialDatabase.Add(CardDB.cardName.demonheart, 5);
            DamageTargetSpecialDatabase.Add(CardDB.cardName.earthshock, 1); //SILENCE /good for raggy etc or iced
            DamageTargetSpecialDatabase.Add(CardDB.cardName.hammerofwrath, 3); //draw a card
            DamageTargetSpecialDatabase.Add(CardDB.cardName.holywrath, 2);//draw a card
            DamageTargetSpecialDatabase.Add(CardDB.cardName.innerrage, 1); // gives 2 attack
            DamageTargetSpecialDatabase.Add(CardDB.cardName.lavashock, 2); //erases overload
            DamageTargetSpecialDatabase.Add(CardDB.cardName.roguesdoit, 4);//draw a card
            DamageTargetSpecialDatabase.Add(CardDB.cardName.savagery, 1);//dmg=herodamage
            DamageTargetSpecialDatabase.Add(CardDB.cardName.shieldslam, 1);//dmg=armor
            DamageTargetSpecialDatabase.Add(CardDB.cardName.shiv, 1);//draw a card
            DamageTargetSpecialDatabase.Add(CardDB.cardName.slam, 2);//draw card if it survives
            DamageTargetSpecialDatabase.Add(CardDB.cardName.soulfire, 4);//delete a card
            DamageTargetSpecialDatabase.Add(CardDB.cardName.quickshot, 3); //draw a card
            DamageTargetSpecialDatabase.Add(CardDB.cardName.bloodtoichor, 1);
            DamageTargetSpecialDatabase.Add(CardDB.cardName.baneofdoom, 2);

            DamageTargetSpecialDatabase.Add(CardDB.cardName.flamegeyser, 2);
        }

        private void setupsilenceDatabase()
        {
            silenceDatabase.Add(CardDB.cardName.dispel, 1);
            silenceDatabase.Add(CardDB.cardName.earthshock, 1);
            silenceDatabase.Add(CardDB.cardName.massdispel, 1);  //enemy minions
            silenceDatabase.Add(CardDB.cardName.silence, 1);
            silenceDatabase.Add(CardDB.cardName.keeperofthegrove, 1);
            silenceDatabase.Add(CardDB.cardName.ironbeakowl, 1);
            silenceDatabase.Add(CardDB.cardName.spellbreaker, 1);
            silenceDatabase.Add(CardDB.cardName.lightschampion, 1); //a demon
            silenceDatabase.Add(CardDB.cardName.purify, 1); //a friendly minion
            silenceDatabase.Add(CardDB.cardName.kabalsongstealer, 1);
            silenceDatabase.Add(CardDB.cardName.defiascleaner, 1); //a deathrattle minion
            silenceDatabase.Add(CardDB.cardName.wailingsoul, 1); //your minions
        }

        private void setupPriorityList()
        {
            priorityDatabase.Add(CardDB.cardName.acidmaw, 3);
            priorityDatabase.Add(CardDB.cardName.alakirthewindlord, 4);
            priorityDatabase.Add(CardDB.cardName.animatedarmor, 2);
            priorityDatabase.Add(CardDB.cardName.archmageantonidas, 10);
            priorityDatabase.Add(CardDB.cardName.armorsmith, 1);
            priorityDatabase.Add(CardDB.cardName.auchenaisoulpriest, 2);
            priorityDatabase.Add(CardDB.cardName.aviana, 5);
            priorityDatabase.Add(CardDB.cardName.brannbronzebeard, 4);
            priorityDatabase.Add(CardDB.cardName.bloodmagethalnos, 2);
            priorityDatabase.Add(CardDB.cardName.buccaneer, 2);
            priorityDatabase.Add(CardDB.cardName.cloakedhuntress, 5);
            priorityDatabase.Add(CardDB.cardName.coldarradrake, 2);
            priorityDatabase.Add(CardDB.cardName.confessorpaletress, 7);
            priorityDatabase.Add(CardDB.cardName.crowdfavorite, 6);
            priorityDatabase.Add(CardDB.cardName.cultmaster, 3);
            priorityDatabase.Add(CardDB.cardName.cutpurse, 2);
            priorityDatabase.Add(CardDB.cardName.dalaranaspirant, 4);
            priorityDatabase.Add(CardDB.cardName.darkshirecouncilman, 4);
            priorityDatabase.Add(CardDB.cardName.dementedfrostcaller, 2);
            priorityDatabase.Add(CardDB.cardName.demolisher, 1);
            priorityDatabase.Add(CardDB.cardName.djinniofzephyrs, 4);
            priorityDatabase.Add(CardDB.cardName.direwolfalpha, 5);
            priorityDatabase.Add(CardDB.cardName.dragonhawkrider, 2);
            priorityDatabase.Add(CardDB.cardName.dustdevil, 1);
            priorityDatabase.Add(CardDB.cardName.emperorthaurissan, 5);
            priorityDatabase.Add(CardDB.cardName.etherealarcanist, 3);
            priorityDatabase.Add(CardDB.cardName.fandralstaghelm, 6);
            priorityDatabase.Add(CardDB.cardName.flametonguetotem, 6);
            priorityDatabase.Add(CardDB.cardName.flesheatingghoul, 1);
            priorityDatabase.Add(CardDB.cardName.flamewaker, 5);
            priorityDatabase.Add(CardDB.cardName.frothingberserker, 3);
            priorityDatabase.Add(CardDB.cardName.gadgetzanauctioneer, 5);
            priorityDatabase.Add(CardDB.cardName.garrisoncommander, 1);
            priorityDatabase.Add(CardDB.cardName.grimpatron, 5);
            priorityDatabase.Add(CardDB.cardName.grimscaleoracle, 5);
            priorityDatabase.Add(CardDB.cardName.grommashhellscream, 4);
            priorityDatabase.Add(CardDB.cardName.hallazealtheascended, 4);
            priorityDatabase.Add(CardDB.cardName.hogger, 4);
            priorityDatabase.Add(CardDB.cardName.holychampion, 5);
            priorityDatabase.Add(CardDB.cardName.hoodedacolyte, 2);
            priorityDatabase.Add(CardDB.cardName.illidanstormrage, 4);
            priorityDatabase.Add(CardDB.cardName.knifejuggler, 4);
            priorityDatabase.Add(CardDB.cardName.kodorider, 6);
            priorityDatabase.Add(CardDB.cardName.kvaldirraider, 1);
            priorityDatabase.Add(CardDB.cardName.leokk, 5);
            priorityDatabase.Add(CardDB.cardName.lightspawn, 4);
            priorityDatabase.Add(CardDB.cardName.maidenofthelake, 2);
            priorityDatabase.Add(CardDB.cardName.malchezaarsimp, 1);
            priorityDatabase.Add(CardDB.cardName.malganis, 10);
            priorityDatabase.Add(CardDB.cardName.malygos, 10);
            priorityDatabase.Add(CardDB.cardName.manaaddict, 1);
            priorityDatabase.Add(CardDB.cardName.manatidetotem, 5);
            priorityDatabase.Add(CardDB.cardName.manawyrm, 1);
            priorityDatabase.Add(CardDB.cardName.masterswordsmith, 1);
            priorityDatabase.Add(CardDB.cardName.mechwarper, 1);
            priorityDatabase.Add(CardDB.cardName.moroes, 10);
            priorityDatabase.Add(CardDB.cardName.muklaschampion, 5);
            priorityDatabase.Add(CardDB.cardName.murlocknight, 5);
            priorityDatabase.Add(CardDB.cardName.murloctidecaller, 2);
            priorityDatabase.Add(CardDB.cardName.murlocwarleader, 5);
            priorityDatabase.Add(CardDB.cardName.natpagle, 2);
            priorityDatabase.Add(CardDB.cardName.nexuschampionsaraad, 6);
            priorityDatabase.Add(CardDB.cardName.northshirecleric, 5);
            priorityDatabase.Add(CardDB.cardName.obsidiandestroyer, 4);
            priorityDatabase.Add(CardDB.cardName.orgrimmaraspirant, 2);
            priorityDatabase.Add(CardDB.cardName.pintsizedsummoner, 3);
            priorityDatabase.Add(CardDB.cardName.priestofthefeast, 3);
            priorityDatabase.Add(CardDB.cardName.prophetvelen, 5);
            priorityDatabase.Add(CardDB.cardName.questingadventurer, 4);
            priorityDatabase.Add(CardDB.cardName.ragnaroslightlord, 5);
            priorityDatabase.Add(CardDB.cardName.raidleader, 5);
            priorityDatabase.Add(CardDB.cardName.recruiter, 1);
            priorityDatabase.Add(CardDB.cardName.rumblingelemental, 2);
            priorityDatabase.Add(CardDB.cardName.savagecombatant, 5);
            priorityDatabase.Add(CardDB.cardName.scalednightmare, 4);
            priorityDatabase.Add(CardDB.cardName.scavenginghyena, 5);
            priorityDatabase.Add(CardDB.cardName.secretkeeper, 1);
            priorityDatabase.Add(CardDB.cardName.shadowfiend, 3);
            priorityDatabase.Add(CardDB.cardName.sorcerersapprentice, 3);
            priorityDatabase.Add(CardDB.cardName.southseacaptain, 5);
            priorityDatabase.Add(CardDB.cardName.stormwindchampion, 5);
            priorityDatabase.Add(CardDB.cardName.summoningportal, 5);
            priorityDatabase.Add(CardDB.cardName.summoningstone, 5);
            priorityDatabase.Add(CardDB.cardName.tournamentmedic, 2);
            priorityDatabase.Add(CardDB.cardName.thunderbluffvaliant, 3);
            priorityDatabase.Add(CardDB.cardName.timberwolf, 3);
            priorityDatabase.Add(CardDB.cardName.tinyknightofevil, 1);
            priorityDatabase.Add(CardDB.cardName.tunneltrogg, 4);
            priorityDatabase.Add(CardDB.cardName.unboundelemental, 2);
            priorityDatabase.Add(CardDB.cardName.usherofsouls, 2);
            priorityDatabase.Add(CardDB.cardName.voidcrusher, 1);
            priorityDatabase.Add(CardDB.cardName.violetillusionist, 10);
            priorityDatabase.Add(CardDB.cardName.violetteacher, 1);
            priorityDatabase.Add(CardDB.cardName.warhorsetrainer, 5);
            priorityDatabase.Add(CardDB.cardName.warsongcommander, 2);
            priorityDatabase.Add(CardDB.cardName.wickedwitchdoctor, 3);
            priorityDatabase.Add(CardDB.cardName.wilfredfizzlebang, 5);
            priorityDatabase.Add(CardDB.cardName.ysera, 10);

            priorityDatabase.Add(CardDB.cardName.grimestreetenforcer, 10);
            priorityDatabase.Add(CardDB.cardName.shakuthecollector, 5);
            priorityDatabase.Add(CardDB.cardName.kabaltrafficker, 5);
            priorityDatabase.Add(CardDB.cardName.grimygadgeteer, 5);
            priorityDatabase.Add(CardDB.cardName.friendlybartender, 1);
            priorityDatabase.Add(CardDB.cardName.auctionmasterbeardo, 5);
            priorityDatabase.Add(CardDB.cardName.backroombouncer, 5);
            priorityDatabase.Add(CardDB.cardName.daringreporter, 1);
            priorityDatabase.Add(CardDB.cardName.burglybully, 3);
            priorityDatabase.Add(CardDB.cardName.redmanawyrm, 5);
        }

        public void setupAttackBuff()
        {
            heroAttackBuffDatabase.Add(CardDB.cardName.bite, 4);
            heroAttackBuffDatabase.Add(CardDB.cardName.claw, 2);
            heroAttackBuffDatabase.Add(CardDB.cardName.heroicstrike, 4);
            heroAttackBuffDatabase.Add(CardDB.cardName.evolvespines, 4);
            heroAttackBuffDatabase.Add(CardDB.cardName.feralrage, 4);

            attackBuffDatabase.Add(CardDB.cardName.abusivesergeant, 2);
            attackBuffDatabase.Add(CardDB.cardName.bananas, 1);
            attackBuffDatabase.Add(CardDB.cardName.bestialwrath, 2); // NEVER ON enemy MINION
            attackBuffDatabase.Add(CardDB.cardName.blessingofkings, 4);
            attackBuffDatabase.Add(CardDB.cardName.blessingofmight, 3);
            attackBuffDatabase.Add(CardDB.cardName.bolster, 2);
            attackBuffDatabase.Add(CardDB.cardName.clockworkknight, 1);
            attackBuffDatabase.Add(CardDB.cardName.coldblood, 2);
            attackBuffDatabase.Add(CardDB.cardName.crueltaskmaster, 2);
            attackBuffDatabase.Add(CardDB.cardName.darkirondwarf, 2);
            attackBuffDatabase.Add(CardDB.cardName.darkwispers, 5); //choice 2
            attackBuffDatabase.Add(CardDB.cardName.demonfuse, 3);
            attackBuffDatabase.Add(CardDB.cardName.explorershat, 1);
            attackBuffDatabase.Add(CardDB.cardName.innerrage, 2);
            attackBuffDatabase.Add(CardDB.cardName.lancecarrier, 2);
            attackBuffDatabase.Add(CardDB.cardName.markofnature, 4); //choice1
            attackBuffDatabase.Add(CardDB.cardName.markofthewild, 2);
            attackBuffDatabase.Add(CardDB.cardName.metaltoothleaper, 2);
            attackBuffDatabase.Add(CardDB.cardName.nightmare, 5); //destroy minion on next turn
            attackBuffDatabase.Add(CardDB.cardName.rampage, 3); //only damaged minion
            attackBuffDatabase.Add(CardDB.cardName.rockbiterweapon, 3);
            attackBuffDatabase.Add(CardDB.cardName.screwjankclunker, 2);
            attackBuffDatabase.Add(CardDB.cardName.sealofchampions, 3);
            attackBuffDatabase.Add(CardDB.cardName.uproot, 5);
            attackBuffDatabase.Add(CardDB.cardName.velenschosen, 2);
            attackBuffDatabase.Add(CardDB.cardName.whirlingblades, 1);
            attackBuffDatabase.Add(CardDB.cardName.briarthorntoxin, 3);
            attackBuffDatabase.Add(CardDB.cardName.divinestrength, 1);
            attackBuffDatabase.Add(CardDB.cardName.lanternofpower, 10);
            attackBuffDatabase.Add(CardDB.cardName.markofyshaarj, 2);
            attackBuffDatabase.Add(CardDB.cardName.mutatinginjection, 4);
            attackBuffDatabase.Add(CardDB.cardName.powerwordtentacles, 2);
            attackBuffDatabase.Add(CardDB.cardName.primalfusion, 1);
            attackBuffDatabase.Add(CardDB.cardName.poweroverwhelming, 4);
            attackBuffDatabase.Add(CardDB.cardName.silvermoonportal, 2);
            attackBuffDatabase.Add(CardDB.cardName.zoobot, 1);
            attackBuffDatabase.Add(CardDB.cardName.menageriemagician, 2);
            //attackBuffDatabase.Add(CardDB.cardName.powerofthewild, 1); //choice 1
            attackBuffDatabase.Add(CardDB.cardName.markofthelotus, 1);
            attackBuffDatabase.Add(CardDB.cardName.virmensensei, 2); //to a beast
            attackBuffDatabase.Add(CardDB.cardName.shadowsensei, 2); //to a stealth
            attackBuffDatabase.Add(CardDB.cardName.bloodfurypotion, 3);
            attackBuffDatabase.Add(CardDB.cardName.crystalweaver, 1); //to demons
            
            //ungoro
            attackBuffDatabase.Add(CardDB.cardName.rockpoolhunter, 1); //to murloc
            attackBuffDatabase.Add(CardDB.cardName.spikeridgedsteed, 2);
            attackBuffDatabase.Add(CardDB.cardName.earthenscales, 1);
        }

        private void setupHealthBuff()
        {
            //healthBuffDatabase.Add(CardDB.cardName.ancientofwar, 5);//choice2 is only buffing himself!
            //healthBuffDatabase.Add(CardDB.cardName.rooted, 5);
            healthBuffDatabase.Add(CardDB.cardName.armorplating, 1);
            healthBuffDatabase.Add(CardDB.cardName.bananas, 1);
            healthBuffDatabase.Add(CardDB.cardName.blessingofkings, 4);
            healthBuffDatabase.Add(CardDB.cardName.bolster, 2);
            healthBuffDatabase.Add(CardDB.cardName.clockworkknight, 1);
            healthBuffDatabase.Add(CardDB.cardName.competitivespirit, 1);
            healthBuffDatabase.Add(CardDB.cardName.darkwispers, 5);//choice2
            healthBuffDatabase.Add(CardDB.cardName.demonfuse, 3);
            healthBuffDatabase.Add(CardDB.cardName.explorershat, 1);
            healthBuffDatabase.Add(CardDB.cardName.markofnature, 4);//choice2
            healthBuffDatabase.Add(CardDB.cardName.markofthewild, 2);
            healthBuffDatabase.Add(CardDB.cardName.nightmare, 5);
            healthBuffDatabase.Add(CardDB.cardName.powerwordshield, 2);
            healthBuffDatabase.Add(CardDB.cardName.rampage, 3);
            healthBuffDatabase.Add(CardDB.cardName.screwjankclunker, 2);
            healthBuffDatabase.Add(CardDB.cardName.upgradedrepairbot, 4);
            healthBuffDatabase.Add(CardDB.cardName.velenschosen, 4);
            healthBuffDatabase.Add(CardDB.cardName.wildwalker, 3);
            healthBuffDatabase.Add(CardDB.cardName.divinestrength, 2);
            healthBuffDatabase.Add(CardDB.cardName.lanternofpower, 10);
            healthBuffDatabase.Add(CardDB.cardName.markofyshaarj, 2);
            healthBuffDatabase.Add(CardDB.cardName.mutatinginjection, 4);
            healthBuffDatabase.Add(CardDB.cardName.powerwordtentacles, 6);
            healthBuffDatabase.Add(CardDB.cardName.primalfusion, 1);
            healthBuffDatabase.Add(CardDB.cardName.silvermoonportal, 2);
            healthBuffDatabase.Add(CardDB.cardName.zoobot, 1);
            healthBuffDatabase.Add(CardDB.cardName.menageriemagician, 2);
            healthBuffDatabase.Add(CardDB.cardName.powerofthewild, 1); //choice 1
            healthBuffDatabase.Add(CardDB.cardName.markofthelotus, 1);
            healthBuffDatabase.Add(CardDB.cardName.virmensensei, 2); //to a beast
            healthBuffDatabase.Add(CardDB.cardName.shadowsensei, 2); //to a stealth
            healthBuffDatabase.Add(CardDB.cardName.bloodfurypotion, 3); //if demon
            healthBuffDatabase.Add(CardDB.cardName.crystalweaver, 1); //to demons
            healthBuffDatabase.Add(CardDB.cardName.kabaltalonpriest, 3);

            //ungoro
            healthBuffDatabase.Add(CardDB.cardName.rockpoolhunter, 1); //to murloc
            healthBuffDatabase.Add(CardDB.cardName.spikeridgedsteed, 6);
            healthBuffDatabase.Add(CardDB.cardName.earthenscales, 1);

            tauntBuffDatabase.Add(CardDB.cardName.markofnature, 1);
            tauntBuffDatabase.Add(CardDB.cardName.markofthewild, 1);
            tauntBuffDatabase.Add(CardDB.cardName.darkwispers, 1);
            tauntBuffDatabase.Add(CardDB.cardName.rustyhorn, 1);
            tauntBuffDatabase.Add(CardDB.cardName.mutatinginjection, 1);
            tauntBuffDatabase.Add(CardDB.cardName.ancestralhealing, 1);
            tauntBuffDatabase.Add(CardDB.cardName.sparringpartner, 1);
            tauntBuffDatabase.Add(CardDB.cardName.spikeridgedsteed, 1);
        }

        private void setupCardDrawBattlecry()
        {
            cardDrawBattleCryDatabase.Add(CardDB.cardName.solemnvigil, 2);

            cardDrawBattleCryDatabase.Add(CardDB.cardName.wrath, 1); //choice=2
            cardDrawBattleCryDatabase.Add(CardDB.cardName.ancientoflore, 2);// choice =1
            cardDrawBattleCryDatabase.Add(CardDB.cardName.nourish, 3); //choice = 2
            cardDrawBattleCryDatabase.Add(CardDB.cardName.grovetender, 1); //choice = 2

            cardDrawBattleCryDatabase.Add(CardDB.cardName.ancientteachings, 2);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.flare, 1);

            cardDrawBattleCryDatabase.Add(CardDB.cardName.excessmana, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.starfire, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.azuredrake, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.coldlightoracle, 2);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.gnomishinventor, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.harrisonjones, 0);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.noviceengineer, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.roguesdoit, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.arcaneintellect, 2);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.hammerofwrath, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.holywrath, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.layonhands, 3);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.massdispel, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.powerwordshield, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.fanofknives, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.shiv, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.sprint, 4);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.farsight, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.lifetap, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.commandingshout, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.shieldblock, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.slam, 1); //if survives
            cardDrawBattleCryDatabase.Add(CardDB.cardName.mortalcoil, 1);//only if kills
            cardDrawBattleCryDatabase.Add(CardDB.cardName.battlerage, 1);//only if wounded own minions
            cardDrawBattleCryDatabase.Add(CardDB.cardName.divinefavor, 1);//only if enemy has more cards than you

            cardDrawBattleCryDatabase.Add(CardDB.cardName.neptulon, 4);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.gnomishexperimenter, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.unstableportal, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.callpet, 1);

            //cardDrawBattleCryDatabase.Add(CardDB.cardName.quickshot, 1);//only if my own cards0

            cardDrawBattleCryDatabase.Add(CardDB.cardName.grandcrusader, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.nexuschampionsaraad, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.spellslinger, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.burgle, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.ancestralknowledge, 2);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.varianwrynn, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.ambush, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.soultap, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.lockandload, 1);
            //cardDrawBattleCryDatabase.Add(CardDB.cardName.kingselekk, 1);  // only if we win joust

            cardDrawBattleCryDatabase.Add(CardDB.cardName.tinkertowntechnician, 1); // if we have a mech
            cardDrawBattleCryDatabase.Add(CardDB.cardName.toshley, 1);

            cardDrawBattleCryDatabase.Add(CardDB.cardName.maptothegoldenmonkey, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.goldenmonkey, 1); //no carddraw, but new cards

            cardDrawBattleCryDatabase.Add(CardDB.cardName.bloodwarriors, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.cabaliststome, 3);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.darkshirelibrarian, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.flameheart, 2);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.kingsbloodtoxin, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.markofyshaarj, 0);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.muklatyrantofthevale, 2);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.shadowcaster, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.thistletea, 3);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.xarilpoisonedmind, 1);

            cardDrawBattleCryDatabase.Add(CardDB.cardName.babblingbook, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.thecurator, 1);

            //cardDrawBattleCryDatabase.Add(CardDB.cardName.quickshot, 1);

            cardDrawBattleCryDatabase.Add(CardDB.cardName.fightpromoter, 2);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.wrathion, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.lunarvisions, 2);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.smalltimerecruits, 3);

            //ungoro
            cardDrawBattleCryDatabase.Add(CardDB.cardName.arcanologist, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.brighteyedscout, 1);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.tolvirwarden, 1); //0-2
            cardDrawBattleCryDatabase.Add(CardDB.cardName.mimicpod, 2);
            cardDrawBattleCryDatabase.Add(CardDB.cardName.ungoropack, 5);

            cardDrawDeathrattleDatabase.Add(CardDB.cardName.bloodmagethalnos, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.clockworkgnome, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.dancingswords, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.loothoarder, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.mechanicalyeti, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.mechbearcat, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.tombpillager, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.toshley, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.webspinner, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.acolyteofpain, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.pollutedhoarder, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.shiftingshade, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.undercityhuckster, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.xarilpoisonedmind, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.deadlyfork, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.runicegg, 1);
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.meanstreetmarshal, 1);

            //ungoro
            cardDrawDeathrattleDatabase.Add(CardDB.cardName.crystallineoracle, 1);

            //add discover minions
            foreach (CardDB.cardName discoverCard in this.discoverCards.Keys)
            {
                cardDrawBattleCryDatabase.Add(discoverCard, 1);
            }
        }

        private void setupDiscardCards()
        {
            cardDiscardDatabase.Add(CardDB.cardName.doomguard, 5);
            cardDiscardDatabase.Add(CardDB.cardName.soulfire, 1);
            cardDiscardDatabase.Add(CardDB.cardName.succubus, 2);
            cardDiscardDatabase.Add(CardDB.cardName.darkbargain, 2);
            cardDiscardDatabase.Add(CardDB.cardName.darkshirelibrarian, 1);
            cardDiscardDatabase.Add(CardDB.cardName.lakkarifelhound, 4);

            discardedCardCount.Add(CardDB.cardName.doomguard, 2);
            discardedCardCount.Add(CardDB.cardName.soulfire, 1);
            discardedCardCount.Add(CardDB.cardName.succubus, 1);
            discardedCardCount.Add(CardDB.cardName.darkbargain, 2);
            discardedCardCount.Add(CardDB.cardName.darkshirelibrarian, 1);
            discardedCardCount.Add(CardDB.cardName.lakkarifelhound, 2);
        }

        private void setupTransformCards()
        {
            TransformCardDatabase.Add(CardDB.cardName.hex, 0);
            TransformCardDatabase.Add(CardDB.cardName.devolve, 0);
            TransformCardDatabase.Add(CardDB.cardName.polymorph, 0);

            KillCardDatabase.Add(CardDB.cardName.equality, 0);
        }

        private void setupDestroyOwnCards()
        {
            this.destroyOwnDatabase.Add(CardDB.cardName.brawl, 0);
            this.destroyOwnDatabase.Add(CardDB.cardName.deathwing, 0);
            this.destroyOwnDatabase.Add(CardDB.cardName.twistingnether, 0);
            this.destroyOwnDatabase.Add(CardDB.cardName.doom, 0);
            this.destroyOwnDatabase.Add(CardDB.cardName.doomsayer, 0);
            this.destroyOwnDatabase.Add(CardDB.cardName.poisonseeds, 0); //replace all minions with 2/2 treants
            this.destroyOwnDatabase.Add(CardDB.cardName.shadowwordhorror, 0); //all minions with 2 or less ap
            this.destroyOwnDatabase.Add(CardDB.cardName.shadowflame, 0); //destroy own minion, deal its ap to all enemies
            this.destroyOwnDatabase.Add(CardDB.cardName.enterthecoliseum, 0);//all but each player's highest ap

            this.destroyOwnDatabase.Add(CardDB.cardName.naturalize, 0);
            this.destroyOwnDatabase.Add(CardDB.cardName.shadowworddeath, 0);
            this.destroyOwnDatabase.Add(CardDB.cardName.shadowwordpain, 0);
            this.destroyOwnDatabase.Add(CardDB.cardName.siphonsoul, 0);
            this.destroyOwnDatabase.Add(CardDB.cardName.biggamehunter, 0); //a minion ap>=7
            this.destroyOwnDatabase.Add(CardDB.cardName.hungrycrab, 0); //a murloc
            this.destroyOwnDatabase.Add(CardDB.cardName.sacrificialpact, 0);
            this.destroyOwnDatabase.Add(CardDB.cardName.mulch, 0);
            this.destroyOwnDatabase.Add(CardDB.cardName.bladeofcthun, 0); //add stats to c'thun
            this.destroyOwnDatabase.Add(CardDB.cardName.reincarnate, 0); //and resummon
            this.destroyOwnDatabase.Add(CardDB.cardName.voidterror, 0);
            this.destroyOwnDatabase.Add(CardDB.cardName.blastcrystalpotion, 0);
            this.destroyOwnDatabase.Add(CardDB.cardName.crush, 0);
            this.destroyOwnDatabase.Add(CardDB.cardName.hemetnesingwary, 0); //a beast
            this.destroyOwnDatabase.Add(CardDB.cardName.moatlurker, 0); //resummon deathrattle
            this.destroyOwnDatabase.Add(CardDB.cardName.rendblackhand, 0); //a legendary if holding dragon
            this.destroyOwnDatabase.Add(CardDB.cardName.shatter, 0); //frozen
            this.destroyOwnDatabase.Add(CardDB.cardName.execute, 0); //hurt

            this.destroyDatabase.Add(CardDB.cardName.brawl, 0);
            this.destroyDatabase.Add(CardDB.cardName.deathwing, 0);
            this.destroyDatabase.Add(CardDB.cardName.twistingnether, 0);
            this.destroyDatabase.Add(CardDB.cardName.doom, 0);
            this.destroyDatabase.Add(CardDB.cardName.doomsayer, 0);
            this.destroyDatabase.Add(CardDB.cardName.poisonseeds, 0); //replace all minions with 2/2 treants
            this.destroyDatabase.Add(CardDB.cardName.shadowwordhorror, 0); //all minions with 2 or less ap
            this.destroyDatabase.Add(CardDB.cardName.shadowflame, 0); //destroy own minion, deal its ap to all enemies
            this.destroyDatabase.Add(CardDB.cardName.enterthecoliseum, 0);//all but each player's highest ap

            this.destroyDatabase.Add(CardDB.cardName.assassinate, 0); //not own mins
            this.destroyDatabase.Add(CardDB.cardName.corruption, 0); //not own mins
            this.destroyDatabase.Add(CardDB.cardName.execute, 0); //hurt
            this.destroyDatabase.Add(CardDB.cardName.naturalize, 0);
            this.destroyDatabase.Add(CardDB.cardName.siphonsoul, 0);
            this.destroyDatabase.Add(CardDB.cardName.mindcontrol, 0); //not own mins
            this.destroyDatabase.Add(CardDB.cardName.theblackknight, 0); //not own mins
            this.destroyDatabase.Add(CardDB.cardName.sabotage, 0); //not own mins
            this.destroyDatabase.Add(CardDB.cardName.crush, 0);
            this.destroyDatabase.Add(CardDB.cardName.hemetnesingwary, 0); //a beast
            this.destroyDatabase.Add(CardDB.cardName.mulch, 0);
            this.destroyDatabase.Add(CardDB.cardName.bladeofcthun, 0); //add stats to c'thun
            this.destroyDatabase.Add(CardDB.cardName.shatter, 0); //frozen
            this.destroyDatabase.Add(CardDB.cardName.moatlurker, 0); //resummon deathrattle
            this.destroyDatabase.Add(CardDB.cardName.bookwyrm, 0); //enemy ap<=3
            this.destroyDatabase.Add(CardDB.cardName.deadlyshot, 0); //random enemy
            this.destroyDatabase.Add(CardDB.cardName.darkbargain, 0);
            this.destroyDatabase.Add(CardDB.cardName.rendblackhand, 0); //a legendary if holding dragon
            this.destroyDatabase.Add(CardDB.cardName.blastcrystalpotion, 0);

            this.backToHandDatabase.Add(CardDB.cardName.sap, 0);
            this.backToHandDatabase.Add(CardDB.cardName.timerewinder, 0);
            this.backToHandDatabase.Add(CardDB.cardName.ancientbrewmaster, 0);
            this.backToHandDatabase.Add(CardDB.cardName.dream, 0);
            this.backToHandDatabase.Add(CardDB.cardName.shadowstep, 0);
            this.backToHandDatabase.Add(CardDB.cardName.youthfulbrewmaster, 0);
            this.backToHandDatabase.Add(CardDB.cardName.kidnapper, 0);
            this.backToHandDatabase.Add(CardDB.cardName.recycle, 0);
            this.backToHandDatabase.Add(CardDB.cardName.vanish, 0);
            this.backToHandDatabase.Add(CardDB.cardName.bloodthistletoxin, 0);
            this.backToHandDatabase.Add(CardDB.cardName.gadgetzanferryman, 0); //combo
        }

        private void setupSpecialMins()
        {
            //== everything with an effect (other than battlecry and normal stuff like taunt, charge, divshield)
            //also deathrattles?
            this.specialMinions.Add(CardDB.cardName.amaniberserker, 0);
            this.specialMinions.Add(CardDB.cardName.angrychicken, 0);
            this.specialMinions.Add(CardDB.cardName.abomination, 0);
            this.specialMinions.Add(CardDB.cardName.acolyteofpain, 0);
            this.specialMinions.Add(CardDB.cardName.alarmobot, 0);
            this.specialMinions.Add(CardDB.cardName.archmage, 0);
            this.specialMinions.Add(CardDB.cardName.archmageantonidas, 0);
            this.specialMinions.Add(CardDB.cardName.armorsmith, 0);
            this.specialMinions.Add(CardDB.cardName.auchenaisoulpriest, 0);
            this.specialMinions.Add(CardDB.cardName.azuredrake, 0);
            this.specialMinions.Add(CardDB.cardName.barongeddon, 0);
            this.specialMinions.Add(CardDB.cardName.bloodimp, 0);
            this.specialMinions.Add(CardDB.cardName.bloodmagethalnos, 0);
            this.specialMinions.Add(CardDB.cardName.cairnebloodhoof, 0);
            this.specialMinions.Add(CardDB.cardName.cultmaster, 0);
            this.specialMinions.Add(CardDB.cardName.dalaranmage, 0);
            this.specialMinions.Add(CardDB.cardName.demolisher, 0);
            this.specialMinions.Add(CardDB.cardName.direwolfalpha, 0);
            this.specialMinions.Add(CardDB.cardName.doomsayer, 0);
            this.specialMinions.Add(CardDB.cardName.emperorcobra, 0);
            this.specialMinions.Add(CardDB.cardName.etherealarcanist, 0);
            this.specialMinions.Add(CardDB.cardName.flametonguetotem, 0);
            this.specialMinions.Add(CardDB.cardName.flesheatingghoul, 0);
            this.specialMinions.Add(CardDB.cardName.gadgetzanauctioneer, 0);
            this.specialMinions.Add(CardDB.cardName.grimscaleoracle, 0);
            this.specialMinions.Add(CardDB.cardName.grommashhellscream, 0);
            this.specialMinions.Add(CardDB.cardName.gruul, 0);
            this.specialMinions.Add(CardDB.cardName.gurubashiberserker, 0);
            this.specialMinions.Add(CardDB.cardName.harvestgolem, 0);
            this.specialMinions.Add(CardDB.cardName.hogger, 0);
            this.specialMinions.Add(CardDB.cardName.illidanstormrage, 0);
            this.specialMinions.Add(CardDB.cardName.impmaster, 0);
            this.specialMinions.Add(CardDB.cardName.knifejuggler, 0);
            this.specialMinions.Add(CardDB.cardName.koboldgeomancer, 0);
            this.specialMinions.Add(CardDB.cardName.lepergnome, 0);
            this.specialMinions.Add(CardDB.cardName.lightspawn, 0);
            this.specialMinions.Add(CardDB.cardName.lightwarden, 0);
            this.specialMinions.Add(CardDB.cardName.lightwell, 0);
            this.specialMinions.Add(CardDB.cardName.loothoarder, 0);
            this.specialMinions.Add(CardDB.cardName.lorewalkercho, 0);
            this.specialMinions.Add(CardDB.cardName.malygos, 0);
            this.specialMinions.Add(CardDB.cardName.manaaddict, 0);
            this.specialMinions.Add(CardDB.cardName.manatidetotem, 0);
            this.specialMinions.Add(CardDB.cardName.manawraith, 0);
            this.specialMinions.Add(CardDB.cardName.manawyrm, 0);
            this.specialMinions.Add(CardDB.cardName.masterswordsmith, 0);
            this.specialMinions.Add(CardDB.cardName.murloctidecaller, 0);
            this.specialMinions.Add(CardDB.cardName.murlocwarleader, 0);
            this.specialMinions.Add(CardDB.cardName.natpagle, 0);
            this.specialMinions.Add(CardDB.cardName.northshirecleric, 0);
            this.specialMinions.Add(CardDB.cardName.ogremagi, 0);
            this.specialMinions.Add(CardDB.cardName.oldmurkeye, 0);
            this.specialMinions.Add(CardDB.cardName.patientassassin, 0);
            this.specialMinions.Add(CardDB.cardName.pintsizedsummoner, 0);
            this.specialMinions.Add(CardDB.cardName.prophetvelen, 0);
            this.specialMinions.Add(CardDB.cardName.questingadventurer, 0);
            this.specialMinions.Add(CardDB.cardName.ragingworgen, 0);
            this.specialMinions.Add(CardDB.cardName.raidleader, 0);
            this.specialMinions.Add(CardDB.cardName.savannahhighmane, 0);
            this.specialMinions.Add(CardDB.cardName.scavenginghyena, 0);
            this.specialMinions.Add(CardDB.cardName.secretkeeper, 0);
            this.specialMinions.Add(CardDB.cardName.sorcerersapprentice, 0);
            this.specialMinions.Add(CardDB.cardName.southseacaptain, 0);
            this.specialMinions.Add(CardDB.cardName.spitefulsmith, 0);
            this.specialMinions.Add(CardDB.cardName.starvingbuzzard, 0);
            this.specialMinions.Add(CardDB.cardName.stormwindchampion, 0);
            this.specialMinions.Add(CardDB.cardName.summoningportal, 0);
            this.specialMinions.Add(CardDB.cardName.sylvanaswindrunner, 0);
            this.specialMinions.Add(CardDB.cardName.taurenwarrior, 0);
            this.specialMinions.Add(CardDB.cardName.thebeast, 0);
            this.specialMinions.Add(CardDB.cardName.timberwolf, 0);
            this.specialMinions.Add(CardDB.cardName.tirionfordring, 0);
            this.specialMinions.Add(CardDB.cardName.tundrarhino, 0);
            this.specialMinions.Add(CardDB.cardName.unboundelemental, 0);
            //this.specialMinions.Add(CardDB.cardName.venturecomercenary, 0);
            this.specialMinions.Add(CardDB.cardName.violetteacher, 0);
            this.specialMinions.Add(CardDB.cardName.warsongcommander, 0);
            this.specialMinions.Add(CardDB.cardName.waterelemental, 0);

            // naxx cards
            this.specialMinions.Add(CardDB.cardName.baronrivendare, 0);
            this.specialMinions.Add(CardDB.cardName.undertaker, 0);
            this.specialMinions.Add(CardDB.cardName.dancingswords, 0);
            this.specialMinions.Add(CardDB.cardName.darkcultist, 0);
            this.specialMinions.Add(CardDB.cardName.deathlord, 0);
            this.specialMinions.Add(CardDB.cardName.feugen, 0);
            this.specialMinions.Add(CardDB.cardName.stalagg, 0);
            this.specialMinions.Add(CardDB.cardName.hauntedcreeper, 0);
            this.specialMinions.Add(CardDB.cardName.kelthuzad, 0);
            this.specialMinions.Add(CardDB.cardName.madscientist, 0);
            this.specialMinions.Add(CardDB.cardName.maexxna, 0);
            this.specialMinions.Add(CardDB.cardName.nerubarweblord, 0);
            this.specialMinions.Add(CardDB.cardName.shadeofnaxxramas, 0);
            this.specialMinions.Add(CardDB.cardName.unstableghoul, 0);
            this.specialMinions.Add(CardDB.cardName.voidcaller, 0);
            this.specialMinions.Add(CardDB.cardName.anubarambusher, 0);
            this.specialMinions.Add(CardDB.cardName.webspinner, 0);

            this.specialMinions.Add(CardDB.cardName.emperorthaurissan, 0);
            this.specialMinions.Add(CardDB.cardName.majordomoexecutus, 0);
            this.specialMinions.Add(CardDB.cardName.chromaggus, 0);
            this.specialMinions.Add(CardDB.cardName.flamewaker, 0);
            this.specialMinions.Add(CardDB.cardName.impgangboss, 0);
            this.specialMinions.Add(CardDB.cardName.axeflinger, 0);
            this.specialMinions.Add(CardDB.cardName.grimpatron, 0);
            this.specialMinions.Add(CardDB.cardName.dragonkinsorcerer, 0);
            this.specialMinions.Add(CardDB.cardName.dragonegg, 0);

            //GVG
            this.specialMinions.Add(CardDB.cardName.snowchugger, 0);
            this.specialMinions.Add(CardDB.cardName.mechwarper, 0);
            this.specialMinions.Add(CardDB.cardName.cogmaster, 0);
            this.specialMinions.Add(CardDB.cardName.mistressofpain, 0);
            this.specialMinions.Add(CardDB.cardName.felcannon, 0);
            this.specialMinions.Add(CardDB.cardName.malganis, 0);
            this.specialMinions.Add(CardDB.cardName.ironsensei, 0);
            this.specialMinions.Add(CardDB.cardName.tradeprincegallywix, 0);
            this.specialMinions.Add(CardDB.cardName.mechbearcat, 0);
            this.specialMinions.Add(CardDB.cardName.vitalitytotem, 0);
            this.specialMinions.Add(CardDB.cardName.siltfinspiritwalker, 0);
            this.specialMinions.Add(CardDB.cardName.gahzrilla, 0);
            this.specialMinions.Add(CardDB.cardName.warbot, 0);
            this.specialMinions.Add(CardDB.cardName.cobaltguardian, 0);
            this.specialMinions.Add(CardDB.cardName.stonesplintertrogg, 0);
            this.specialMinions.Add(CardDB.cardName.burlyrockjawtrogg, 0);
            this.specialMinions.Add(CardDB.cardName.shadowboxer, 0);
            this.specialMinions.Add(CardDB.cardName.shipscannon, 0);
            this.specialMinions.Add(CardDB.cardName.steamwheedlesniper, 0);
            this.specialMinions.Add(CardDB.cardName.jeeves, 0);
            this.specialMinions.Add(CardDB.cardName.goblinsapper, 0);
            this.specialMinions.Add(CardDB.cardName.floatingwatcher, 0);
            this.specialMinions.Add(CardDB.cardName.micromachine, 0);
            this.specialMinions.Add(CardDB.cardName.hobgoblin, 0);
            this.specialMinions.Add(CardDB.cardName.junkbot, 0);
            this.specialMinions.Add(CardDB.cardName.mimironshead, 0);
            this.specialMinions.Add(CardDB.cardName.mogortheogre, 0);
            this.specialMinions.Add(CardDB.cardName.foereaper4000, 0);
            this.specialMinions.Add(CardDB.cardName.mekgineerthermaplugg, 0);
            this.specialMinions.Add(CardDB.cardName.gazlowe, 0);
            this.specialMinions.Add(CardDB.cardName.troggzortheearthinator, 0);
            this.specialMinions.Add(CardDB.cardName.clockworkgnome, 0);
            this.specialMinions.Add(CardDB.cardName.explosivesheep, 0);
            this.specialMinions.Add(CardDB.cardName.mechanicalyeti, 0);
            this.specialMinions.Add(CardDB.cardName.pilotedshredder, 0);
            this.specialMinions.Add(CardDB.cardName.pilotedskygolem, 0);
            this.specialMinions.Add(CardDB.cardName.malorne, 0);
            this.specialMinions.Add(CardDB.cardName.sneedsoldshredder, 0);
            this.specialMinions.Add(CardDB.cardName.toshley, 0);

            //TGT
            this.specialMinions.Add(CardDB.cardName.lowlysquire, 0);
            this.specialMinions.Add(CardDB.cardName.boneguardlieutenant, 0);
            this.specialMinions.Add(CardDB.cardName.dragonhawkrider, 0);
            this.specialMinions.Add(CardDB.cardName.silverhandregent, 0);
            this.specialMinions.Add(CardDB.cardName.maidenofthelake, 0);
            this.specialMinions.Add(CardDB.cardName.tournamentmedic, 0);
            this.specialMinions.Add(CardDB.cardName.kvaldirraider, 0);
            this.specialMinions.Add(CardDB.cardName.muklaschampion, 0);
            this.specialMinions.Add(CardDB.cardName.garrisoncommander, 0);
            this.specialMinions.Add(CardDB.cardName.crowdfavorite, 0);
            this.specialMinions.Add(CardDB.cardName.recruiter, 0);
            this.specialMinions.Add(CardDB.cardName.kodorider, 0);
            this.specialMinions.Add(CardDB.cardName.eydisdarkbane, 0);
            this.specialMinions.Add(CardDB.cardName.fjolalightbane, 0);
            this.specialMinions.Add(CardDB.cardName.nexuschampionsaraad, 0);
            this.specialMinions.Add(CardDB.cardName.bolframshield, 0);

            this.specialMinions.Add(CardDB.cardName.savagecombatant, 0);
            this.specialMinions.Add(CardDB.cardName.knightofthewild, 0);
            this.specialMinions.Add(CardDB.cardName.aviana, 0);
            this.specialMinions.Add(CardDB.cardName.bravearcher, 0);
            this.specialMinions.Add(CardDB.cardName.dreadscale, 0);
            this.specialMinions.Add(CardDB.cardName.acidmaw, 0);
            this.specialMinions.Add(CardDB.cardName.dalaranaspirant, 0);
            this.specialMinions.Add(CardDB.cardName.fallenhero, 0);
            this.specialMinions.Add(CardDB.cardName.coldarradrake, 0);
            this.specialMinions.Add(CardDB.cardName.warhorsetrainer, 0);
            this.specialMinions.Add(CardDB.cardName.murlocknight, 0);
            this.specialMinions.Add(CardDB.cardName.holychampion, 0);
            this.specialMinions.Add(CardDB.cardName.spawnofshadows, 0);
            this.specialMinions.Add(CardDB.cardName.shadowfiend, 0);
            this.specialMinions.Add(CardDB.cardName.confessorpaletress, 0);
            this.specialMinions.Add(CardDB.cardName.buccaneer, 0);
            this.specialMinions.Add(CardDB.cardName.cutpurse, 0);
            this.specialMinions.Add(CardDB.cardName.thunderbluffvaliant, 0);
            this.specialMinions.Add(CardDB.cardName.wrathguard, 0);
            this.specialMinions.Add(CardDB.cardName.tinyknightofevil, 0);
            this.specialMinions.Add(CardDB.cardName.voidcrusher, 0);
            this.specialMinions.Add(CardDB.cardName.wilfredfizzlebang, 0);
            this.specialMinions.Add(CardDB.cardName.orgrimmaraspirant, 0);
            this.specialMinions.Add(CardDB.cardName.magnatauralpha, 0);

            //LOE (week 1 and 2 :D)
            this.specialMinions.Add(CardDB.cardName.obsidiandestroyer, 0);
            this.specialMinions.Add(CardDB.cardName.djinniofzephyrs, 0);
            this.specialMinions.Add(CardDB.cardName.summoningstone, 0);
            this.specialMinions.Add(CardDB.cardName.rumblingelemental, 0);
            this.specialMinions.Add(CardDB.cardName.tunneltrogg, 0);
            this.specialMinions.Add(CardDB.cardName.brannbronzebeard, 0);

            //OG
            specialMinions.Add(CardDB.cardName.aberrantberserker, 0);
            specialMinions.Add(CardDB.cardName.addledgrizzly, 0);
            specialMinions.Add(CardDB.cardName.ancientharbinger, 0);
            specialMinions.Add(CardDB.cardName.blackwaterpirate, 0);
            specialMinions.Add(CardDB.cardName.bloodhoofbrave, 0);
            specialMinions.Add(CardDB.cardName.crazedworshipper, 0);
            specialMinions.Add(CardDB.cardName.cthun, 0);
            specialMinions.Add(CardDB.cardName.cultsorcerer, 0);
            specialMinions.Add(CardDB.cardName.darkshirecouncilman, 0);
            specialMinions.Add(CardDB.cardName.dementedfrostcaller, 0);
            specialMinions.Add(CardDB.cardName.evolvedkobold, 0);
            specialMinions.Add(CardDB.cardName.fandralstaghelm, 0);
            specialMinions.Add(CardDB.cardName.giantsandworm, 0);
            specialMinions.Add(CardDB.cardName.hallazealtheascended, 0);
            specialMinions.Add(CardDB.cardName.hoggerdoomofelwynn, 0);
            specialMinions.Add(CardDB.cardName.hoodedacolyte, 0);
            specialMinions.Add(CardDB.cardName.infestedtauren, 0);
            specialMinions.Add(CardDB.cardName.infestedwolf, 0);
            specialMinions.Add(CardDB.cardName.ragnaroslightlord, 0);
            specialMinions.Add(CardDB.cardName.scalednightmare, 0);
            specialMinions.Add(CardDB.cardName.shiftingshade, 0);
            specialMinions.Add(CardDB.cardName.southseasquidface, 0);
            specialMinions.Add(CardDB.cardName.spawnofnzoth, 0);
            specialMinions.Add(CardDB.cardName.stewardofdarkshire, 0);
            specialMinions.Add(CardDB.cardName.theboogeymonster, 0);
            specialMinions.Add(CardDB.cardName.twilightelder, 0);
            specialMinions.Add(CardDB.cardName.undercityhuckster, 0);
            specialMinions.Add(CardDB.cardName.usherofsouls, 0);
            specialMinions.Add(CardDB.cardName.wobblingrunts, 0);
            specialMinions.Add(CardDB.cardName.xarilpoisonedmind, 0);
            specialMinions.Add(CardDB.cardName.ysera, 0);
            specialMinions.Add(CardDB.cardName.yshaarjrageunbound, 0);
            specialMinions.Add(CardDB.cardName.zealousinitiate, 0);

            //Kara
            specialMinions.Add(CardDB.cardName.arcaneanomaly, 0);
            specialMinions.Add(CardDB.cardName.cloakedhuntress, 0);
            specialMinions.Add(CardDB.cardName.deadlyfork, 0);
            specialMinions.Add(CardDB.cardName.moroes, 0);
            specialMinions.Add(CardDB.cardName.priestofthefeast, 0);
            this.specialMinions.Add(CardDB.cardName.possessedvillager, 0);
            specialMinions.Add(CardDB.cardName.kindlygrandmother, 0);
            specialMinions.Add(CardDB.cardName.wickedwitchdoctor, 0);
            specialMinions.Add(CardDB.cardName.moatlurker, 0);
            specialMinions.Add(CardDB.cardName.hugetoad, 0);
            specialMinions.Add(CardDB.cardName.fierybat, 0);
            specialMinions.Add(CardDB.cardName.pollutedhoarder, 0);
            //specialMinions.Add(CardDB.cardName.darkshirelibrarian, 0);

            //MSG
            specialMinions.Add(CardDB.cardName.ratpack, 0);
            specialMinions.Add(CardDB.cardName.shakyzipgunner, 0);
            specialMinions.Add(CardDB.cardName.knuckles, 0);
            specialMinions.Add(CardDB.cardName.meanstreetmarshal, 0);
            specialMinions.Add(CardDB.cardName.wickerflameburnbristle, 0);
            specialMinions.Add(CardDB.cardName.grimestreetenforcer, 0);
            specialMinions.Add(CardDB.cardName.manageode, 0);
            specialMinions.Add(CardDB.cardName.jadeswarmer, 0);
            specialMinions.Add(CardDB.cardName.shakuthecollector, 0);
            specialMinions.Add(CardDB.cardName.lotusassassin, 0);
            specialMinions.Add(CardDB.cardName.lotusillusionist, 0);
            specialMinions.Add(CardDB.cardName.whiteeyes, 0);
            specialMinions.Add(CardDB.cardName.unlicensedapothecary, 0);
            specialMinions.Add(CardDB.cardName.kabaltrafficker, 0);
            specialMinions.Add(CardDB.cardName.grimygadgeteer, 0);
            specialMinions.Add(CardDB.cardName.alleyarmorsmith, 0);
            specialMinions.Add(CardDB.cardName.mistressofmixtures, 0);
            specialMinions.Add(CardDB.cardName.smalltimebuccaneer, 0);
            specialMinions.Add(CardDB.cardName.weaseltunneler, 0);
            specialMinions.Add(CardDB.cardName.friendlybartender, 0);
            specialMinions.Add(CardDB.cardName.auctionmasterbeardo, 0);
            specialMinions.Add(CardDB.cardName.backstreetleper, 0);
            specialMinions.Add(CardDB.cardName.felorcsoulfiend, 0);
            specialMinions.Add(CardDB.cardName.sergeantsally, 0);
            specialMinions.Add(CardDB.cardName.backroombouncer, 0);
            specialMinions.Add(CardDB.cardName.daringreporter, 0);
            specialMinions.Add(CardDB.cardName.genzotheshark, 0);
            specialMinions.Add(CardDB.cardName.bombsquad, 0);
            specialMinions.Add(CardDB.cardName.burglybully, 0);
            specialMinions.Add(CardDB.cardName.finjatheflyingstar, 0);
            specialMinions.Add(CardDB.cardName.redmanawyrm, 0);
            specialMinions.Add(CardDB.cardName.windupburglebot, 0);
            specialMinions.Add(CardDB.cardName.mayornoggenfogger, 0);

            //ungoro
            specialMinions.Add(CardDB.cardName.radiantelemental, 0);
            specialMinions.Add(CardDB.cardName.swampkingdred, 0);
            specialMinions.Add(CardDB.cardName.primalfintotem, 0);
            specialMinions.Add(CardDB.cardName.lyrathesunshard, 0);
            specialMinions.Add(CardDB.cardName.viciousfledgling, 0);
        }

        public void setupBuffingMinions()
        {
            buffingMinionsDatabase.Add(CardDB.cardName.abusivesergeant, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.captaingreenskin, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.cenarius, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.coldlightseer, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.crueltaskmaster, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.darkirondwarf, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.defenderofargus, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.direwolfalpha, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.flametonguetotem, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.grimscaleoracle, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.houndmaster, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.leokk, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.murlocwarleader, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.raidleader, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.shatteredsuncleric, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.southseacaptain, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.spitefulsmith, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.stormwindchampion, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.templeenforcer, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.timberwolf, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.malganis, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.warsongcommander, 0);

            buffingMinionsDatabase.Add(CardDB.cardName.metaltoothleaper, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.quartermaster, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.screwjankclunker, 0);

            buffingMinionsDatabase.Add(CardDB.cardName.lancecarrier, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.clockworkknight, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.wildwalker, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.warhorsetrainer, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.thunderbluffvaliant, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.bloodsailcultist, 5);

            buffingMinionsDatabase.Add(CardDB.cardName.virmensensei, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.grimestreetprotector, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.kabaltalonpriest, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.shadowsensei, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.crystalweaver, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.hobartgrapplehammer, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.grimestreetpawnbroker, 0);
            buffingMinionsDatabase.Add(CardDB.cardName.nagacorsair, 0);

            buffing1TurnDatabase.Add(CardDB.cardName.abusivesergeant, 0);
            buffing1TurnDatabase.Add(CardDB.cardName.darkirondwarf, 0);
            buffing1TurnDatabase.Add(CardDB.cardName.rockbiterweapon, 0);
            buffing1TurnDatabase.Add(CardDB.cardName.poweroverwhelming, 0);
            buffing1TurnDatabase.Add(CardDB.cardName.bloodlust, 0);
        }

        private void setupEnemyTargetPriority()
        {
            priorityTargets.Add(CardDB.cardName.lightwarden, 4);
            priorityTargets.Add(CardDB.cardName.secretkeeper, 5);
            priorityTargets.Add(CardDB.cardName.youngdragonhawk, 4);
            priorityTargets.Add(CardDB.cardName.bloodmagethalnos, 3);
            priorityTargets.Add(CardDB.cardName.direwolfalpha, 4);
            priorityTargets.Add(CardDB.cardName.doomsayer, 10);
            priorityTargets.Add(CardDB.cardName.knifejuggler, 8);
            priorityTargets.Add(CardDB.cardName.koboldgeomancer, 3);
            priorityTargets.Add(CardDB.cardName.manaaddict, 4);
            priorityTargets.Add(CardDB.cardName.masterswordsmith, 2);
            priorityTargets.Add(CardDB.cardName.natpagle, 10);
            priorityTargets.Add(CardDB.cardName.murloctidehunter, 4);
            priorityTargets.Add(CardDB.cardName.pintsizedsummoner, 10);
            priorityTargets.Add(CardDB.cardName.wildpyromancer, 4);
            priorityTargets.Add(CardDB.cardName.alarmobot, 10);
            priorityTargets.Add(CardDB.cardName.acolyteofpain, 4);
            priorityTargets.Add(CardDB.cardName.demolisher, 4);
            priorityTargets.Add(CardDB.cardName.flesheatingghoul, 4);
            priorityTargets.Add(CardDB.cardName.impmaster, 3);
            priorityTargets.Add(CardDB.cardName.questingadventurer, 16);
            priorityTargets.Add(CardDB.cardName.raidleader, 6);
            priorityTargets.Add(CardDB.cardName.thrallmarfarseer, 10);
            priorityTargets.Add(CardDB.cardName.cultmaster, 10);
            priorityTargets.Add(CardDB.cardName.violetteacher, 10);
            priorityTargets.Add(CardDB.cardName.gadgetzanauctioneer, 18);
            priorityTargets.Add(CardDB.cardName.hogger, 10);
            priorityTargets.Add(CardDB.cardName.illidanstormrage, 10);
            priorityTargets.Add(CardDB.cardName.barongeddon, 10);
            priorityTargets.Add(CardDB.cardName.stormwindchampion, 10);
            priorityTargets.Add(CardDB.cardName.gurubashiberserker, 10);
            priorityTargets.Add(CardDB.cardName.ysera, 10);
            priorityTargets.Add(CardDB.cardName.ragnarosthefirelord, 10);
            priorityTargets.Add(CardDB.cardName.ragingworgen, 5);
            priorityTargets.Add(CardDB.cardName.southseacaptain, 8);

            //warrior cards
            priorityTargets.Add(CardDB.cardName.frothingberserker, 10);
            priorityTargets.Add(CardDB.cardName.warsongcommander, 10);
            priorityTargets.Add(CardDB.cardName.armorsmith, 4);

            //warlock cards
            priorityTargets.Add(CardDB.cardName.summoningportal, 10);

            //shaman cards
            priorityTargets.Add(CardDB.cardName.dustdevil, 10);
            //priorityTargets.Add(CardDB.cardName.wrathofairtotem, 2);
            priorityTargets.Add(CardDB.cardName.flametonguetotem, 4);
            priorityTargets.Add(CardDB.cardName.manatidetotem, 7);
            priorityTargets.Add(CardDB.cardName.unboundelemental, 5);
            priorityTargets.Add(CardDB.cardName.thunderbluffvaliant, 14);

            //rogue cards

            //priest cards
            priorityTargets.Add(CardDB.cardName.northshirecleric, 8);
            priorityTargets.Add(CardDB.cardName.lightwell, 10);
            priorityTargets.Add(CardDB.cardName.auchenaisoulpriest, 12);
            priorityTargets.Add(CardDB.cardName.prophetvelen, 10);

            //paladin cards

            //mage cards
            priorityTargets.Add(CardDB.cardName.manawyrm, 6);
            priorityTargets.Add(CardDB.cardName.sorcerersapprentice, 10);
            priorityTargets.Add(CardDB.cardName.etherealarcanist, 6);
            priorityTargets.Add(CardDB.cardName.archmageantonidas, 10);

            //hunter cards
            priorityTargets.Add(CardDB.cardName.timberwolf, 4);
            priorityTargets.Add(CardDB.cardName.scavenginghyena, 10);
            priorityTargets.Add(CardDB.cardName.starvingbuzzard, 10);
            priorityTargets.Add(CardDB.cardName.leokk, 10);
            priorityTargets.Add(CardDB.cardName.tundrarhino, 15);

            //naxx cards
            priorityTargets.Add(CardDB.cardName.baronrivendare, 5);
            priorityTargets.Add(CardDB.cardName.kelthuzad, 10);
            priorityTargets.Add(CardDB.cardName.nerubarweblord, 10);
            priorityTargets.Add(CardDB.cardName.shadeofnaxxramas, 10);
            priorityTargets.Add(CardDB.cardName.undertaker, 4);

            //GVG
            this.priorityTargets.Add(CardDB.cardName.ironsensei, 6);
            this.priorityTargets.Add(CardDB.cardName.mechwarper, 4);
            this.priorityTargets.Add(CardDB.cardName.malganis, 10);
            this.priorityTargets.Add(CardDB.cardName.vitalitytotem, 4);
            this.priorityTargets.Add(CardDB.cardName.gahzrilla, 10);
            this.priorityTargets.Add(CardDB.cardName.steamwheedlesniper, 5);
            this.priorityTargets.Add(CardDB.cardName.floatingwatcher, 10);
            this.priorityTargets.Add(CardDB.cardName.micromachine, 4);
            this.priorityTargets.Add(CardDB.cardName.hobgoblin, 5);
            this.priorityTargets.Add(CardDB.cardName.mogortheogre, 10);
            this.priorityTargets.Add(CardDB.cardName.foereaper4000, 10);
            this.priorityTargets.Add(CardDB.cardName.troggzortheearthinator, 10);

            //BRM
            this.priorityTargets.Add(CardDB.cardName.flamewaker, 10);
            this.priorityTargets.Add(CardDB.cardName.impgangboss, 5);
            this.priorityTargets.Add(CardDB.cardName.grimpatron, 10);
            this.priorityTargets.Add(CardDB.cardName.dragonkinsorcerer, 4);
            this.priorityTargets.Add(CardDB.cardName.emperorthaurissan, 10);
            this.priorityTargets.Add(CardDB.cardName.chromaggus, 10);
            this.priorityTargets.Add(CardDB.cardName.tournamentmedic, 4);

            //TGT
            this.priorityTargets.Add(CardDB.cardName.muklaschampion, 10);
            this.priorityTargets.Add(CardDB.cardName.kodorider, 5);
            this.priorityTargets.Add(CardDB.cardName.eydisdarkbane, 5);
            this.priorityTargets.Add(CardDB.cardName.nexuschampionsaraad, 10);
            this.priorityTargets.Add(CardDB.cardName.savagecombatant, 8);
            this.priorityTargets.Add(CardDB.cardName.aviana, 17);
            this.priorityTargets.Add(CardDB.cardName.acidmaw, 5);

            this.priorityTargets.Add(CardDB.cardName.coldarradrake, 5);
            this.priorityTargets.Add(CardDB.cardName.warhorsetrainer, 5);
            this.priorityTargets.Add(CardDB.cardName.murlocknight, 10);
            this.priorityTargets.Add(CardDB.cardName.holychampion, 5);
            this.priorityTargets.Add(CardDB.cardName.wilfredfizzlebang, 10);

            //LOE

            this.priorityTargets.Add(CardDB.cardName.brannbronzebeard, 10);
            this.priorityTargets.Add(CardDB.cardName.obsidiandestroyer, 10);
            this.priorityTargets.Add(CardDB.cardName.summoningstone, 20);
            this.priorityTargets.Add(CardDB.cardName.djinniofzephyrs, 5);
            this.priorityTargets.Add(CardDB.cardName.rumblingelemental, 5);
            this.priorityTargets.Add(CardDB.cardName.animatedarmor, 5);
            priorityTargets.Add(CardDB.cardName.tunneltrogg, 5);

            priorityTargets.Add(CardDB.cardName.addledgrizzly, 10);
            priorityTargets.Add(CardDB.cardName.cthun, 10);
            priorityTargets.Add(CardDB.cardName.cultsorcerer, 3);
            priorityTargets.Add(CardDB.cardName.darkshirecouncilman, 10);
            priorityTargets.Add(CardDB.cardName.dementedfrostcaller, 10);
            priorityTargets.Add(CardDB.cardName.fandralstaghelm, 15);
            priorityTargets.Add(CardDB.cardName.giantsandworm, 10);
            priorityTargets.Add(CardDB.cardName.hoggerdoomofelwynn, 10);
            priorityTargets.Add(CardDB.cardName.hoodedacolyte, 10);
            priorityTargets.Add(CardDB.cardName.ragnaroslightlord, 10);
            priorityTargets.Add(CardDB.cardName.scalednightmare, 10);
            priorityTargets.Add(CardDB.cardName.theboogeymonster, 10);
            priorityTargets.Add(CardDB.cardName.twilightsummoner, 10);
            priorityTargets.Add(CardDB.cardName.yshaarjrageunbound, 10);
            priorityTargets.Add(CardDB.cardName.stewardofdarkshire, 10);
            priorityTargets.Add(CardDB.cardName.hallazealtheascended, 10);

            priorityTargets.Add(CardDB.cardName.cloakedhuntress, 10);
            priorityTargets.Add(CardDB.cardName.moroes, 5);
            priorityTargets.Add(CardDB.cardName.priestofthefeast, 4);
            priorityTargets.Add(CardDB.cardName.wickedwitchdoctor, 8);
            priorityTargets.Add(CardDB.cardName.malchezaarsimp, 3);
            priorityTargets.Add(CardDB.cardName.violetillusionist, 6);

            //killing target
            priorityTargets.Add(CardDB.cardName.pitsnake, 5);
            priorityTargets.Add(CardDB.cardName.maexxna, 5);
            priorityTargets.Add(CardDB.cardName.patientassassin, 5);
            priorityTargets.Add(CardDB.cardName.emperorcobra, 5);

            //MSG
            priorityTargets.Add(CardDB.cardName.grimestreetenforcer, 10);
            priorityTargets.Add(CardDB.cardName.shakuthecollector, 5);
            priorityTargets.Add(CardDB.cardName.kabaltrafficker, 5);
            priorityTargets.Add(CardDB.cardName.grimygadgeteer, 5);
            priorityTargets.Add(CardDB.cardName.friendlybartender, 1);
            priorityTargets.Add(CardDB.cardName.auctionmasterbeardo, 5);
            priorityTargets.Add(CardDB.cardName.backroombouncer, 5);
            priorityTargets.Add(CardDB.cardName.daringreporter, 1);
            priorityTargets.Add(CardDB.cardName.burglybully, 3);
            priorityTargets.Add(CardDB.cardName.redmanawyrm, 5);

            priorityTargets.Add(CardDB.cardName.selflesshero, 4);
            priorityTargets.Add(CardDB.cardName.grimscaleoracle, 2);

            priorityTargets.Add(CardDB.cardName.finjatheflyingstar, 11);

            //ungoro
            priorityTargets.Add(CardDB.cardName.radiantelemental, 6);
            priorityTargets.Add(CardDB.cardName.primalfintotem, 8);
            priorityTargets.Add(CardDB.cardName.lyrathesunshard, 15);
            priorityTargets.Add(CardDB.cardName.viciousfledgling, 10);
        }

        private void setupLethalHelpMinions()
        {
            lethalHelpers.Add(CardDB.cardName.auchenaisoulpriest, 0);
            //spellpower minions
            lethalHelpers.Add(CardDB.cardName.archmage, 0);
            lethalHelpers.Add(CardDB.cardName.dalaranmage, 0);
            lethalHelpers.Add(CardDB.cardName.koboldgeomancer, 0);
            lethalHelpers.Add(CardDB.cardName.ogremagi, 0);
            lethalHelpers.Add(CardDB.cardName.ancientmage, 0);
            lethalHelpers.Add(CardDB.cardName.azuredrake, 0);
            lethalHelpers.Add(CardDB.cardName.bloodmagethalnos, 0);
            lethalHelpers.Add(CardDB.cardName.malygos, 0);
            lethalHelpers.Add(CardDB.cardName.sootspewer, 0);
            lethalHelpers.Add(CardDB.cardName.minimage, 0);

            lethalHelpers.Add(CardDB.cardName.varianwrynn, 0);
            lethalHelpers.Add(CardDB.cardName.bravearcher, 0);
            lethalHelpers.Add(CardDB.cardName.acidmaw, 0);
            lethalHelpers.Add(CardDB.cardName.coldarradrake, 0);
            lethalHelpers.Add(CardDB.cardName.polymorphboar, 0);
            //
            lethalHelpers.Add(CardDB.cardName.cultsorcerer, 0);
            lethalHelpers.Add(CardDB.cardName.evolvedkobold, 0);
        }

        private void setupSilenceTargets()
        {
            this.silenceTargets.Add(CardDB.cardName.abomination, 0);
            this.silenceTargets.Add(CardDB.cardName.acolyteofpain, 0);
            this.silenceTargets.Add(CardDB.cardName.archmageantonidas, 0);
            this.silenceTargets.Add(CardDB.cardName.armorsmith, 0);
            this.silenceTargets.Add(CardDB.cardName.auchenaisoulpriest, 0);
            this.silenceTargets.Add(CardDB.cardName.barongeddon, 0);
            //this.silenceTargets.Add(CardDB.cardName.bloodimp, 0);
            this.silenceTargets.Add(CardDB.cardName.cairnebloodhoof, 0);
            this.silenceTargets.Add(CardDB.cardName.cultmaster, 0);
            this.silenceTargets.Add(CardDB.cardName.direwolfalpha, 0);
            this.silenceTargets.Add(CardDB.cardName.doomsayer, 0);
            this.silenceTargets.Add(CardDB.cardName.emperorcobra, 0);
            this.silenceTargets.Add(CardDB.cardName.etherealarcanist, 0);
            this.silenceTargets.Add(CardDB.cardName.flametonguetotem, 0);
            this.silenceTargets.Add(CardDB.cardName.gadgetzanauctioneer, 10);
            this.silenceTargets.Add(CardDB.cardName.grommashhellscream, 0);
            this.silenceTargets.Add(CardDB.cardName.gruul, 0);
            this.silenceTargets.Add(CardDB.cardName.gurubashiberserker, 0);
            this.silenceTargets.Add(CardDB.cardName.hogger, 0);
            this.silenceTargets.Add(CardDB.cardName.illidanstormrage, 0);
            this.silenceTargets.Add(CardDB.cardName.impmaster, 0);
            this.silenceTargets.Add(CardDB.cardName.knifejuggler, 0);
            this.silenceTargets.Add(CardDB.cardName.lightspawn, 0);
            this.silenceTargets.Add(CardDB.cardName.lightwarden, 0);
            this.silenceTargets.Add(CardDB.cardName.lightwell, 0);
            this.silenceTargets.Add(CardDB.cardName.lorewalkercho, 0);
            this.silenceTargets.Add(CardDB.cardName.malygos, 0);
            this.silenceTargets.Add(CardDB.cardName.manatidetotem, 0);
            this.silenceTargets.Add(CardDB.cardName.manawraith, 0);
            this.silenceTargets.Add(CardDB.cardName.manawyrm, 0);
            this.silenceTargets.Add(CardDB.cardName.masterswordsmith, 0);
            this.silenceTargets.Add(CardDB.cardName.murloctidecaller, 0);
            this.silenceTargets.Add(CardDB.cardName.murlocwarleader, 0);
            this.silenceTargets.Add(CardDB.cardName.natpagle, 0);
            this.silenceTargets.Add(CardDB.cardName.northshirecleric, 0);
            this.silenceTargets.Add(CardDB.cardName.oldmurkeye, 0);
            this.silenceTargets.Add(CardDB.cardName.prophetvelen, 0);
            this.silenceTargets.Add(CardDB.cardName.questingadventurer, 0);
            this.silenceTargets.Add(CardDB.cardName.raidleader, 0);
            this.silenceTargets.Add(CardDB.cardName.savannahhighmane, 0);
            this.silenceTargets.Add(CardDB.cardName.scavenginghyena, 0);
            this.silenceTargets.Add(CardDB.cardName.sorcerersapprentice, 0);
            this.silenceTargets.Add(CardDB.cardName.southseacaptain, 0);
            this.silenceTargets.Add(CardDB.cardName.spitefulsmith, 0);
            this.silenceTargets.Add(CardDB.cardName.starvingbuzzard, 0);
            this.silenceTargets.Add(CardDB.cardName.stormwindchampion, 0);
            this.silenceTargets.Add(CardDB.cardName.summoningportal, 0);
            this.silenceTargets.Add(CardDB.cardName.sylvanaswindrunner, 0);
            this.silenceTargets.Add(CardDB.cardName.timberwolf, 0);
            this.silenceTargets.Add(CardDB.cardName.tirionfordring, 0);
            this.silenceTargets.Add(CardDB.cardName.tundrarhino, 0);
            //this.specialMinions.Add(CardDB.cardName.unboundelemental, 0);
            //this.specialMinions.Add(CardDB.cardName.venturecomercenary, 0);
            this.silenceTargets.Add(CardDB.cardName.violetteacher, 0);
            this.silenceTargets.Add(CardDB.cardName.warsongcommander, 0);
            //this.specialMinions.Add(CardDB.cardName.waterelemental, 0);

            // naxx cards
            this.silenceTargets.Add(CardDB.cardName.baronrivendare, 0);
            this.silenceTargets.Add(CardDB.cardName.undertaker, 0);
            this.silenceTargets.Add(CardDB.cardName.darkcultist, 0);
            this.silenceTargets.Add(CardDB.cardName.feugen, 0);
            this.silenceTargets.Add(CardDB.cardName.stalagg, 0);
            this.silenceTargets.Add(CardDB.cardName.hauntedcreeper, 0);
            this.silenceTargets.Add(CardDB.cardName.kelthuzad, 10);
            this.silenceTargets.Add(CardDB.cardName.madscientist, 0);
            this.silenceTargets.Add(CardDB.cardName.maexxna, 0);
            this.silenceTargets.Add(CardDB.cardName.nerubarweblord, 0);
            this.silenceTargets.Add(CardDB.cardName.shadeofnaxxramas, 0);

            this.silenceTargets.Add(CardDB.cardName.webspinner, 0);
            this.silenceTargets.Add(CardDB.cardName.ironsensei, 0);
            this.silenceTargets.Add(CardDB.cardName.vitalitytotem, 0);

            this.silenceTargets.Add(CardDB.cardName.malganis, 0);
            this.silenceTargets.Add(CardDB.cardName.malorne, 0);
            this.silenceTargets.Add(CardDB.cardName.gahzrilla, 0);
            this.silenceTargets.Add(CardDB.cardName.bolvarfordragon, 0);
            this.silenceTargets.Add(CardDB.cardName.mogortheogre, 0);
            this.silenceTargets.Add(CardDB.cardName.stonesplintertrogg, 0);
            this.silenceTargets.Add(CardDB.cardName.burlyrockjawtrogg, 0);
            this.silenceTargets.Add(CardDB.cardName.shadowboxer, 0);
            this.silenceTargets.Add(CardDB.cardName.explosivesheep, 0);
            this.silenceTargets.Add(CardDB.cardName.animagolem, 0);
            this.silenceTargets.Add(CardDB.cardName.siegeengine, 0);
            this.silenceTargets.Add(CardDB.cardName.steamwheedlesniper, 0);
            this.silenceTargets.Add(CardDB.cardName.floatingwatcher, 0);
            this.silenceTargets.Add(CardDB.cardName.micromachine, 0);
            this.silenceTargets.Add(CardDB.cardName.hobgoblin, 0);
            this.silenceTargets.Add(CardDB.cardName.pilotedskygolem, 0);
            this.silenceTargets.Add(CardDB.cardName.junkbot, 0);
            this.silenceTargets.Add(CardDB.cardName.v07tr0n, 0);
            this.silenceTargets.Add(CardDB.cardName.foereaper4000, 0);
            this.silenceTargets.Add(CardDB.cardName.sneedsoldshredder, 0);
            this.silenceTargets.Add(CardDB.cardName.mekgineerthermaplugg, 0);
            this.silenceTargets.Add(CardDB.cardName.troggzortheearthinator, 0);

            this.silenceTargets.Add(CardDB.cardName.flamewaker, 0);
            this.silenceTargets.Add(CardDB.cardName.impgangboss, 0);
            this.silenceTargets.Add(CardDB.cardName.grimpatron, 0);
            this.silenceTargets.Add(CardDB.cardName.dragonkinsorcerer, 0);
            this.silenceTargets.Add(CardDB.cardName.majordomoexecutus, 0);
            this.silenceTargets.Add(CardDB.cardName.emperorthaurissan, 0);
            this.silenceTargets.Add(CardDB.cardName.chromaggus, 0);

            this.silenceTargets.Add(CardDB.cardName.quartermaster, 0);

            //TGT
            this.silenceTargets.Add(CardDB.cardName.silverhandregent, 0);
            this.silenceTargets.Add(CardDB.cardName.muklaschampion, 0);
            this.silenceTargets.Add(CardDB.cardName.maidenofthelake, 0);
            this.silenceTargets.Add(CardDB.cardName.crowdfavorite, 0);
            this.silenceTargets.Add(CardDB.cardName.kodorider, 0);
            this.silenceTargets.Add(CardDB.cardName.eydisdarkbane, 0);
            this.silenceTargets.Add(CardDB.cardName.fjolalightbane, 0);
            this.silenceTargets.Add(CardDB.cardName.nexuschampionsaraad, 0);
            this.silenceTargets.Add(CardDB.cardName.theskeletonknight, 0);
            this.silenceTargets.Add(CardDB.cardName.chillmaw, 0);
            this.silenceTargets.Add(CardDB.cardName.savagecombatant, 0);
            this.silenceTargets.Add(CardDB.cardName.aviana, 0);
            this.silenceTargets.Add(CardDB.cardName.dreadscale, 0);
            this.silenceTargets.Add(CardDB.cardName.acidmaw, 0);
            this.silenceTargets.Add(CardDB.cardName.coldarradrake, 0);
            this.silenceTargets.Add(CardDB.cardName.rhonin, 0);
            this.silenceTargets.Add(CardDB.cardName.warhorsetrainer, 0);
            this.silenceTargets.Add(CardDB.cardName.murlocknight, 0);
            this.silenceTargets.Add(CardDB.cardName.holychampion, 0);
            this.silenceTargets.Add(CardDB.cardName.confessorpaletress, 0);
            this.silenceTargets.Add(CardDB.cardName.thunderbluffvaliant, 0);
            this.silenceTargets.Add(CardDB.cardName.voidcrusher, 0);
            this.silenceTargets.Add(CardDB.cardName.wilfredfizzlebang, 0);
            this.silenceTargets.Add(CardDB.cardName.magnatauralpha, 0);
            this.silenceTargets.Add(CardDB.cardName.anubarak, 0);

            //LOE
            silenceTargets.Add(CardDB.cardName.addledgrizzly, 0);
            silenceTargets.Add(CardDB.cardName.ancientharbinger, 0);
            silenceTargets.Add(CardDB.cardName.anomalus, 0);
            silenceTargets.Add(CardDB.cardName.anubisathsentinel, 0);
            silenceTargets.Add(CardDB.cardName.blackwaterpirate, 0);
            silenceTargets.Add(CardDB.cardName.crazedworshipper, 0);
            silenceTargets.Add(CardDB.cardName.cthun, 0);
            silenceTargets.Add(CardDB.cardName.cultsorcerer, 0);
            silenceTargets.Add(CardDB.cardName.darkshirecouncilman, 0);
            silenceTargets.Add(CardDB.cardName.dementedfrostcaller, 0);
            silenceTargets.Add(CardDB.cardName.evolvedkobold, 0);
            silenceTargets.Add(CardDB.cardName.fandralstaghelm, 0);
            silenceTargets.Add(CardDB.cardName.giantsandworm, 0);
            silenceTargets.Add(CardDB.cardName.hallazealtheascended, 0);
            silenceTargets.Add(CardDB.cardName.hoggerdoomofelwynn, 0);
            silenceTargets.Add(CardDB.cardName.hoodedacolyte, 0);
            silenceTargets.Add(CardDB.cardName.scalednightmare, 0);
            silenceTargets.Add(CardDB.cardName.shiftingshade, 0);
            silenceTargets.Add(CardDB.cardName.southseasquidface, 0);
            silenceTargets.Add(CardDB.cardName.spawnofnzoth, 0);
            silenceTargets.Add(CardDB.cardName.stewardofdarkshire, 0);
            silenceTargets.Add(CardDB.cardName.theboogeymonster, 0);
            silenceTargets.Add(CardDB.cardName.twilightelder, 0);
            silenceTargets.Add(CardDB.cardName.undercityhuckster, 0);
            silenceTargets.Add(CardDB.cardName.usherofsouls, 0);
            silenceTargets.Add(CardDB.cardName.wobblingrunts, 0);
            silenceTargets.Add(CardDB.cardName.yshaarjrageunbound, 0);
        }

        private void setupRandomCards()
        {
            this.randomEffects.Add(CardDB.cardName.deadlyshot, 1); //destroy random enemy minion
            this.randomEffects.Add(CardDB.cardName.multishot, 1);

            this.randomEffects.Add(CardDB.cardName.animalcompanion, 1); //random "companion" beast
            this.randomEffects.Add(CardDB.cardName.arcanemissiles, 3); //random 3 dmg split enemies
            this.randomEffects.Add(CardDB.cardName.goblinblastmage, 1); //random 4 dmg split enemies if have mech
            this.randomEffects.Add(CardDB.cardName.avengingwrath, 8); //random 8 dmg split enemies

            this.randomEffects.Add(CardDB.cardName.flamecannon, 4); //random 4 dmg enemy minion

            //this.randomEffects.Add(CardDB.cardName.baneofdoom, 1);
            this.randomEffects.Add(CardDB.cardName.brawl, 1); //random 1 minion lives
            this.randomEffects.Add(CardDB.cardName.captainsparrot, 1);
            this.randomEffects.Add(CardDB.cardName.cleave, 1);
            this.randomEffects.Add(CardDB.cardName.forkedlightning, 1);
            this.randomEffects.Add(CardDB.cardName.gelbinmekkatorque, 1);
            this.randomEffects.Add(CardDB.cardName.iammurloc, 3);
            this.randomEffects.Add(CardDB.cardName.lightningstorm, 1); //deal 2-3 dmg all enemy minions
            this.randomEffects.Add(CardDB.cardName.madbomber, 3); //random 3 dmg split all
            this.randomEffects.Add(CardDB.cardName.mindgames, 1);
            this.randomEffects.Add(CardDB.cardName.mindcontroltech, 1); //steal random minion if they have >3
            this.randomEffects.Add(CardDB.cardName.mindvision, 1);
            this.randomEffects.Add(CardDB.cardName.powerofthehorde, 1);
            this.randomEffects.Add(CardDB.cardName.sensedemons, 2);
            this.randomEffects.Add(CardDB.cardName.tinkmasteroverspark, 1);
            this.randomEffects.Add(CardDB.cardName.totemiccall, 1); //random totem
            this.randomEffects.Add(CardDB.cardName.elitetaurenchieftain, 1); //random "power cord" card both hands
            this.randomEffects.Add(CardDB.cardName.lifetap, 1); //draw card
            this.randomEffects.Add(CardDB.cardName.soultap, 1); //draw card

            this.randomEffects.Add(CardDB.cardName.unstableportal, 1); //random minion to hand
            this.randomEffects.Add(CardDB.cardName.crackle, 1); //deal 3-6 dmg
            this.randomEffects.Add(CardDB.cardName.bouncingblade, 3); //random 1 dmg until minion dies
            this.randomEffects.Add(CardDB.cardName.coghammer, 1); //random taunt/divine shield
            this.randomEffects.Add(CardDB.cardName.madderbomber, 6); //random 6 dmg split all
            this.randomEffects.Add(CardDB.cardName.bomblobber, 1); //random 4 dmg enemy
            this.randomEffects.Add(CardDB.cardName.enhanceomechano, 1); //random taunt/windfury/divine shield own minions

            this.randomEffects.Add(CardDB.cardName.nefarian, 2);
            this.randomEffects.Add(CardDB.cardName.dieinsect, 2);
            this.randomEffects.Add(CardDB.cardName.resurrect, 2);
            this.randomEffects.Add(CardDB.cardName.fireguarddestroyer, 2);

            //TGT
            //50% minions
            this.randomEffects.Add(CardDB.cardName.mogorschampion, 1);

            this.randomEffects.Add(CardDB.cardName.gadgetzanjouster, 1);
            this.randomEffects.Add(CardDB.cardName.armoredwarhorse, 1);
            this.randomEffects.Add(CardDB.cardName.masterjouster, 1);
            this.randomEffects.Add(CardDB.cardName.tuskarrjouster, 1);
            this.randomEffects.Add(CardDB.cardName.tuskarrtotemic, 1); //random totem
            this.randomEffects.Add(CardDB.cardName.healingwave, 1); //heal 7/14 joust
            this.randomEffects.Add(CardDB.cardName.elementaldestruction, 1); //deal 4-5 dmg to all minions
            this.randomEffects.Add(CardDB.cardName.darkbargain, 2);
            this.randomEffects.Add(CardDB.cardName.varianwrynn, 3); //draw 3 cards, minions get summoned

            this.randomEffects.Add(CardDB.cardName.lockandload, 10); //random hunter card for each spell cast
            this.randomEffects.Add(CardDB.cardName.flamejuggler, 1); //random 1 dmg
            this.randomEffects.Add(CardDB.cardName.grandcrusader, 1);
            this.randomEffects.Add(CardDB.cardName.spellslinger, 1); //random spell to both hands
            randomEffects.Add(CardDB.cardName.cthun, 10); //random X dmg split enemies, X=attack
            randomEffects.Add(CardDB.cardName.fierybat, 1); //random 1 dmg
            randomEffects.Add(CardDB.cardName.spreadingmadness, 9); //random 9 dmg split all

            //todo these need more thought in getRandomPenalty
            this.randomEffects.Add(CardDB.cardName.servantofyoggsaron, 1); //random 1 spell
            this.randomEffects.Add(CardDB.cardName.cabaliststome, 1); //random 3 mage spells
            this.randomEffects.Add(CardDB.cardName.nexuschampionsaraad, 1); //random spell inspire
            this.randomEffects.Add(CardDB.cardName.xarilpoisonedmind, 1); //random toxin, random toxin deathrattle
            this.randomEffects.Add(CardDB.cardName.yoggsaronhopesend, 1); //cast X random spells equal to # previous casts
            this.randomEffects.Add(CardDB.cardName.infest, 1); //add random beast deathrattle

            this.randomEffects.Add(CardDB.cardName.goldenmonkey, 1); //replace your cards with legendaries
            this.randomEffects.Add(CardDB.cardName.timepieceofhorror, 1); //random 10 dmg split enemies
            this.randomEffects.Add(CardDB.cardName.succubus, 1); //discard card
            this.randomEffects.Add(CardDB.cardName.desertcamel, 1); //summon 1 cost from both decks
            this.randomEffects.Add(CardDB.cardName.facelesssummoner, 1); //summon random 3 cost
            this.randomEffects.Add(CardDB.cardName.hugetoad, 1); //random 1 dmg deathrattle
            this.randomEffects.Add(CardDB.cardName.hungrydragon, 1); //summon random 1 cost for enemy
            this.randomEffects.Add(CardDB.cardName.mountedraptor, 1); //summon random 1 cost deathrattle
            this.randomEffects.Add(CardDB.cardName.murlocknight, 1); //summon random murloc inspire
            this.randomEffects.Add(CardDB.cardName.rumblingelemental, 2); //random 2 dmg after each battlecry
            this.randomEffects.Add(CardDB.cardName.soulfire, 1); //discard card
            this.randomEffects.Add(CardDB.cardName.thoughtsteal, 1); //copy 2 cards from enemy deck
            this.randomEffects.Add(CardDB.cardName.zealousinitiate, 1); //give random +1/1 deathrattle
            this.randomEffects.Add(CardDB.cardName.burgle, 1); //add 2 random enemy class cards
            this.randomEffects.Add(CardDB.cardName.darkshirelibrarian, 1); //discard card, draw card deathrattle
            this.randomEffects.Add(CardDB.cardName.dementedfrostcaller, 1); //freeze random enemy every spell
            this.randomEffects.Add(CardDB.cardName.doomguard, 1); //discard 2
            this.randomEffects.Add(CardDB.cardName.evolve, 1); //replace all your minions with ones that cost 1 more
            this.randomEffects.Add(CardDB.cardName.fistofjaraxxus, 1); //random 4 dmg
            this.randomEffects.Add(CardDB.cardName.flamewaker, 1); //random 2 dmg split after spell
            this.randomEffects.Add(CardDB.cardName.knifejuggler, 1); //random 1 dmg every summon
            this.randomEffects.Add(CardDB.cardName.masterofevolution, 1); //replace minion with one that costs 1 more
            this.randomEffects.Add(CardDB.cardName.ramwrangler, 1); //summon random beast if you have a beast
            this.randomEffects.Add(CardDB.cardName.selflesshero, 1); //random divine shield deathrattle
            this.randomEffects.Add(CardDB.cardName.shifterzerus, 1); //his morphing doesn't matter but w/e he becomes could have random effects //todo sepefeets - remove him once we properly detect what he becomes
            this.randomEffects.Add(CardDB.cardName.shiftingshade, 1); //copy card from enemy deck deathrattle
            this.randomEffects.Add(CardDB.cardName.stampedingkodo, 1); // destroy random <3 attack minion
            this.randomEffects.Add(CardDB.cardName.summoningstone, 1); // summon minion with equal cost to any spell cast
            this.randomEffects.Add(CardDB.cardName.undercityhuckster, 1); //add random enemy class card deathrattle
            this.randomEffects.Add(CardDB.cardName.voidcrusher, 1); //destroy random minion for both players inspire
            this.randomEffects.Add(CardDB.cardName.renouncedarkness, 1); //replace cards/power with a new class
            this.randomEffects.Add(CardDB.cardName.confessorpaletress, 1); //random legendary inspire
            this.randomEffects.Add(CardDB.cardName.eydisdarkbane, 1); //random 3 dmg when you target her with spell
            this.randomEffects.Add(CardDB.cardName.malkorok, 1); //equip random weapon
            this.randomEffects.Add(CardDB.cardName.sylvanaswindrunner, 1); //steal random enemy deathrattle

            randomEffects.Add(CardDB.cardName.firelandsportal, 1);
            randomEffects.Add(CardDB.cardName.maelstromportal, 1);
            randomEffects.Add(CardDB.cardName.ivoryknight, 1);
            randomEffects.Add(CardDB.cardName.silvermoonportal, 1);
            randomEffects.Add(CardDB.cardName.onyxbishop, 1);
            randomEffects.Add(CardDB.cardName.swashburglar, 1);
            randomEffects.Add(CardDB.cardName.wickedwitchdoctor, 1);
            randomEffects.Add(CardDB.cardName.barnes, 1);
            randomEffects.Add(CardDB.cardName.babblingbook, 1);
            randomEffects.Add(CardDB.cardName.zoobot, 1);
            randomEffects.Add(CardDB.cardName.menageriemagician, 1);
            randomEffects.Add(CardDB.cardName.thecurator, 1);
            randomEffects.Add(CardDB.cardName.malchezaarsimp, 1);
            randomEffects.Add(CardDB.cardName.ironforgeportal, 1);
            randomEffects.Add(CardDB.cardName.netherspitehistorian, 1);
            randomEffects.Add(CardDB.cardName.atiesh, 1);
            randomEffects.Add(CardDB.cardName.moongladeportal, 1);

            //ungoro
            randomEffects.Add(CardDB.cardName.tortollanforager, 1);
        }

        private void setupTargetAbilitys()
        {
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.CS1h_001, 1);  //Lesser Heal
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.CS1h_001_H1, 1);
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.AT_132_PRIEST, 1);
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.CS1h_001_H1_AT_132, 1);
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.CS2_034, 1);  //Fireblast
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.CS2_034_H1, 1);
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.CS2_034_H2, 1);
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.AT_132_MAGE, 1); //Fireblast Rank 2
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.CS2_034_H1_AT_132, 1);
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.CS2_034_H2_AT_132, 1);
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.DS1h_292, 1);  //Steady Shot
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.DS1h_292_H1, 1);
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.AT_132_HUNTER, 1); //Ballista Shot
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.DS1h_292_H1_AT_132, 1);
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.EX1_625t, 1);  //Mind Spike r1
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.EX1_625t2, 1);  //Mind Spike r2
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.AT_050t, 1);  //Lightning Jolt
            this.TargetAbilitysDatabase.Add(CardDB.cardIDEnum.UNG_917t1, 1); //Dinomancy
        }

        public void setupDiscover()
        {
            //original
            discoverCards.Add(CardDB.cardName.tracking, 1);

            //explorer
            discoverCards.Add(CardDB.cardName.jeweledscarab, 1);
            discoverCards.Add(CardDB.cardName.darkpeddler, 1);
            discoverCards.Add(CardDB.cardName.tombspider, 1);
            discoverCards.Add(CardDB.cardName.gorillabota3, 1);  // only if you have a mech
            discoverCards.Add(CardDB.cardName.etherealconjurer, 1);
            discoverCards.Add(CardDB.cardName.museumcurator, 1);
            discoverCards.Add(CardDB.cardName.ravenidol, 1);
            discoverCards.Add(CardDB.cardName.archthiefrafaam, 1);
            discoverCards.Add(CardDB.cardName.sirfinleymrrgglton, 1);

            //old gods
            discoverCards.Add(CardDB.cardName.alightinthedarkness, 1);
            discoverCards.Add(CardDB.cardName.journeybelow, 1);

            //kara
            discoverCards.Add(CardDB.cardName.ivoryknight, 1);
            discoverCards.Add(CardDB.cardName.netherspitehistorian, 1);

            //gadget
            discoverCards.Add(CardDB.cardName.kabalcourier, 1);
            discoverCards.Add(CardDB.cardName.lotusagents, 1);
            discoverCards.Add(CardDB.cardName.drakonidoperative, 1);
            discoverCards.Add(CardDB.cardName.finderskeepers, 1);
            discoverCards.Add(CardDB.cardName.iknowaguy, 1);
            discoverCards.Add(CardDB.cardName.grimestreetinformant, 1);

            //ungoro
            discoverCards.Add(CardDB.cardName.chitteringtunneler, 1);
            discoverCards.Add(CardDB.cardName.stonehilldefender, 1);
            discoverCards.Add(CardDB.cardName.hydrologist, 1);
            discoverCards.Add(CardDB.cardName.shadowvisions, 1);
            discoverCards.Add(CardDB.cardName.primalfinlookout, 1);
            discoverCards.Add(CardDB.cardName.servantofkalimos, 1); // when drop elemental last turn
            discoverCards.Add(CardDB.cardName.primordialglyph, 1);
            discoverCards.Add(CardDB.cardName.hallucination, 1);
        }

        private void setupStrongInspireMinions()
        {
            strongInspireEffectMinions.Add(CardDB.cardName.boneguardlieutenant, 0);
            strongInspireEffectMinions.Add(CardDB.cardName.confessorpaletress, 10);
            strongInspireEffectMinions.Add(CardDB.cardName.dalaranaspirant, 0);
            strongInspireEffectMinions.Add(CardDB.cardName.kodorider, 10);
            strongInspireEffectMinions.Add(CardDB.cardName.kvaldirraider, 10);
            strongInspireEffectMinions.Add(CardDB.cardName.lowlysquire, 0);
            strongInspireEffectMinions.Add(CardDB.cardName.muklaschampion, 10);
            strongInspireEffectMinions.Add(CardDB.cardName.murlocknight, 10);
            strongInspireEffectMinions.Add(CardDB.cardName.nexuschampionsaraad, 10);
            strongInspireEffectMinions.Add(CardDB.cardName.recruiter, 1);
            strongInspireEffectMinions.Add(CardDB.cardName.thunderbluffvaliant, 0);
            strongInspireEffectMinions.Add(CardDB.cardName.tournamentmedic, 1);
            strongInspireEffectMinions.Add(CardDB.cardName.savagecombatant, 0);
            strongInspireEffectMinions.Add(CardDB.cardName.silverhandregent, 0);
        }

        private void anti_aoe()
        {
            this.anti_aoe_minion.Add(CardDB.cardName.nerubianegg, 12);
            this.anti_aoe_minion.Add(CardDB.cardName.hauntedcreeper, 9);
            this.anti_aoe_minion.Add(CardDB.cardName.voidcaller, 10);
            this.anti_aoe_minion.Add(CardDB.cardName.pilotedshredder, 12);
            this.anti_aoe_minion.Add(CardDB.cardName.pilotedskygolem, 10);
            this.anti_aoe_minion.Add(CardDB.cardName.madscientist, 5);
            this.anti_aoe_minion.Add(CardDB.cardName.savannahhighmane, 15);
            this.anti_aoe_minion.Add(CardDB.cardName.sylvanaswindrunner, 3);
            this.anti_aoe_minion.Add(CardDB.cardName.darkcultist, 3);
            this.anti_aoe_minion.Add(CardDB.cardName.harvestgolem, 4);
            this.anti_aoe_minion.Add(CardDB.cardName.anubisathsentinel, 5);
            this.anti_aoe_minion.Add(CardDB.cardName.mountedraptor, 6);
            this.anti_aoe_minion.Add(CardDB.cardName.wobblingrunts, 15);
            this.anti_aoe_minion.Add(CardDB.cardName.sludgebelcher, 4);
            this.anti_aoe_minion.Add(CardDB.cardName.tirionfordring, 6);
            this.anti_aoe_minion.Add(CardDB.cardName.possessedvillager, 7);
            this.anti_aoe_minion.Add(CardDB.cardName.infestedwolf, 8);
            this.anti_aoe_minion.Add(CardDB.cardName.kindlygrandmother, 12);
            this.anti_aoe_minion.Add(CardDB.cardName.impgangboss, 3);
            this.anti_aoe_minion.Add(CardDB.cardName.dragonegg, 7);
            this.anti_aoe_minion.Add(CardDB.cardName.ayablackpaw, 8);
            this.anti_aoe_minion.Add(CardDB.cardName.cairnebloodhoof, 10);
        }

        public void setupSummonMinionSpellsDatabase()
        {
            summonMinionSpellsDatabase.Add(CardDB.cardName.mirrorimage, 2);
            summonMinionSpellsDatabase.Add(CardDB.cardName.totemiccall, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.reinforce, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.animalcompanion, 3);
            summonMinionSpellsDatabase.Add(CardDB.cardName.powerofthewild, 1); //choice
            summonMinionSpellsDatabase.Add(CardDB.cardName.feralspirit, 2);
            summonMinionSpellsDatabase.Add(CardDB.cardName.baneofdoom, 1); //only if it kills
            summonMinionSpellsDatabase.Add(CardDB.cardName.unleashthehounds, 1); //count=enemy minions
            summonMinionSpellsDatabase.Add(CardDB.cardName.forceofnature, 3);
            summonMinionSpellsDatabase.Add(CardDB.cardName.thesilverhand, 2);
            summonMinionSpellsDatabase.Add(CardDB.cardName.anyfincanhappen, 1); //count=dead murlocs
            summonMinionSpellsDatabase.Add(CardDB.cardName.thetidalhand, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.onthehunt, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.forbiddenshaping, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.forbiddenritual, 1); //count=mana spent
            summonMinionSpellsDatabase.Add(CardDB.cardName.wispsoftheoldgods, 7); //choice
            summonMinionSpellsDatabase.Add(CardDB.cardName.callofthewild, 3);
            summonMinionSpellsDatabase.Add(CardDB.cardName.standagainstdarkness, 5);
            summonMinionSpellsDatabase.Add(CardDB.cardName.bloodtoichor, 1); //if it doesn't kill
            summonMinionSpellsDatabase.Add(CardDB.cardName.iammurloc, 3); //3-5
            summonMinionSpellsDatabase.Add(CardDB.cardName.powerofthehorde, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.lightofthenaaru, 1); //if it doesn't heal to full
            summonMinionSpellsDatabase.Add(CardDB.cardName.darkwispers, 5); //choice
            summonMinionSpellsDatabase.Add(CardDB.cardName.musterforbattle, 3);
            summonMinionSpellsDatabase.Add(CardDB.cardName.livingroots, 2); //choice
            summonMinionSpellsDatabase.Add(CardDB.cardName.ballofspiders, 3);
            summonMinionSpellsDatabase.Add(CardDB.cardName.totemicslam, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.firelandsportal, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.maelstromportal, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.protecttheking, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.silvermoonportal, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.karakazham, 3);
            summonMinionSpellsDatabase.Add(CardDB.cardName.moongladeportal, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.ironforgeportal, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.jadeidol, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.jadeblossom, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.jadeshuriken, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.callinthefinishers, 1);
            summonMinionSpellsDatabase.Add(CardDB.cardName.jadelightning, 1);
            //ungoro
            summonMinionSpellsDatabase.Add(CardDB.cardName.lostinthejungle, 2);
            summonMinionSpellsDatabase.Add(CardDB.cardName.moltenreflection, 1);
        }

        private void setupAlsoEquipsWeaponDB()
        {
            alsoEquipsWeaponDB.Add(CardDB.cardName.arathiweaponsmith, 2);
            alsoEquipsWeaponDB.Add(CardDB.cardName.blingtron3000, 3); //random weapon so be conservative
            alsoEquipsWeaponDB.Add(CardDB.cardName.malkorok, 3); //random weapon so be conservative
            alsoEquipsWeaponDB.Add(CardDB.cardName.musterforbattle, 1);
            alsoEquipsWeaponDB.Add(CardDB.cardName.nzothsfirstmate, 1);
            alsoEquipsWeaponDB.Add(CardDB.cardName.upgrade, 1); //if we don't have a weapon
            alsoEquipsWeaponDB.Add(CardDB.cardName.medivhtheguardian, 1);
        }

        public void setupNeedSleepBecauseTimingMinionsDB()
        {
            //damaged
            NeedSleepBecauseTimingMinionsDB.Add(CardDB.cardName.gahzrilla, 2);
            NeedSleepBecauseTimingMinionsDB.Add(CardDB.cardName.frothingberserker, 2);
            NeedSleepBecauseTimingMinionsDB.Add(CardDB.cardName.wrathguard, 2);
            NeedSleepBecauseTimingMinionsDB.Add(CardDB.cardName.gurubashiberserker, 2);
            NeedSleepBecauseTimingMinionsDB.Add(CardDB.cardName.dragonegg, 2);
            NeedSleepBecauseTimingMinionsDB.Add(CardDB.cardName.impgangboss, 2);

            //
        }

        public void AdaptMinionDB()
        {
            AdaptDatabase.Add(CardDB.cardName.ravenouspterrordax, 0);
            AdaptDatabase.Add(CardDB.cardName.elderlongneck, 0);
            AdaptDatabase.Add(CardDB.cardName.lightfusedstegodon, 0);
            AdaptDatabase.Add(CardDB.cardName.pterrordaxhatchling, 0);
            AdaptDatabase.Add(CardDB.cardName.ornerydirehorn, 0);
            AdaptDatabase.Add(CardDB.cardName.verdantlongneck, 0);
            AdaptDatabase.Add(CardDB.cardName.ravasaurrunt, 0);
            AdaptDatabase.Add(CardDB.cardName.gentlemegasaur, 0);
            AdaptDatabase.Add(CardDB.cardName.adaptation, 0);
            AdaptDatabase.Add(CardDB.cardName.evolvingspores, 0);
            AdaptDatabase.Add(CardDB.cardName.thunderlizard, 0);
            AdaptDatabase.Add(CardDB.cardName.cracklingrazormaw, 0);
            AdaptDatabase.Add(CardDB.cardName.volcanosaur, 0);
            //AdaptDatabase.Add(CardDB.cardName.viciousfledgling, 0);//adapt after attacking hero
        }
    }
}
