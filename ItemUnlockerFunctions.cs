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
using System.Data.Common;

namespace ItemUnlocker
{
    public class ItemUnlockerFunctions
    {
        public static List<string> starterItems = [];

        public static void SetStarterItems()
        {
            starterItems = [];
            Dictionary<string, SubClassData> _subClassDataSource = Traverse.Create(Globals.Instance).Field("_SubClassSource").GetValue<Dictionary<string, SubClassData>>();
            if (_subClassDataSource == null)
            {
                LogError("Failed to get subclass data source");
                return;
            }

            foreach (SubClassData subclass in _subClassDataSource.Values)
            {
                if (subclass == null || subclass.Item == null)
                {
                    LogDebug($"Subclass {subclass?.Id ?? ""} or subclass item is null");
                    continue;
                }
                starterItems.Add(subclass.Item.Id);
            }

            // LogDebug($"StarterItem List = {string.Join(", ", starterItems)}");
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
                if (itemDataLocal.ContainsKey(itemID) &&
                    ShouldMakeCardDroponly(itemID, itemDataLocal))
                {
                    itemDataLocal[itemID].DropOnly = false;
                }
            }
            Traverse.Create(Globals.Instance).Field("_ItemDataSource").SetValue(itemDataLocal);

            Dictionary<string, CardData> cardSourceLocal = Traverse.Create(Globals.Instance).Field("_CardsSource").GetValue<Dictionary<string, CardData>>();
            Dictionary<string, CardData> cardsLocal = Traverse.Create(Globals.Instance).Field("_Cards").GetValue<Dictionary<string, CardData>>();
            foreach (string cardID in cardSourceLocal.Keys)
            {
                if (cardsLocal.ContainsKey(cardID) &&
                    cardsLocal[cardID].CardType != Enums.CardType.Pet &&
                    cardsLocal[cardID].Item != null &&
                    ShouldMakeCardDroponly(cardID, cardSourceLocal))
                {
                    if (cardSourceLocal[cardID].Item.DropOnly)
                        LogDebug($"Adding {cardID} to pool");
                    cardSourceLocal[cardID].Item.DropOnly = false;
                    cardsLocal[cardID].Item.DropOnly = false;
                }
            }
            Traverse.Create(Globals.Instance).Field("_Cards").SetValue(cardsLocal);
            Traverse.Create(Globals.Instance).Field("_CardsSource").SetValue(cardSourceLocal);
        }

        public static bool ShouldMakeCardDroponly(string id, Dictionary<string, ItemData> data)
        {
            if (id.EndsWith("rare"))
                return false;
            if (IsStarterItem(id))
            {
                LogDebug($"Starter item Found {id}");
                return false;
            }
            if (IsStarterItem(id) && DisableStarterItems.Value)
                return false;
            if (IsStarterItem(id) && DisableStarterItemUpgrades.Value && (id.EndsWith("a") || id.EndsWith("b")))
                return false;
            return true;
        }

        public static bool ShouldMakeCardDroponly(string id, Dictionary<string, CardData> data)
        {
            if (IsStarterItem(id))
            {
                LogDebug($"Starter item Found {id}");
                return false;
            }

            if (data.TryGetValue(id, out CardData card))
            {
                if ((card.CardUpgraded == Enums.CardUpgraded.Rare) ||
                    (card.CardUpgraded != Enums.CardUpgraded.No && DisableStarterItemUpgrades.Value && IsStarterItem(id)) ||
                    (DisableStarterItems.Value && IsStarterItem(id)))
                {
                    return false;
                }
                return true;
            }
            return false;

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

