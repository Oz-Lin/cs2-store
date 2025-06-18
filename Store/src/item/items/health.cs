using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Store.Extension;
using static Store.ConfigConfig;
using static StoreApi.Store;

namespace Store;

[StoreItemType("health")]
public class ItemHealth : IItemModule
{
    public bool Equipable => false;
    public bool? RequiresAlive => true;

    public void OnPluginStart() { }

    public void OnMapStart() { }

    public void OnServerPrecacheResources(ResourceManifest manifest) { }

    public bool OnEquip(CCSPlayerController player, Dictionary<string, string> item)
    {
        if (!int.TryParse(item["healthValue"], out int healthValue))
            return false;

        if (player.PlayerPawn.Value is not { } playerPawn)
            return false;

        int currentHealth = playerPawn.GetHealth();
        int maxHealth = Config.Settings.MaxHealth;
        int pawnMaxHealth = playerPawn.MaxHealth;

        switch (maxHealth)
        {
            case > 0 when currentHealth >= maxHealth:
            case -1 when currentHealth >= pawnMaxHealth:
                return false;
        }

        int newHealth = currentHealth + healthValue;

        newHealth = maxHealth switch
        {
            > 0 => Math.Min(newHealth, maxHealth),
            -1 => Math.Min(newHealth, pawnMaxHealth),
            _ => newHealth
        };

        player.SetHealth(newHealth);
        return true;
    }

    public bool OnUnequip(CCSPlayerController player, Dictionary<string, string> item, bool update)
    {
        return true;
    }
}