using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class GameConfigs {
    public bool wishMusicOn;
    public bool wishSfxOn;
}

public class PlayerPersistence : MonoBehaviour {

    private static string FILE_PATH = Application.persistentDataPath + "/PlayerData.dat";

    /// <summary>
    /// Saves player's data to a local file
    /// </summary>
    /// <param name="lastLevel">The last level played</param>
    /// <param name="scoreMultiplier">The current score multiplier gotten in the last quiz</param>
    /// <param name="hearts">How many hearts he has available</param>
    public static void SavePlayerData(float scoreMultiplier, int hearts) {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(FILE_PATH);

        PlayerData playerData = new PlayerData();
        playerData.ScoreMultiplier = scoreMultiplier;
        playerData.Hearts = hearts;

        binaryFormatter.Serialize(file, playerData);
        file.Close();
        //Debug.Log("%%%%%% SavePlayerData(float, int) executado %%%%%%%%");
    }

    public static void SavePlayerData(PlayerData playerData) {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(FILE_PATH);
        binaryFormatter.Serialize(file, playerData);
        file.Close();
    }

    /// <summary>
    /// Loads player's data
    /// </summary>
    /// <returns>Return of type PlayerData</returns>
    public static PlayerData LoadPlayerData() {
        if(File.Exists(FILE_PATH)) {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(FILE_PATH, FileMode.Open);
            PlayerData playerData = (PlayerData)binaryFormatter.Deserialize(file);
            file.Close();

            return playerData;
        }
        return null;
    }

    /// <summary>
    /// Returns true if the player has hearts available
    /// </summary>
    /// <returns>Return of type int with the amount of hearts</returns>
    public static bool HasHearts() {
        PlayerData playerData = LoadPlayerData();

        if(playerData.Hearts > 0) {
            return true;
        }

        return false;
    }

    public static bool HasFileCreated() {
        //If the user is playing for the first time
        if (File.Exists(FILE_PATH)) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns how many hearts the player has available
    /// </summary>
    /// <returns>Returns an int with the amount of hearts</returns>
    public static long GetHeartsStored() {
        return LoadPlayerData().Hearts;
    }

    /// <summary>
    /// Returns the last score multiplier set in a quiz
    /// </summary>
    /// <returns>Return of type float with the current score multiplier value</returns>
    public static double GetScoreMultiplier() {
        return LoadPlayerData().ScoreMultiplier;
    }

    public static void SetScoreMultiplier(double newScoreMultiplier) {
        PlayerData playerData = LoadPlayerData();
        if (playerData != null) {
            playerData.ScoreMultiplier = newScoreMultiplier;
            SavePlayerData(playerData);
        }
    }

    /// <summary>
    /// Subtracts an amount of hearts from the player's saved hearts, saves the result and returns it
    /// </summary>
    public static long DecreaseHeart(int amount) {
        PlayerData playerData = LoadPlayerData();
        if (playerData != null) {
            playerData.Hearts -= amount;
            SavePlayerData(playerData);
            return playerData.Hearts;
        }
        return -1;
    }

    /// <summary>
    /// Adds an amount of hearts from the player's saved hearts, saves the result and returns it
    /// </summary>
    public static long IncreaseHeart(int amount) {
        PlayerData playerData = LoadPlayerData();
        if (playerData != null) {
            playerData.Hearts += amount;
            SavePlayerData(playerData);

            return playerData.Hearts;
        }
        return -1;
    }

    public static void SetHearts(int amount) {
        PlayerData playerData = LoadPlayerData();
        if (playerData != null) {
            playerData.Hearts = amount;
            SavePlayerData(playerData);
        }
    }

    public static void AddLevelToFile(LevelInfo newLevel) {
        PlayerData playerData = LoadPlayerData();
        //If we have a list initialized
        if(playerData.levelsPlayed != null) {
            LevelInfo levelFound = null;
            //Searchs the level
            foreach(LevelInfo levelInList in playerData.levelsPlayed) {
                //If the names match, then
                if(levelInList.Equals(newLevel)) {            
                    levelFound = levelInList;
                    break;
                }
            }
            //If the list doesn't contain this level
            if (levelFound == null) {
                //Debug.Log("@@@@@@ Level encontrado na lista");
                //Add level to the list
                playerData.levelsPlayed.Add(newLevel);
                //Save the PlayerData
                SavePlayerData(playerData);
            //If level was found in the list
            } else {
                //Debug.Log("@@@@@@ Level NAO encontrado na lista");
                //If the new level has a score bigger than the level found
                if (newLevel.score > levelFound.score) {
                    //Removes the old one
                    playerData.levelsPlayed.Remove(levelFound);
                    //And add the new level to the list
                    playerData.levelsPlayed.Add(newLevel);
                    //Save the PlayerData
                    SavePlayerData(playerData);
                }
            }
        //If we don't have a list of levels yet
        } else {
            //Initialize a new list
            playerData.levelsPlayed = new List<LevelInfo>();
            //Add to level to the list
            playerData.levelsPlayed.Add(newLevel);
            //Save the PlayerData
            SavePlayerData(playerData);
        }
    }

    public static LevelInfo HasPlayedLevel(string levelName) {
        PlayerData playerData = LoadPlayerData();
        //If we have a player data saved
        if(playerData != null) {
            //If we have a valid list
            if (playerData.levelsPlayed != null && playerData.levelsPlayed.Count > 0) {                
                foreach(LevelInfo level in playerData.levelsPlayed) {
                    //If a level in the list match the name with the parameter
                    if(level.name.Equals(levelName)) {
                        return level;
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Store the music state in game as the parameter passed
    /// </summary>
    /// <param name="musicState">New state</param>
    public static void SetMusicGameConfigs(bool musicState) {
        PlayerData playerData = LoadPlayerData();

        //If we have a player data saved
        if (playerData != null) {
            //If there isn't a game config set
            if (playerData.gameConfigs == null) {
                //Create a new one
                playerData.gameConfigs = new GameConfigs();
                //Set the new music state
                playerData.gameConfigs.wishMusicOn = musicState;

            //If there is a game config set
            } else {                
                //Just set the new music
                playerData.gameConfigs.wishMusicOn = musicState;
            }

            //Save Player Data
            PlayerPersistence.SavePlayerData(playerData);
        }
    }
    
    /// <summary>
    /// Store the SFX state in game as the parameter passed
    /// </summary>
    /// <param name="sfxState">New State</param>
    public static void SetSfxGameConfig(bool sfxState) {
        PlayerData playerData = LoadPlayerData();

        //If we have a player data saved
        if (playerData != null) {
            //If there isn't a game config set
            if (playerData.gameConfigs == null) {
                //Create a new GameConfig
                playerData.gameConfigs = new GameConfigs();
                //Set the new music state
                playerData.gameConfigs.wishSfxOn = sfxState;

            //If there is a game config set
            } else {
                //Just set the new music state
                playerData.gameConfigs.wishSfxOn = sfxState;
            }

            //Save Player Data
            PlayerPersistence.SavePlayerData(playerData);
        }
    }

    /// <summary>
    /// Get the stored game configurations 
    /// </summary>
    /// <returns></returns>
    public static GameConfigs GetGameConfigs() {
        PlayerData playerData = LoadPlayerData();

        //If we have a player data saved
        if (playerData != null) {
            if (playerData.gameConfigs != null) {
                return playerData.gameConfigs;
            }
        }
        return null;
    }
}

/// <summary>
/// Class to be stored/serialized to the local files with the player's info
/// </summary>
[Serializable]
public class PlayerData {
    [SerializeField]
    //Using double due to Firebase JSON types
    private double scoreMultiplier;
    public double ScoreMultiplier { get { return scoreMultiplier; } set { scoreMultiplier = value; } }

    [SerializeField]
    //Using long due to Firebase JSON types
    private long hearts;
    public long Hearts { get { return hearts; } set { hearts = value; } }

    //[SerializeField]
    public List<LevelInfo> levelsPlayed;
    //public List<LevelInfo> LevelsPlayed { get => levelsPlayed; set => levelsPlayed = value; }

    [SerializeField]
    public GameConfigs gameConfigs;

    public PlayerData() {
        //levelsPlayed = new List<LevelInfo>();
        //Debug.LogError("############# PlayerData > Created class PlayerData!!!!");
    }
}
