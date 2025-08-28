using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
// using Obeliskial_Content;
// using Obeliskial_Essentials;
using System.IO;
using static UnityEngine.Mathf;
using UnityEngine.TextCore.LowLevel;
using static ItemUnlocker.Plugin;
using System.Collections.ObjectModel;
using UnityEngine;

namespace ItemUnlocker
{
    public class ItemUnlockerFunctions
    {


        public static void UnlockItem(string itemId)
        {
            Dictionary<string, CardData> cardsSource = Traverse.Create(Globals.Instance).Field("_CardsSource").GetValue<Dictionary<string, CardData>>();
            if (cardsSource.TryGetValue(itemId, out CardData card))
            {
                if (card != null && card.Item != null)
                    card.Item.DropOnly = false;
            }

            Traverse.Create(Globals.Instance).Field("_CardsSource").SetValue(cardsSource);

        }

    }
}

