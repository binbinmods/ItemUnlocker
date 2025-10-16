using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
// using static Obeliskial_Essentials.Essentials;
using System;
using static ItemUnlocker.Plugin;
using static ItemUnlocker.CustomFunctions;
using static ItemUnlocker.ItemUnlockerFunctions;
using System.Collections.Generic;
using static Functions;
using UnityEngine;
// using Photon.Pun;
using TMPro;
using System.Linq;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
// using Unity.TextMeshPro;

// Make sure your namespace is the same everywhere
namespace ItemUnlocker
{

    [HarmonyPatch] // DO NOT REMOVE/CHANGE - This tells your plugin that this is part of the mod

    public class ItemUnlockerPatches
    {
        public static bool devMode = false; //DevMode.Value;



        [HarmonyPostfix]
        [HarmonyPatch(typeof(MainMenuManager), "Start")]
        public static void MMStartPostfix(ref MainMenuManager __instance)
        {
            SetStarterItems();
            if (NoDropOnlyItems.Value)
            {
                UpdateDropOnlyItems();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LootManager), "ShowItemsForLoot")]
        public static void ShowItemsForLootPostfix(string itemListId)
        {
            if (!UnlockAsYouGo.Value)
                return;
            List<string> itemList = AtOManager.Instance.GetItemList(itemListId);
            foreach (string itemId in itemList)
            {
                LogDebug($"Unlocking item {itemId}");
                UnlockItem(itemId);
            }
        }

    }
}