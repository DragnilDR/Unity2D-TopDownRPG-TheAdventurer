using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

[System.Serializable]
public class GameData
{
    [System.Serializable]
    public struct Vec3
    {
        public float x, y, z;

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    [System.Serializable]
    public struct PlayerStatsData
    {
        public Stats Stats;
        public Player.AttackType AttakType;

        public PlayerStatsData(Stats stats, Player.AttackType attakType)
        {
            Stats = stats;
            AttakType = attakType;
        }
    }

    public PlayerStatsData playerStatsData;

    public void SavePlayerStats(Stats stats, Player.AttackType attakType)
    {
        playerStatsData = new PlayerStatsData(stats, attakType);
    }

    [System.Serializable]
    public struct PlayerPositionData
    {
        public Vec3 Position;
        
        public PlayerPositionData(Vec3 pos)
        {
            Position = pos;
        }
    }

    public PlayerPositionData playerPositionData;

    public void SavePlayerPosition(Vector3 playerPosition)
    {
        playerPositionData = new PlayerPositionData(new Vec3(playerPosition.x, playerPosition.y, playerPosition.z));
    }

    [System.Serializable]
    public struct QuestSaveData
    {
        public string QuestName;
        public int QuestStageNumber;
        public int Count;
        public QuestSlot.QuestStage QuestStage;
        
        public bool IsTracking;
        
        public QuestSaveData(string questName, int questStageNumber, int count, QuestSlot.QuestStage questStage, bool isTracking)
        {
            QuestName = questName;
            QuestStageNumber = questStageNumber;
            Count = count;
            QuestStage = questStage;
            
            IsTracking = isTracking;
        }
    }

    public List<QuestSaveData> questSaveData = new();

    public void SaveQuest(List<QuestSlot> quests)
    {
        foreach (var quest in quests)
        {
            questSaveData.Add(new QuestSaveData(quest.questInfo.questName, quest.questGoalNumber, quest.count, quest.questStage, quest.isTracking));
        }
    }
    
    [System.Serializable]
    public struct QuestBoardSaveData
    {
        public string QuestName;
        public QuestSlot.QuestStage QuestStage;

        public QuestBoardSaveData(string questName, QuestSlot.QuestStage questStage)
        {
            QuestName = questName;
            QuestStage = questStage;
        }
    }

    public List<QuestBoardSaveData> questBoardSaveDatas = new();

    public void SaveQuestsInBoard(List<QuestSlot> quests)
    {
        foreach (var quest in quests)
        {
            questBoardSaveDatas.Add(new QuestBoardSaveData(quest.questInfo.questName, quest.questStage));
        }
    }

    [System.Serializable]
    public struct InventorySaveData
    {
        public string ItemName;
        public int Count;
        public bool IsEquiped;

        public InventorySaveData(string itemName, int count, bool isEquiped)
        {
            ItemName = itemName;
            Count = count;
            IsEquiped = isEquiped;
        }
    }

    public List<InventorySaveData> inventorySaveData = new();

    public void SaveInventory(List<InventorySlot> inventory)
    {
        foreach (var item in inventory)
        {
            inventorySaveData.Add(new InventorySaveData(item.item.name, item.count, item.isEquiped));
        }
    }

    [System.Serializable]
    public struct ItemSaveData
    {
        public string ItemName;
        public int Count;
        public Vec3 Position;

        public ItemSaveData(string itemName, int count, Vec3 position)
        {
            ItemName = itemName;
            Count = count;
            Position = position;
        }
    }

    public List<ItemSaveData> itemSaveData = new();

    public void SaveItem(List<GameObject> items)
    {
        foreach (var item in items)
        {
            Vec3 pos = new(item.transform.position.x, item.transform.position.y, item.transform.position.z);

            itemSaveData.Add(new ItemSaveData(item.GetComponent<ItemPickup>().inventorySlot.item.itemName, item.GetComponent<ItemPickup>().inventorySlot.count, pos));
        }
    }

    [System.Serializable]
    public struct RankSaveData
    {
        public int Exp;
        public string Rank;
        public int CurrentLevel;

        public RankSaveData(int exp, string rank, int currentLevel)
        {
            Exp = exp;
            Rank = rank;
            CurrentLevel = currentLevel;
        }
    }

    public RankSaveData rankSaveData;

    public void SaveRank(RankSystem rankSystem)
    {
        rankSaveData = new RankSaveData(rankSystem.currentExp, rankSystem.currentRank, rankSystem.currentLevel);
    }

    [System.Serializable]
    public struct SettingsSaveData
    {
        public float MusicVolume;
        public float SoundEffectVolume;

        public SettingsSaveData(float musicVolume, float soundEffectVolume)
        {
            MusicVolume = musicVolume;
            SoundEffectVolume = soundEffectVolume;
        }
    }

    public SettingsSaveData settingsSaveData;

    public void SaveSettings(GameSettings gameSettings)
    {
        settingsSaveData = new SettingsSaveData(gameSettings.musicSlider.value, gameSettings.soundEffectSlider.value);
    }

    [System.Serializable]
    public struct TimeSaveData
    {
        public int Hours;
        public int Minutes;

        public TimeSaveData(int hours, int minutes)
        {
            Hours = hours;
            Minutes = minutes;
        }
    }

    public TimeSaveData timeSaveData;

    public void SaveTime(DayNightCycle dayNightCycle)
    {
        timeSaveData = new TimeSaveData(dayNightCycle.hours, dayNightCycle.minutes);
    }

    public string Projectile;
    public string activeSceneName;
}

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    private string filePath;
    private readonly string fileName = "save.gg";

    private Player player;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        player = FindAnyObjectByType<Player>();
    }

    private void OnEnable()
    {
        Menu.OnSaveGame += SaveGame;
        Menu.OnLoadGame += LoadGame;
        Loader.OnLoadGameSettings += LoadGameSettings;
        Loader.OnLoadGame += LoadGame;
        Player.OnDeleteSave += DeleteSave;
        Menu.OnDeleteSave += DeleteSave;
    }

    private void OnDisable()
    {
        Menu.OnSaveGame -= SaveGame;
        Menu.OnLoadGame -= LoadGame;
        Loader.OnLoadGameSettings -= LoadGameSettings;
        Loader.OnLoadGame -= LoadGame;
        Player.OnDeleteSave -= DeleteSave;
        Menu.OnDeleteSave -= DeleteSave;
    }

    public void SaveGameSettings()
    {
        GameData gameData = new();
        filePath = Application.persistentDataPath + "/" + "settings.st";
        SaveSettings(gameData);

        BinaryFormatter binaryFormatter = new();
        FileStream fileStream = new(filePath, FileMode.Create);
        binaryFormatter.Serialize(fileStream, gameData);
        fileStream.Close();
    }

    public void SaveGame()
    {
        GameData gameData = new();
        filePath = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + fileName;

        SavePlayerPosition(gameData);
        SaveQuestsInBoard(gameData);
        SaveItems(gameData);
        
        BinaryFormatter binaryFormatter = new();
        FileStream fileStream = new(filePath, FileMode.Create);
        binaryFormatter.Serialize(fileStream, gameData);
        fileStream.Close();

        filePath = Application.persistentDataPath + "/" + fileName;
        SaveActiveScene(gameData);
        SavePlayerStats(gameData);
        SaveInventory(gameData);
        SaveQuests(gameData);
        SaveRank(gameData);
        SaveProjectile(gameData);
        SaveTime(gameData);

        FileStream fileStream2 = new(filePath, FileMode.Create);

        binaryFormatter.Serialize(fileStream2, gameData);
        fileStream2.Close();
    }

    public void SaveActiveScene(GameData gameData)
    {
        gameData.activeSceneName = SceneManager.GetActiveScene().name;
    }

    public void SaveSettings(GameData gameDate)
    {
        gameDate.SaveSettings(GameSettings.Instance);
    }

    public void SaveTime(GameData gameData)
    {
        gameData.SaveTime(DayNightCycle.Instance);
    }

    public void SavePlayerStats(GameData gameData)
    {
        gameData.SavePlayerStats(player.stats, player.attackType);
    }

    public void SaveProjectile(GameData gameData)
    {
        if (player.projectile != null)
        {
            gameData.Projectile = player.projectile.name;
        }
    }

    public void SavePlayerPosition(GameData gameData)
    {
        if (player != null)
        {
            gameData.SavePlayerPosition(player.transform.position);
        }
    }

    public void SaveInventory(GameData gameData)
    {
        gameData.SaveInventory(Inventory.Instance.items);
    }

    public void SaveQuests(GameData gameData)
    {
        gameData.SaveQuest(QuestList.Instance.questList);
    }

    public void SaveQuestsInBoard(GameData gameData)
    {
        QuestManager questManager = FindObjectOfType<QuestManager>();
        
        gameData.SaveQuestsInBoard(questManager.questSlot);
    }

    public void SaveRank(GameData gameData)
    {
        gameData.SaveRank(RankSystem.Instance);
    }

    public void SaveItems(GameData gameData)
    {
        ItemPickup[] itemsOfComponent = FindObjectsOfType<ItemPickup>();
        List<GameObject> items = new();
        foreach (var item in itemsOfComponent)
        {
            items.Add(item.gameObject);
        }
        gameData.SaveItem(items);
    }

    public void LoadGameSettings()
    {
        filePath = Application.persistentDataPath + "/" + "settings.st";
        if (File.Exists(filePath))
        {
            BinaryFormatter binaryFormatter = new();
            FileStream fileStream = new(filePath, FileMode.Open);
            GameData gameData = (GameData)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();

            LoadSettings(gameData);
        }
    }

    public void LoadGame()
    {
        filePath = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + fileName;
        if (File.Exists(filePath))
        {
            BinaryFormatter binaryFormatter = new();
            FileStream fileStream = new(filePath, FileMode.Open);
            GameData gameData = (GameData)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();

            LoadPlayerPosition(gameData);
            LoadQuestInBoard(gameData);
            LoadItems(gameData);
        }

        filePath = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(filePath))
        {
            BinaryFormatter binaryFormatter = new();
            FileStream fileStream = new(filePath, FileMode.Open);
            GameData gameData = (GameData)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();

            LoadPlayerStats(gameData);
            LoadInventory(gameData);
            LoadQuests(gameData);
            LoadRank(gameData);
            LoadProjectile(gameData);
            LoadTime(gameData);
        }
    }

    public string LoadSaveScene()
    {
        filePath = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(filePath))
        {
            BinaryFormatter binaryFormatter = new();
            FileStream fileStream = new(filePath, FileMode.Open);
            GameData gameData = (GameData)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();

            return gameData.activeSceneName;
        }
        else
            return null;
    }

    public void LoadSettings(GameData gameData)
    {
        GameSettings.Instance.musicSlider.value = gameData.settingsSaveData.MusicVolume;
        GameSettings.Instance.soundEffectSlider.value = gameData.settingsSaveData.SoundEffectVolume;
    }

    public void LoadTime(GameData gameData)
    {
        DayNightCycle.Instance.hours = gameData.timeSaveData.Hours;
        DayNightCycle.Instance.minutes = gameData.timeSaveData.Minutes;

        DayNightCycle.Instance.UpdateCurrentTimeUI();
        DayNightCycle.Instance.UpdateLightIntensity();
    }

    public void LoadPlayerStats(GameData gameData)
    {
        if (player != null)
        {
            player.stats = gameData.playerStatsData.Stats;
            player.attackType = gameData.playerStatsData.AttakType;
        }
    }

    public void LoadProjectile(GameData gameData)
    {
        player.projectile = (GameObject)Resources.Load(gameData.Projectile);
    }

    public void LoadPlayerPosition(GameData gameData)
    {
        if (player != null)
        {
            player.transform.position = new Vector3(gameData.playerPositionData.Position.x, gameData.playerPositionData.Position.y, gameData.playerPositionData.Position.z);
        }
    }

    public void LoadInventory(GameData gameData)
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.items.Clear();
            foreach (var itemData in gameData.inventorySaveData)
            {
                ItemInfoSO itemInfo = FindItemInfoByName(itemData.ItemName);

                if (itemInfo != null)
                {
                    InventorySlot inventorySlot = new()
                    {
                        item = itemInfo,
                        count = itemData.Count,
                        isEquiped = itemData.IsEquiped
                    };

                    Inventory.Instance.AddItem(inventorySlot);
                }
            }
        }
    }

    public void LoadQuests(GameData gameData)
    {
        if (QuestList.Instance != null)
        {
            QuestList.Instance.questList.Clear();
            foreach (var questData in gameData.questSaveData)
            {
                QuestInfoSO questInfo = FindQuestInfoByName(questData.QuestName); // Найдите QuestInfoSO по его имени
                
                if (questInfo != null)
                {
                    QuestSlot questSlot = new()
                    {
                        questInfo = questInfo,
                        questGoalNumber = questData.QuestStageNumber,
                        count = questData.Count,
                        questStage = questData.QuestStage,
                        
                        isTracking = questData.IsTracking
                    };

                    // Добавьте квест в список квестов.
                    QuestList.Instance.questList.Add(questSlot);
                }
            }
        }
    }

    public void LoadQuestInBoard(GameData gameData)
    {
        QuestManager questManager = FindObjectOfType<QuestManager>();
        questManager.questSlot.Clear();

        foreach (var questData in gameData.questBoardSaveDatas)
        {
            QuestInfoSO questInfo = FindQuestInfoByName(questData.QuestName);

            if (questInfo != null)
            {
                QuestSlot questSlot = new()
                {
                    questInfo = questInfo,
                    questStage = questData.QuestStage
                };

                questManager.questSlot.Add(questSlot);
            }
        }
    }

    public void LoadRank(GameData gameData)
    {
        if (RankSystem.Instance != null)
        {
            RankSystem.Instance.currentExp = gameData.rankSaveData.Exp;
            RankSystem.Instance.currentRank = gameData.rankSaveData.Rank;
            RankSystem.Instance.currentLevel = gameData.rankSaveData.CurrentLevel;
        }
    }

    public void LoadItems(GameData gameData)
    {
        ItemPickup[] itemsOfComponent = FindObjectsOfType<ItemPickup>();
        if (itemsOfComponent != null)
        {
            foreach (var item in itemsOfComponent)
            {
                Destroy(item.gameObject);
            }

            foreach (var item in gameData.itemSaveData)
            {
                Vector3 itemPos = new(item.Position.x, item.Position.y, item.Position.z);
                GameObject itemGameObject = Instantiate((GameObject)Resources.Load("Item"), itemPos, Quaternion.identity);
                itemGameObject.GetComponent<ItemPickup>().inventorySlot.item = FindItemInfoByName(item.ItemName);
                itemGameObject.GetComponent<ItemPickup>().inventorySlot.count = item.Count;
            }
        }
    }

    private void DeleteSave()
    {
        string[] saveFiles = Directory.GetFiles(Application.persistentDataPath, "*.gg");

        foreach (string file in saveFiles)
        {
            try
            {
                File.Delete(file);
                Debug.Log("Deleted file: " + file);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to delete file: " + file + "\n" + e.Message);
            }
        }
    }

    private QuestInfoSO FindQuestInfoByName(string questName)
    {
        // Здесь вы можете найти QuestInfoSO по его имени из доступных ресурсов.
        // Например, можно использовать Resources.Load, чтобы найти ресурс по имени.
        // Обратите внимание, что это предполагает, что QuestInfoSO находятся в Resources папке.
        // Подставьте свою логику поиска на основе вашей организации данных.

        // Пример:
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("QuestInfo"); // Здесь "QuestInfo" - это имя папки Resources
        foreach (var quest in allQuests)
        {
            if (quest.questName == questName)
            {
                return quest;
            }
        }

        // Если не найдено, вернуть null или обработать случай отсутствия QuestInfoSO.
        return null;
    }

    private ItemInfoSO FindItemInfoByName(string itemName)
    {
        ItemInfoSO[] allItems = Resources.LoadAll<ItemInfoSO>("ItemInfo");
        foreach (var item
            in allItems)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }

        return null;
    }
}
