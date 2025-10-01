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
        public static List<string> starterItems = [];

        public static void SetStarterItems()
        {
            starterItems = [];
            Dictionary<string, SubClassData> _subClassDataSource = Traverse.Create(Globals.Instance).Field("_SubClassDataSource").GetValue<Dictionary<string, SubClassData>>();
            foreach (SubClassData subclass in _subClassDataSource.Values)
            {
                starterItems.Add(subclass.Item.Id);
            }
        }

        public static bool IsStarterItem(string itemID)
        {
            // if the itemId starts with any of the starterItems, return true
            foreach (string starterItem in starterItems)
            {
                if (itemID.StartsWith(starterItem))
                {
                    return true;
                }
            }
            return false;
        }

        public static void UpdateDropOnlyItems()
        {
            LogDebug("Updating drop-only items...");
            Dictionary<string, ItemData> itemDataLocal = Traverse.Create(Globals.Instance).Field("_ItemDataSource").GetValue<Dictionary<string, ItemData>>();
            //bool bHasSOU = (GameManager.Instance != null && GameManager.Instance.IsMultiplayer() && NetworkManager.Instance != null && NetworkManager.Instance.AnyPlayersHaveSku("2511580")) || (SteamManager.Instance != null && SteamManager.Instance.PlayerHaveDLC("2511580"));
            foreach (string itemID in itemDataLocal.Keys)
            {
                if (itemDataLocal.ContainsKey(itemID))
                {
                    itemDataLocal[itemID].DropOnly = false;
                }
            }
            Traverse.Create(Globals.Instance).Field("_ItemDataSource").SetValue(itemDataLocal);

            Dictionary<string, CardData> cardSourceLocal = Traverse.Create(Globals.Instance).Field("_CardsSource").GetValue<Dictionary<string, CardData>>();
            Dictionary<string, CardData> cardsLocal = Traverse.Create(Globals.Instance).Field("_Cards").GetValue<Dictionary<string, CardData>>();
            foreach (string cardID in cardSourceLocal.Keys)
            {
                if (cardsLocal.ContainsKey(cardID) && cardsLocal[cardID].CardType != Enums.CardType.Pet && cardsLocal[cardID].Item != null)
                {
                    if (cardSourceLocal[cardID].CardUpgraded != Enums.CardUpgraded.No || (DisableStarterItems.Value && IsStarterItem(cardID)))
                    {
                        continue;
                    }
                    cardSourceLocal[cardID].Item.DropOnly = false;
                    cardsLocal[cardID].Item.DropOnly = false;
                }
            }
            Traverse.Create(Globals.Instance).Field("_Cards").SetValue(cardsLocal);
            Traverse.Create(Globals.Instance).Field("_CardsSource").SetValue(cardSourceLocal);
        }

        public static void UnlockItem(string itemId)
        {
            Dictionary<string, ItemData> itemDataLocal = Traverse.Create(Globals.Instance).Field("_ItemDataSource").GetValue<Dictionary<string, ItemData>>();
            Dictionary<string, CardData> cardSourceLocal = Traverse.Create(Globals.Instance).Field("_CardsSource").GetValue<Dictionary<string, CardData>>();
            Dictionary<string, CardData> cardsLocal = Traverse.Create(Globals.Instance).Field("_Cards").GetValue<Dictionary<string, CardData>>();

            if (cardSourceLocal.TryGetValue(itemId, out CardData card))
            {
                if (card != null && card.Item != null)
                {
                    if (card.CardUpgraded != Enums.CardUpgraded.No)
                    {
                        return;
                    }
                    card.Item.DropOnly = false;
                }
            }
            if (cardsLocal.TryGetValue(itemId, out CardData card2))
            {
                if (card2 != null && card2.Item != null)
                {
                    card2.Item.DropOnly = false;
                }
            }
            if (itemDataLocal.TryGetValue(itemId, out ItemData item))
            {
                if (item != null)
                    item.DropOnly = false;
            }

            Traverse.Create(Globals.Instance).Field("_ItemDataSource").SetValue(itemDataLocal);
            Traverse.Create(Globals.Instance).Field("_CardsSource").SetValue(cardSourceLocal);
            Traverse.Create(Globals.Instance).Field("_Cards").SetValue(cardsLocal);

        }

    }
}

