namespace OpenAI
{
    public class SimTemplate
    {
        public virtual void OnSecretPlay(Playfield p, bool ownplay, Minion attacker, Minion target, out int number)
        {
            number = 0;
        }

        public virtual void OnSecretPlay(Playfield p, bool ownplay, Minion target, int number)
        {
            return;
        }

        public virtual void OnSecretPlay(Playfield p, bool ownplay, int number)
        {
            return;
        }

        public virtual void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            return;
        }

        public virtual void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            return;
        }

        public virtual void OnAuraStarts(Playfield p, Minion m)
        {
            return;
        }

        public virtual void OnAuraEnds(Playfield p, Minion m)
        {
            return;
        }

        public virtual void OnInspire(Playfield p, Minion m)
        {
            return;
        }

        public virtual void OnEnrageStart(Playfield p, Minion m)
        {
            return;
        }

        public virtual void OnEnrageStop(Playfield p, Minion m)
        {
            return;
        }

        public virtual void OnAMinionGotHealedTrigger(Playfield p, Minion triggerEffectMinion)
        {
            return;
        }

        public virtual void OnAHeroGotHealedTrigger(Playfield p, Minion triggerEffectMinion)
        {
            return;
        }

        public virtual void OnTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            return;
        }

        public virtual void OnTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
            return;
        }

        public virtual void OnMinionGotDmgTrigger(Playfield p, Minion triggerEffectMinion, bool ownDmgdMinion)
        {
            return;
        }

        public virtual void OnMinionDiedTrigger(Playfield p, Minion triggerEffectMinion, Minion diedMinion)
        {
            return;
        }

        public virtual void OnMinionIsSummoned(Playfield p, Minion triggerEffectMinion, Minion summonedMinion)
        {
            return;
        }

        public virtual void OnMinionWasSummoned(Playfield p, Minion triggerEffectMinion, Minion summonedMinion)
        {
            return;
        }

        public virtual void OnDeathrattle(Playfield p, Minion m)
        {
            return;
        }

        public virtual void OnCardIsGoingToBePlayed(Playfield p, CardDB.Card c, bool wasOwnCard, Minion triggerEffectMinion, Minion target, int choice)
        {
            return;
        }

        public virtual void OnCardWasPlayed(Playfield p, CardDB.Card c, bool wasOwnCard, Minion triggerEffectMinion)
        {
            return;
        }


        public virtual void OnCardWasDiscarded(Playfield p, bool wasOwnCard, Minion triggerEffectMinion)
        {
            return;
        }

        public virtual void OnCardIsDiscarded(Playfield p, CardDB.Card card, bool own)
        {
            return;
        }

        public virtual void OnCardToDecks(Playfield p, bool ownplay, Minion target, int choice)
        {
            return;
        }

        public virtual void OnAdaptChoice(Playfield p, bool ownplay, Minion target, CardDB.cardIDEnum choice)
        {
            return;
        }
    }
}
