﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CS2ScreenMenuAPI;
using StoreApi;
using System.Text.Json;
using static StoreApi.Store;

namespace Store;

public class Store : BasePlugin, IPluginConfig<Item_Config>
{
    public override string ModuleName => "Store";
    public override string ModuleVersion => "1.8";
    public override string ModuleAuthor => "schwarper";

    public Item_Config Config { get; set; } = new Item_Config();
    public List<Store_Player> GlobalStorePlayers { get; set; } = [];
    public List<Store_Item> GlobalStorePlayerItems { get; set; } = [];
    public List<Store_Equipment> GlobalStorePlayerEquipments { get; set; } = [];
    public List<Store_Item_Types> GlobalStoreItemTypes { get; set; } = [];
    public Dictionary<CCSPlayerController, PlayerTimer> GlobalDictionaryPlayer { get; set; } = [];
    public int GlobalTickrate { get; set; } = 0;
    public static Store Instance { get; set; } = new();
    public Random Random { get; set; } = new();
    public Dictionary<CCSPlayerController, float> GlobalGiftTimeout { get; set; } = [];
    public static StoreAPI Api { get; set; } = new();
    public Dictionary<string, Dictionary<string, string>> Items { get; set; } = [];
    public Dictionary<CBaseModelEntity, CCSPlayerController> InspectList { get; set; } = [];

    public override void Load(bool hotReload)
    {
        Capabilities.RegisterPluginCapability(IStoreApi.Capability, () => Api);

        Instance = this;

        Event.Load();
        Command.Load();

        Item_Armor.OnPluginStart();
        Item_Bunnyhop.OnPluginStart();
        Item_ColoredSkin.OnPluginStart();
        Item_CustomWeapon.OnPluginStart();
        Item_Godmode.OnPluginStart();
        Item_Gravity.OnPluginStart();
        Item_GrenadeTrail.OnPluginStart();
        Item_Health.OnPluginStart();
        Item_Link.OnPluginStart();
        Item_Open.OnPluginStart();
        Item_PlayerSkin.OnPluginStart();
        Item_Respawn.OnPluginStart();
        Item_Smoke.OnPluginStart();
        Item_Sound.OnPluginStart();
        Item_Speed.OnPluginStart();
        Item_Tracer.OnPluginStart();
        Item_Trail.OnPluginStart();
        Item_Weapon.OnPluginStart();
        Item_Equipment.OnPluginStart();

        if (hotReload)
        {
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                Database.LoadPlayer(player);
                MenuAPI.CloseActiveMenu(player);
            }
        }
    }

    public override void Unload(bool hotReload)
    {
        Event.Unload();

        HashSet<string> ScreenMenuNames = ["worldtext", "screen", "screenmenu"];

        if (ScreenMenuNames.Contains(Config_Config.Config.Menu.MenuType))
        {
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                MenuAPI.CloseActiveMenu(player);
            }
        }
    }

    public void OnConfigParsed(Item_Config config)
    {
        Config_Config.Load();

        if (config.Items.ValueKind != JsonValueKind.Object)
        {
            throw new JsonException();
        }

        Dictionary<string, Dictionary<string, string>> itemsDictionary = [];

        foreach (JsonProperty category in config.Items.EnumerateObject())
        {
            ExtractItems(category.Value, itemsDictionary);
        }

        Items = itemsDictionary;
        Config = config;
    }

    public static void ExtractItems(JsonElement category, Dictionary<string, Dictionary<string, string>> itemsDictionary)
    {
        foreach (JsonProperty subItem in category.EnumerateObject())
        {
            if (subItem.Value.ValueKind == JsonValueKind.Object)
            {
                if (subItem.Value.TryGetProperty("uniqueid", out JsonElement uniqueIdElement))
                {
                    string uniqueId = uniqueIdElement.GetString() ?? $"unknown_{subItem.Name}";
                    Dictionary<string, string> itemData = [];

                    foreach (JsonProperty property in subItem.Value.EnumerateObject())
                    {
                        itemData[property.Name] = property.Value.ToString();
                    }

                    itemsDictionary[uniqueId] = itemData;
                }
                else
                {
                    ExtractItems(subItem.Value, itemsDictionary);
                }
            }
        }
    }
}
