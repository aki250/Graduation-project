using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseMgr
{
    private static DatabaseMgr instance;
    public static DatabaseMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DatabaseMgr();
            }
            return instance;
        }
    }

    private MySqlConnection conn;
    private int gameID = -1;
    private int settingID = -1;


    private DatabaseMgr() 
    {
        conn = new MySqlConnection("server=localhost;User Id = Unity_; password = 123456;Database = akigame; Charset = utf8mb4; Pooling = true;");
        //conn = new MySqlConnection("Server=127.0.0.1;database=darkgod;user=root;password=;Charset = utf8mb4;");//SslMode=none;
        conn.Open();
    }

    #region 设置数据
    public SettingsData QuerySetting()
    {
        SettingsData settingsData = null;
        MySqlDataReader reader = null;
        bool isNew = true;
        try
        {
            MySqlCommand cmd = new MySqlCommand("select * from setting", conn);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isNew = false;
                settingsData = new SettingsData();

                settingID = reader.GetInt32("id");
                string volumeStr = reader.GetString("volume");
                TextUtil.SplitStrTwice(volumeStr, E_SymbolType.VerBar, E_SymbolType.WellNum, (str1, str2) =>
                {
                    settingsData.volumeSettingsDictionary.Add(str1, float.Parse(str2));
                });

                string keyStr = reader.GetString("keybinds");
                TextUtil.SplitStrTwice(keyStr, E_SymbolType.VerBar, E_SymbolType.WellNum, (str1, str2) =>
                {
                    if(!settingsData.keybindsDictionary.ContainsKey(str1))
                    {
                        settingsData.keybindsDictionary.Add(str1, (KeyCode)int.Parse(str2));
                    }
                    else
                    {
                        settingsData.keybindsDictionary[str1] = (KeyCode)int.Parse(str2);
                    }
                });

                string playsetStr = reader.GetString("playset");
                TextUtil.SplitStrTwice(playsetStr, E_SymbolType.VerBar, E_SymbolType.WellNum, (str1, str2) =>
                {
                    settingsData.gameplayToggleSettingsDictionary.Add(str1, int.Parse(str2) == 0 ? false : true);
                });

                settingsData.localeID = reader.GetInt32("language");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Query SettingsData Error:" + e.ToString());
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
            if (isNew)
            {
                settingsData = new SettingsData();
                settingID = InsertNewSettingsData(settingsData);
            }
        }

        return settingsData;
    }

    private int InsertNewSettingsData(SettingsData sd)
    {
        int id = -1;
        try
        {
            MySqlCommand cmd = new MySqlCommand("insert into setting set volume = @volume, keybinds = @keybinds, playset = @playset, language = @language", conn);
            
            string volume = "";
            foreach(KeyValuePair<string, float> item in sd.volumeSettingsDictionary)
            {
                volume += item.Key + "#" + item.Value + "|";
            }
            if(volume.Length > 0)
                volume = volume.Substring(0, volume.Length - 1);

            string keybinds = "";
            foreach (KeyValuePair<string, KeyCode> item in sd.keybindsDictionary)
            {
                keybinds += item.Key + "#" + ((int)item.Value) + "|";
            }
            if(keybinds.Length > 0)
                keybinds = keybinds.Substring(0, keybinds.Length - 1);

            string playset = "";
            foreach (KeyValuePair<string, bool> item in sd.gameplayToggleSettingsDictionary)
            {
                playset += item.Key + "#" + (item.Value?1:0) + "|";
            }
            if(playset.Length > 0)
                playset = playset.Substring(0, playset.Length - 1);

            cmd.Parameters.AddWithValue("volume", volume);
            cmd.Parameters.AddWithValue("keybinds", keybinds);
            cmd.Parameters.AddWithValue("playset", playset);
            cmd.Parameters.AddWithValue("language", sd.localeID);
            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;
        }
        catch (Exception e)
        {
            Debug.LogError("Insert SettingsData Error:" + e);
        }
        return id;
    }

    public bool UpdateSettingsData(SettingsData sd)
    {
        if(settingID == -1)
        {
            QuerySetting();
        }

        if (settingID == -1)
        {
            Debug.LogError("数据未存先读");
            return false;
        }

        try
        {
            MySqlCommand cmd = new MySqlCommand("update setting set volume = @volume, keybinds = @keybinds, playset = @playset, language = @language" +
                 " where id = @id", conn);

            string volume = "";
            foreach (KeyValuePair<string, float> item in sd.volumeSettingsDictionary)
            {
                volume += item.Key + "#" + item.Value + "|";
            }
            if (volume.Length > 0)
                volume = volume.Substring(0, volume.Length - 1);

            string keybinds = "";
            foreach (KeyValuePair<string, KeyCode> item in sd.keybindsDictionary)
            {
                keybinds += item.Key + "#" + ((int)item.Value) + "|";
            }
            if (keybinds.Length > 0)
                keybinds = keybinds.Substring(0, keybinds.Length - 1);

            string playset = "";
            foreach (KeyValuePair<string, bool> item in sd.gameplayToggleSettingsDictionary)
            {
                playset += item.Key + "#" + (item.Value ? 1 : 0) + "|";
            }
            if (playset.Length > 0)
                playset = playset.Substring(0, playset.Length - 1);

            cmd.Parameters.AddWithValue("volume", volume);
            cmd.Parameters.AddWithValue("keybinds", keybinds);
            cmd.Parameters.AddWithValue("playset", playset);
            cmd.Parameters.AddWithValue("language", sd.localeID);
            cmd.Parameters.AddWithValue("id", settingID);
            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return false;
        }
        return true;
    }

    #endregion

    #region 游戏数据
    public GameData QueryGame()
    {
        GameData gameData = null;
        MySqlDataReader reader = null;
        bool isNew = true;
        try
        {
            MySqlCommand cmd = new MySqlCommand("select * from game", conn);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                isNew = false;
                gameData = new GameData();

                gameID = reader.GetInt32("id");
                gameData.currecny = reader.GetInt32("currecny");

                string skillStr = reader.GetString("skill");
                if(skillStr.Length > 1)
                {
                    TextUtil.SplitStrTwice(skillStr, E_SymbolType.VerBar, E_SymbolType.WellNum, (str1, str2) =>
                    {
                        gameData.skillTree.Add(str1, int.Parse(str2) == 0 ? false : true);
                    });
                }

                string inventoryStr = reader.GetString("inventory");
                if(inventoryStr.Length > 1)
                {
                    TextUtil.SplitStrTwice(inventoryStr, E_SymbolType.VerBar, E_SymbolType.WellNum, (str1, str2) =>
                    {
                        gameData.inventory.Add(str1, int.Parse(str2));
                    });
                }
                
                string equipidStr = reader.GetString("equipid");
                string[] equipidStrArr = TextUtil.SplitStr(equipidStr, E_SymbolType.VerBar);
                for (int i = 0; i < equipidStrArr.Length; i++)
                {
                    if(!string.IsNullOrEmpty(equipidStrArr[i]))
                    {
                        gameData.equippedEquipmentIDs.Add(equipidStrArr[i]);
                    }
                }

                string checkpointStr = reader.GetString("checkpoint");
                if(checkpointStr.Length > 1)
                {
                    TextUtil.SplitStrTwice(checkpointStr, E_SymbolType.VerBar, E_SymbolType.WellNum, (str1, str2) =>
                    {
                        gameData.checkpointsDictionary.Add(str1, int.Parse(str2) == 0 ? false : true);
                    });
                }

                if(!reader.GetString("closepoint").Equals("|"))
                    gameData.closestActivatedCheckpointID = reader.GetString("closepoint");

                if (!reader.GetString("lastpoint").Equals("|"))
                    gameData.lastActivatedCheckpointID = reader.GetString("lastpoint");

                gameData.droppedCurrencyAmount = reader.GetInt32("dropamount");

                string deathpointStr = reader.GetString("deathpoint");
                string[] deathpointStrArr = TextUtil.SplitStr(deathpointStr, E_SymbolType.VerBar);
                if(deathpointStrArr.Length >= 2)
                {
                    gameData.deathPosition = new Vector2(float.Parse(deathpointStrArr[0]), float.Parse(deathpointStrArr[1]));
                }

                string maplistStr = reader.GetString("maplist");
                if(!maplistStr.Equals("|"))
                {
                    int[] maplistArr = TextUtil.SplitStrToIntArr(maplistStr, E_SymbolType.VerBar);
                    for (int i = 0; i < maplistArr.Length; i++)
                    {
                        gameData.UsedMapElementIDList.Add(maplistArr[i]);
                    }
                }

                gameData.isNew = reader.GetInt32("newplayer") == 0 ? false:true;

            }
        }
        catch (Exception e)
        {
            Debug.Log("Query GameData Error:" + e.ToString());
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
            if (isNew)
            {
                gameData = new GameData();
                gameID = InsertNewGameData(gameData);
            }
        }

        return gameData;
    }

    private int InsertNewGameData(GameData gd)
    {
        int id = -1;
        try
        {
            MySqlCommand cmd = new MySqlCommand("insert into game set currecny = @currecny, skill = @skill, inventory = @inventory, equipid = @equipid, " +
                "checkpoint = @checkpoint, closepoint = @closepoint, lastpoint = @lastpoint, dropamount = @dropamount, deathpoint = @deathpoint, maplist = @maplist, newplayer = @newplayer", conn);

            string skill = "";
            foreach (KeyValuePair<string, bool> item in gd.skillTree)
            {
                skill += item.Key + "#" + (item.Value ? 1 : 0) + "|";
            }
            if (skill.Length > 0)
                skill = skill.Substring(0, skill.Length - 1);
            else
                skill = "|";

            string inventory = "";
            foreach (KeyValuePair<string, int> item in gd.inventory)
            {
                inventory += item.Key + "#" + item.Value + "|";
            }
            if (inventory.Length > 0)
                inventory = inventory.Substring(0, inventory.Length - 1);
            else
                inventory = "|";

            string equipid = "";
            foreach(string item in gd.equippedEquipmentIDs)
            {
                equipid += item + "|";
            }
            if (equipid.Length > 0)
                equipid = equipid.Substring(0, equipid.Length - 1);
            else
                equipid = "|";

            string checkpoint = "";
            foreach (KeyValuePair<string, bool> item in gd.checkpointsDictionary)
            {
                checkpoint += item.Key + "#" + (item.Value ? 1 : 0) + "|";
            }
            if (checkpoint.Length > 0)
                checkpoint = checkpoint.Substring(0, checkpoint.Length - 1);
            else
                checkpoint = "|";

            string deathpoint = gd.deathPosition.x + "|" + gd.deathPosition.y;

            string maplist = "";
            foreach (int item in gd.UsedMapElementIDList)
            {
                maplist += item + "|";
            }
            if (maplist.Length > 0)
                maplist = maplist.Substring(0, maplist.Length - 1);
            else
                maplist = "|";


            string closepoint = gd.closestActivatedCheckpointID;
            if (string.IsNullOrEmpty(closepoint))
            {
                closepoint = "|";
            }

            string lastpoint = gd.lastActivatedCheckpointID;
            if (string.IsNullOrEmpty(lastpoint))
            {
                lastpoint = "|";
            }

            cmd.Parameters.AddWithValue("currecny", gd.currecny);
            cmd.Parameters.AddWithValue("skill", skill);
            cmd.Parameters.AddWithValue("inventory", inventory);
            cmd.Parameters.AddWithValue("equipid", equipid);
            cmd.Parameters.AddWithValue("checkpoint", checkpoint);
            cmd.Parameters.AddWithValue("closepoint", closepoint);
            cmd.Parameters.AddWithValue("lastpoint", lastpoint);
            cmd.Parameters.AddWithValue("dropamount", gd.droppedCurrencyAmount);
            cmd.Parameters.AddWithValue("deathpoint", deathpoint);
            cmd.Parameters.AddWithValue("maplist", maplist);
            cmd.Parameters.AddWithValue("newplayer", gd.isNew?1:0);
            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;
        }
        catch (Exception e)
        {
            Debug.LogError("Insert GameData Error:" + e);
        }
        return id;
    }

    public bool UpdateGameData(GameData gd)
    {
        if (gameID == -1)
        {
            QueryGame();
        }

        if (gameID == -1)
        {
            Debug.LogError("数据未存先读");
            return false;
        }

        try
        {
            MySqlCommand cmd = new MySqlCommand("update game set currecny = @currecny, skill = @skill, inventory = @inventory, equipid = @equipid, " +
                "checkpoint = @checkpoint, closepoint = @closepoint, lastpoint = @lastpoint, dropamount = @dropamount, deathpoint = @deathpoint, maplist = @maplist, newplayer = @newplayer" +
                 " where id = @id", conn);

            string skill = "";
            foreach (KeyValuePair<string, bool> item in gd.skillTree)
            {
                skill += item.Key + "#" + (item.Value ? 1 : 0) + "|";
            }
            if (skill.Length > 0)
                skill = skill.Substring(0, skill.Length - 1);
            else
                skill = "|";

            string inventory = "";
            foreach (KeyValuePair<string, int> item in gd.inventory)
            {
                inventory += item.Key + "#" + item.Value + "|";
            }
            if (inventory.Length > 0)
                inventory = inventory.Substring(0, inventory.Length - 1);
            else
                inventory = "|";

            string equipid = "";
            foreach (string item in gd.equippedEquipmentIDs)
            {
                equipid += item + "|";
            }
            if (equipid.Length > 0)
                equipid = equipid.Substring(0, equipid.Length - 1);
            else
                equipid = "|";

            string checkpoint = "";
            foreach (KeyValuePair<string, bool> item in gd.checkpointsDictionary)
            {
                checkpoint += item.Key + "#" + (item.Value ? 1 : 0) + "|";
            }
            if (checkpoint.Length > 0)
                checkpoint = checkpoint.Substring(0, checkpoint.Length - 1);
            else
                checkpoint = "|";

            string deathpoint = gd.deathPosition.x + "|" + gd.deathPosition.y;

            string maplist = "";
            foreach (int item in gd.UsedMapElementIDList)
            {
                maplist += item + "|";
            }
            if (maplist.Length > 0)
                maplist = maplist.Substring(0, maplist.Length - 1);
            else
                maplist = "|";


            string closepoint = gd.closestActivatedCheckpointID;
            if (string.IsNullOrEmpty(closepoint))
            {
                closepoint = "|";
            }

            string lastpoint = gd.lastActivatedCheckpointID;
            if (string.IsNullOrEmpty(lastpoint))
            {
                lastpoint = "|";
            }

            cmd.Parameters.AddWithValue("currecny", gd.currecny);
            cmd.Parameters.AddWithValue("skill", skill);
            cmd.Parameters.AddWithValue("inventory", inventory);
            cmd.Parameters.AddWithValue("equipid", equipid);
            cmd.Parameters.AddWithValue("checkpoint", checkpoint);
            cmd.Parameters.AddWithValue("closepoint", closepoint);
            cmd.Parameters.AddWithValue("lastpoint", lastpoint);
            cmd.Parameters.AddWithValue("dropamount", gd.droppedCurrencyAmount);
            cmd.Parameters.AddWithValue("deathpoint", deathpoint);
            cmd.Parameters.AddWithValue("maplist", maplist);
            cmd.Parameters.AddWithValue("newplayer", 0);
            cmd.Parameters.AddWithValue("id", gameID);
            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Debug.LogError("update GameData Error:" + e);
            return false;
        }
        return true;
    }

    public void DeleteGameData()
    {
        if (gameID == -1)
        {
            QueryGame();
        }

        if (gameID == -1)
        {
            Debug.LogError("数据未存先读");
            return;
        }

        try
        {
            MySqlCommand cmd = new MySqlCommand("delete from game where id = @id", conn);
            cmd.Parameters.AddWithValue("id", gameID);
            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Debug.LogError("Delete GameData Error:" + e);
        }
    }
    #endregion
}
