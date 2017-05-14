using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redfish.ai
{
    class ArenaDraft
    {

        //private int CountDeckCardNum(int cost,bool is_minion, bool is_spell,List<HSRangerLib.GameArenaDraftEventArgs.DeckCard> deck)
        //{
        //    int num = 0;

        //    foreach (var item in deck)
        //    {
        //        CardDef def = CardDefDB.Instance.GetCardDef(item.card_id);

        //        if (def.Cost == cost)
        //        {
        //            if (is_minion)
        //            {
        //                if (def.CardType == TAG_CARDTYPE.MINION)
        //                {
        //                    num += item.num;
        //                }
        //            }

        //            if (is_spell)
        //            {
        //                if (def.CardType == TAG_CARDTYPE.ABILITY ||
        //                    def.CardType == TAG_CARDTYPE.ENCHANTMENT)
        //                {
        //                    num += item.num;
        //                }
        //            }
        //        }
        //    }

        //    return num;
        //}

        //private int f(string hero_id)
        //{
        //    CardDef def = CardDefDB.Instance.GetCardDef(hero_id);

        //    //No.1 choice (Your best choice)
        //    if (def.Class == TAG_CLASS.DRUID)
        //    {
        //        return 1;
        //    }

        //    //No.2 choice
        //    if (def.Class == TAG_CLASS.HUNTER)
        //    {
        //        return 2;
        //    }

        //    //No.3 choice
        //    if (def.Class == TAG_CLASS.MAGE)
        //    {
        //        return 3;
        //    }

        //    //No.4 choice
        //    if (def.Class == TAG_CLASS.PALADIN)
        //    {
        //        return 4; 
        //    }

        //    //No.5 choice
        //    if (def.Class == TAG_CLASS.PRIEST)
        //    {
        //        return 5;
        //    }

        //    //No.6 choice
        //    if (def.Class == TAG_CLASS.ROGUE)
        //    {
        //        return 6;
        //    }
        //    //No.7 choice
        //    if (def.Class == TAG_CLASS.SHAMAN)
        //    {
        //        return 7;
        //    }
        //    //No.8 choice
        //    if (def.Class == TAG_CLASS.WARLOCK)
        //    {
        //        return 8;
        //    }
        //    //No.9 choice
        //    if (def.Class == TAG_CLASS.WARRIOR)
        //    {
        //        return 9;
        //    }

        //    return 100;
        //}

        //private string GetBestHeroCardId(GameArenaDraftEventArgs e)
        //{
        //    string best_hero_id = "";
        //    foreach (var card_id in e.draft_choices.OrderBy( hero => GetHeroPriority(hero)))
        //    {
        //        best_hero_id = card_id;
        //        break;
        //    }

        //    return best_hero_id;
        //}

    }
}
