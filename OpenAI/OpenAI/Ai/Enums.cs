namespace OpenAI
{
    public enum ActionType
    {
        INVALID = 0,
        ATTACK_WITH_HERO = 1,
        ATTACK_WITH_MINION = 2,
        END_TURN = 3,
        PLAY_CARD = 4,
        USE_HERO_POWER = 5,
    }

    public enum ComboType
    {
        INVALID = 0,
        COMBO = 1,
        TARGET = 2,
        WEAPON_USE = 3,
    }
}
