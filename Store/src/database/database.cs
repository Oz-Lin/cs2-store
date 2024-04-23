using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Dapper;
using MySqlConnector;
using static Store.Store;
using static StoreApi.Store;

namespace Store;

public static class Database
{
    public static string GlobalDatabaseConnectionString { get; set; } = string.Empty;
    private static string? equipTableName;

    public static MySqlConnection Connect()
    {
        MySqlConnection connection = new(GlobalDatabaseConnectionString);
        connection.Open();
        return connection;
    }

    public static void Execute(string query, object? parameters)
    {
        using MySqlConnection connection = Connect();
        connection.Execute(query, parameters);
    }

    public static void CreateDatabase(StoreConfig config)
    {
        if (!config.Settings.TryGetValue("database_equip_table_name", out equipTableName))
        {
            equipTableName = "store_equipments_default";
        }

        MySqlConnectionStringBuilder builder = new()
        {
            Server = config.Database["host"],
            Database = config.Database["name"],
            UserID = config.Database["user"],
            Password = config.Database["password"],
            Port = uint.Parse(config.Database["port"]),
            Pooling = true,
            MinimumPoolSize = 0,
            MaximumPoolSize = 640,
            ConnectionIdleTimeout = 30,
            AllowZeroDateTime = true
        };

        GlobalDatabaseConnectionString = builder.ConnectionString;

        _ = Task.Run(async () =>
        {
            using MySqlConnection connection = Connect();
            using MySqlTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                await connection.QueryAsync(@"
                    CREATE TABLE IF NOT EXISTS store_players (
                        id INT NOT NULL AUTO_INCREMENT,
                        SteamID BIGINT UNSIGNED NOT NULL,
                        PlayerName VARCHAR(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
                        Credits INT NOT NULL,
                        DateOfJoin DATETIME NOT NULL,
                        DateOfLastJoin DATETIME NOT NULL,
                        PRIMARY KEY (id),
                        UNIQUE KEY id (id),
                        UNIQUE KEY SteamID (SteamID)
				);", transaction: transaction);

                await connection.QueryAsync(@"
                    CREATE TABLE IF NOT EXISTS store_items (
                        id INT NOT NULL AUTO_INCREMENT,
                        SteamID BIGINT UNSIGNED NOT NULL,
                        Price INT UNSIGNED NOT NULL,
                        Type varchar(16) NOT NULL,
                        UniqueId varchar(256) NOT NULL,
                        DateOfPurchase DATETIME NOT NULL,
                        DateOfExpiration DATETIME NOT NULL,
                        PRIMARY KEY (id)
			    );", transaction: transaction);

                await connection.QueryAsync($@"
                    CREATE TABLE IF NOT EXISTS {equipTableName} (
                        id INT NOT NULL AUTO_INCREMENT,
                        SteamID BIGINT UNSIGNED NOT NULL,
                        Type varchar(16) NOT NULL,
                        UniqueId varchar(256) NOT NULL,
                        Slot INT,
                        PRIMARY KEY (id)
			    );", transaction: transaction);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public static async Task LoadPlayer(CCSPlayerController player)
    {
        Credits.SetOriginal(player, -1);
        Credits.Set(player, -1);

        try
        {
            using MySqlConnection connection = Connect();

            SqlMapper.GridReader multiQuery = await connection.QueryMultipleAsync(@"
                SELECT * FROM store_players WHERE SteamID = @SteamID;
                SELECT * FROM store_items WHERE SteamID = @SteamID AND(DateOfExpiration > @Now OR DateOfExpiration = '0001-01-01 00:00:00');
                SELECT * FROM " + equipTableName + @" WHERE SteamID = @SteamID"
            ,
            new
            {
                player.SteamID,
                DateTime.Now
            });

            Store_Player playerData = await multiQuery.ReadFirstOrDefaultAsync<Store_Player>();

            IEnumerable<Store_Item> items = await multiQuery.ReadAsync<Store_Item>();

            IEnumerable<Store_Equipment> equipments = await multiQuery.ReadAsync<Store_Equipment>();

            Server.NextFrame(() =>
            {
                if (playerData == null)
                {
                    Store_Player newPlayer = new()
                    {
                        SteamID = player.SteamID,
                        PlayerName = player.PlayerName,
                        Credits = Instance.Config.Credits["start"],
                        OriginalCredits = Instance.Config.Credits["start"],
                        DateOfJoin = DateTime.Now,
                        DateOfLastJoin = DateTime.Now,
                        bPlayerIsLoaded = true,
                    };

                    Instance.GlobalStorePlayers.Add(newPlayer);
                    InsertNewPlayer(player);
                }
                else
                {
                    Store_Player? existingPlayer = Instance.GlobalStorePlayers.FirstOrDefault(p => p.SteamID == playerData.SteamID);

                    if (existingPlayer != null)
                    {
                        existingPlayer.PlayerName = playerData.PlayerName;
                        existingPlayer.Credits = Convert.ToInt32(playerData.Credits);
                        existingPlayer.OriginalCredits = existingPlayer.Credits; // ����ͬ��
                        existingPlayer.DateOfJoin = playerData.DateOfJoin;
                        existingPlayer.DateOfLastJoin = playerData.DateOfLastJoin;
                        existingPlayer.bPlayerIsLoaded = true;
                    }
                    else
                    {
                        playerData.OriginalCredits = Convert.ToInt32(playerData.Credits);

                        Instance.GlobalStorePlayers.Add(playerData);
                    }

                }

                foreach (Store_Item newItem in items)
                {
                    Store_Item? existingItem = Instance.GlobalStorePlayerItems.FirstOrDefault(i => i.SteamID == newItem.SteamID && i.UniqueId == newItem.UniqueId && i.Type == newItem.Type);

                    if (existingItem != null)
                    {
                        existingItem.Price = newItem.Price;
                        existingItem.DateOfExpiration = newItem.DateOfExpiration;
                    }
                    else
                    {
                        Instance.GlobalStorePlayerItems.Add(newItem);
                    }
                }

                foreach (Store_Equipment newEquipment in equipments)
                {
                    Store_Equipment? existingEquipment = Instance.GlobalStorePlayerEquipments.FirstOrDefault(e => e.SteamID == newEquipment.SteamID && e.UniqueId == newEquipment.UniqueId);
                    if (existingEquipment != null)
                    {
                        existingEquipment.Type = newEquipment.Type;
                        existingEquipment.Slot = newEquipment.Slot;
                    }
                    else
                    {
                        Instance.GlobalStorePlayerEquipments.Add(newEquipment);
                    }
                }
            });
        }
        catch (Exception)
        {
            Credits.SetOriginal(player, -1);
            Credits.Set(player, -1);
        }
    }

    public static void InsertNewPlayer(CCSPlayerController player)
    {
        Execute(@"
                INSERT INTO store_players (
                    SteamID, PlayerName, Credits, DateOfJoin, DateOfLastJoin
                ) VALUES (
                    @SteamID, @PlayerName, @Credits, @DateOfJoin, @DateOfLastJoin
                );"
            ,
            new
            {
                player.SteamID,
                player.PlayerName,
                Credits = Instance.Config.Credits["start"],
                DateOfJoin = DateTime.Now,
                DateOfLastJoin = DateTime.Now
            });
    }

    public static void SavePlayer(CCSPlayerController player)
    {
        int PlayerCredits = Credits.Get(player);
        int PlayerOriginalCredits = Credits.GetOriginal(player);

        if (PlayerOriginalCredits == -1 || PlayerCredits == -1)
        {
            return;
        }

        int SetCredits = PlayerCredits - PlayerOriginalCredits;

        Execute(@"
            UPDATE
                store_players
            SET
                PlayerName = @PlayerName,
                Credits = GREATEST(Credits + @SetCredits, 0), 
                DateOfJoin = @DateOfJoin, 
                DateOfLastJoin = @DateOfLastJoin
            WHERE
                SteamID = @SteamID;
        ",
            new
            {
                player.PlayerName,
                SetCredits,
                DateOfJoin = DateTime.Now,
                DateOfLastJoin = DateTime.Now,
                SteamId = player.SteamID,
            });

        Credits.SetOriginal(player, PlayerCredits);

    }

    public static void SavePlayerItem(CCSPlayerController player, Store_Item item)
    {
        Execute(@"
            INSERT INTO store_items (
                SteamID, Price, Type, UniqueId, DateOfPurchase, DateOfExpiration
            ) VALUES (
                @SteamID, @Price, @Type, @UniqueId, @DateOfPurchase, @DateOfExpiration
            );
           "
            ,
            new
            {
                player.SteamID,
                item.Price,
                item.Type,
                item.UniqueId,
                item.DateOfPurchase,
                item.DateOfExpiration
            });
    }
    public static void RemovePlayerItem(CCSPlayerController player, Store_Item item)
    {
        Execute(@"
                DELETE
                FROM
                    store_items
                WHERE
                    SteamID = @SteamID AND UniqueId = @UniqueId;
            "
            ,
            new
            {
                player.SteamID,
                item.UniqueId
            });
    }
    public static void SavePlayerEquipment(CCSPlayerController player, Store_Equipment item)
    {
        Execute(@"
                INSERT INTO " + equipTableName + @" (
                    SteamID, Type, UniqueId, Slot
                ) VALUES (
                    @SteamID, @Type, @UniqueId, @Slot
                );
            "
            ,
            new
            {
                player.SteamID,
                item.Type,
                item.UniqueId,
                item.Slot
            });


    }
    public static void RemovePlayerEquipment(CCSPlayerController player, string UniqueId)
    {
        Execute(@"
                    DELETE FROM " + equipTableName + @" WHERE SteamID = @SteamID AND UniqueId = @UniqueId;
                "
            ,
            new
            {
                player.SteamID,
                UniqueId
            });
    }

    public static void ResetPlayer(CCSPlayerController player)
    {
        Execute(@"
            DELETE FROM store_items WHERE SteamID = @SteamID; 
            DELETE FROM " + equipTableName + @" WHERE SteamID = @SteamID
        "
        ,
        new
        {
            player.SteamID
        });
    }

    public static void ResetDatabase()
    {
        using MySqlConnection connection = Connect();

        connection.Query(@"DROP TABLE store_players");
        connection.Query(@"DROP TABLE store_items");
        connection.Query($@"DROP TABLE {equipTableName}");

        Server.ExecuteCommand("_restart");
    }
}