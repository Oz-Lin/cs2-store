# cs2-store

If you want to donate or need a help about plugin, you can contact me in discord private/server

Discord nickname: schwarper

Discord link : [cs2-store discord](https://discord.gg/4zQfUzjk36)

## FAQ

### What is the store plugin?
Store plugin, designed to enhance your gameplay by providing a dynamic credit system that allows you to purchase essential items directly from the store.

### What features are available?
For now, the features are armor, godmode, gravity, health, playerskin, respawn, colored smoke, speed, trail, weapon. You can also add features using the API system. Also New features will also be added in the future based on your feedback.

### What commands are included in the plugin?
The commands can be changed in the config file. For example, by default you can see your credits with !credits. You can change it to !tl, !dollar, !euro if you would like.

```json
"Commands": {
    "credits": [ "credits" ],
    "store": [ "store", "shop", "market" ],
    "inventory": [ "inv", "inventory" ],
    "givecredits": [ "givecredits" ],
    "gift": [ "gift" ],
    "resetplayer": [ "resetplayer" ],
    "resetdatabase": [ "resetdatabase" ]
}
```

### Where should I install the files?
- Begin by downloading the cs2-store plugin and place it in the correct directory within the folder:
  
    `addons/counterstrikesharp/plugins/cs2-store/`

- Copy the StoreApi library files to the following directory:
  
    `addons/counterstrikesharp/shared/StoreApi/`

- To modify the store settings:

    `addons/counterstrikesharp/configs/plugins/cs2-store/cs2-store.json`

## Features

- Armor
- Godmode
- Gravity
- Health
- Player skins
- Respawn
- Colorful smoke
- Speed
- Trail
- Weapon

# Images

![image](https://github.com/schwarper/cs2-store/assets/75811921/d0edc64e-6475-4d04-b5c7-0ea03686d1e6)

![image](https://github.com/schwarper/cs2-store/assets/75811921/a5643eb8-305e-446b-8600-af87976fcbdf)

![image](https://github.com/schwarper/cs2-store/assets/75811921/0893a4f1-333f-4c3e-b126-a8e1f0ec6380)

![image](https://github.com/schwarper/cs2-store/assets/75811921/43652f9f-1ce2-423e-afe4-e13d98ee167a)

![image](https://github.com/schwarper/cs2-store/assets/75811921/212c3139-d2c9-4afe-8c0d-8680d7b5e361)

![image](https://github.com/schwarper/cs2-store/assets/75811921/66b54a97-1b5b-46e2-b838-9a998d65781a)

![image](https://github.com/schwarper/cs2-store/assets/75811921/0db6f827-e2c2-4c7c-9440-9ae97f4225e2)

![image](https://github.com/schwarper/cs2-store/assets/75811921/02a48527-6146-46ea-ae34-83deb8e36ca7)

# Example of json

```json
// This configuration was automatically generated by CounterStrikeSharp for plugin 'cs2-store', at 2024.03.16 11:16:16
{
    "database": {
      "host": "",
      "port": "3306",
      "user": "",
      "password": "",
      "name": ""
    },
    "commands": {
      "credits": [
        "credits",
        "tl"
      ],
      "store": [
        "store",
        "shop",
        "market"
      ],
      "inventory": [
        "inv",
        "inventory"
      ],
      "givecredits": [
        "givecredits"
      ],
      "gift": [
        "gift"
      ],
      "resetplayer": [
        "resetplayer"
      ],
      "resetdatabase": [
        "resetdatabase"
      ]
    },
    "defaultmodels": {
      "ct": [
        "characters/models/ctm_fbi/ctm_fbi_variantb.vmdl"
      ],
      "t": [
        "characters/models/tm_leet/tm_leet_variantj.vmdl"
      ]
    },
    "credits": {
      "ignore_warmup": 1,
      "start": 0,
      "interval_active_inactive": 60,
      "amount_active": 10,
      "amount_inactive": 1,
      "amount_kill": 1
    },
    "menu": {
      "enable_selling": "1",
      "vip_flag": "@css/root"
    },
    "items": {
        "playerskins": {
            "1": {
                "Name": "Mr. Muhlik",
                "UniqueId": "characters/models/tm_leet/tm_leet_variantj.vmdl",
                "Type": "playerskin",
                "Price": 0,
                "Team": 2,
                "Slot": 2,
                "Enable": true
            },
            "2": {
                "Name": "Number K",
                "UniqueId": "characters/models/tm_professional/tm_professional_vari.vmdl",
                "Type": "playerskin",
                "Price": 0,
                "Team": 2,
                "Slot": 2,
                "Enable": true
            },
            "3": {
                "Name": "Operator",
                "UniqueId": "characters/models/ctm_fbi/ctm_fbi_variantf.vmdl",
                "Type": "playerskin",
                "Price": 0,
                "Team": 3,
                "Slot": 3,
                "Enable": true
            },
            "4": {
                "Name": "Blueberries",
                "UniqueId": "characters/models/ctm_st6/ctm_st6_variantj.vmdl",
                "Type": "playerskin",
                "Price": 0,
                "Team": 3,
                "Slot": 3,
                "Enable": true
            }
        },
        "weapons": {
            "1": {
                "Name": "AK47",
                "UniqueId": "weapon_ak47",
                "Type": "weapon",
                "Price": 0,
                "Enable": true
            },
            "2": {
                "Name": "M4A1",
                "UniqueId": "weapon_m4a1",
                "Type": "weapon",
                "Price": 0,
                "Enable": true
            }
        },
        "features": {
            "1": {
                "Name": "Respawn",
                "UniqueId": "respawn",
                "Type": "respawn",
                "Price": 0,
                "Enable": true
            },
            "2": {
                "Name": "50 HP",
                "UniqueId": "50",
                "Type": "health",
                "Price": 0,
                "Enable": true
            },
            "3": {
                "Name": "10 Armor",
                "UniqueId": "10",
                "Type": "armor",
                "Price": 0,
                "Enable": true
            },
            "4": {
              	"Name": "Colored Skin",
                "UniqueId": "coloredskin",
                "Type": "coloredskin",
                "Price": 0,
                "Enable": true
            }
        },
        "gravity": {
            "1": {
                "Name": "100",
                "UniqueId": "100",
                "Type": "gravity",
                "Price": 0,
                "Enable": true
            },
            "2": {
                "Name": "200",
                "UniqueId": "200",
                "Type": "gravity",
                "Price": 0,
                "Enable": true
            },
            "3": {
                "Name": "300",
                "UniqueId": "300",
                "Type": "gravity",
                "Price": 0,
                "Enable": true
            }
        },
        "speed": {
            "1": {
                "Name": "5 Sec",
                "UniqueId": "5",
                "Type": "speed",
                "Price": 0,
                "Enable": true
            },
            "2": {
                "Name": "10 Sec",
                "UniqueId": "10",
                "Type": "speed",
                "Price": 0,
                "Enable": true
            },
            "3": {
                "Name": "60 Sec",
                "UniqueId": "60",
                "Type": "speed",
                "Price": 0,
                "Enable": true
            }
        },
        "godmode": {
            "1": {
                "Name": "5 Sec",
                "UniqueId": "5",
                "Type": "godmode",
                "Price": 0,
                "Enable": true
            },
            "2": {
                "Name": "10 Sec",
                "UniqueId": "10",
                "Type": "godmode",
                "Price": 0,
                "Enable": true
            },
            "3": {
                "Name": "60 Sec",
                "UniqueId": "60",
                "Type": "godmode",
                "Price": 0,
                "Enable": true
            }
        },
        "smoke": {
            "1": {
                "Name": "Random",
                "UniqueId": "colorsmoke",
                "Type": "smoke",
                "Price": 0,
                "Slot": 1,
                "Enable": true
            },
            "2": {
                "Name": "Red",
                "UniqueId": "redsmoke",
                "Type": "smoke",
                "Color": "255 0 0",
                "Price": 0,
                "Slot": 1,
                "Enable": true
            },
            "3": {
                "Name": "Green",
                "UniqueId": "redsmoke",
                "Type": "smoke",
                "Color": "0 255 0",
                "Price": 0,
                "Slot": 1,
                "Enable": true
            },
            "4": {
                "Name": "Blue",
                "UniqueId": "redsmoke",
                "Type": "smoke",
                "Color": "0 0 255",
                "Price": 0,
                "Slot": 1,
                "Enable": true
            }
        },
        "trail": {
            "1": {
                "Name": "Energycirc",
                "UniqueId": "particles/ui/status_levels/ui_status_level_8_energycirc.vpcf",
                "Type": "trail",
                "Price": 0,
                "Slot": 1,
                "Enable": true
            }
        }
    },
    "ConfigVersion": 1
}
```
