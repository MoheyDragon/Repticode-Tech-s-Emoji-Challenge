using System.Collections.Generic;
using UnityEngine;
using System.IO;
public  class LocalizationManager : Singleton<LocalizationManager>
{
    /* Mainly this Localization system works as follows : each collection of dialogues are represented by a single csv file 
     * located in the streaming assets/Texts/ groupName.csv.
     * At Awake setup is called: it reads all .csvs located in the Texts folder 
     * and saves it's details in the corresponding LocalizedDialogueGroup.
     * To Setup the Localization system You only need to add both this script and LanguageManager to a game object in the scene and change the "LocalizedGroupsNames" Enums to match your groups.
     * when localized text is required , the public function GetLocalizedString is all what is needed
     * it has two parameters the localizedDialogueGroupName as Enum to make it easier to call , and the index of the line needed
     * this functions should be called from a diaplogue manager script that has a reference to the localizedDialogueGroupEnum
     */
    LocalizedDialogueGroup[] localizedDialogueGroups;
    string textsPath = Application.streamingAssetsPath + "/Texts/";
    string filename;
    int numberOfLocalDialoguesGroups;
    protected override void Awake()
    {
        base.Awake();
        Setup();
    }
    private void Setup()
    {
        DirectoryInfo info = new DirectoryInfo(textsPath);
        int filesCount = info.GetFiles().Length;
        for (int i = 0; i < filesCount; i++)
        {
            if (Path.GetFileName(info.GetFiles()[i].Name).EndsWith(".csv"))
                numberOfLocalDialoguesGroups++;
        }
        localizedDialogueGroups = new LocalizedDialogueGroup[numberOfLocalDialoguesGroups];

        for (int i = 0; i < numberOfLocalDialoguesGroups; i++)
        {
            _SetupSingleDialogueGroup(i);
        }
    }
    private void _SetupSingleDialogueGroup(int index)
    {
        localizedDialogueGroups[index] = new LocalizedDialogueGroup(new List<Row>());
        filename = textsPath + (LocalizedGroupsNames)index + ".csv";
        if (!LoadLocalizedGroupDataFromDisk(localizedDialogueGroups[index],(LocalizedGroupsNames)index))
            Debug.LogError("Failed to read data");
    }
    #region LoadingData
    private bool LoadLocalizedGroupDataFromDisk(LocalizedDialogueGroup localizedDialogueGroup,LocalizedGroupsNames groupName)
    {
        #region CheckIfHaveDataFile
        try
        {
            string[] data = File.ReadAllLines(filename);
        }
        catch
        {
            return true;
        }
        #endregion
        int errorIndex=0;
        try
        {
            string[] data = File.ReadAllLines(filename);
            for (int i = 1; i < data.Length; i++)
            {
                errorIndex = i-1;
                string[] sortedLine = data[i].Split('|');
                float duration =float.Parse(sortedLine[0]);
                string[] localTexts = new string[sortedLine.Length-1];
                for (int j = 0; j < localTexts.Length; j++)
                {
                    localTexts[j] = sortedLine[j+1];
                }
                _AddNewRow(localizedDialogueGroup,duration,localTexts);
            }
            return true;
        }
        catch
        {
            Debug.LogError("Line " + errorIndex + " from " + groupName + ".csv has invaild input");
            return false;
        }
    }

    private void _AddNewRow(LocalizedDialogueGroup localizedDialogueGroup,float duration,string[] localTexts)
    {
        Row newRow = new(duration,localTexts);
        localizedDialogueGroup.rows.Add(newRow);
    }
    #endregion

    #region publicFunctions
    public Row GetRow(LocalizedGroupsNames localizedDialogueGroupName, int textIndexInDialogueGroup)
    {
        int localGroupIndex = (int)localizedDialogueGroupName;
        return localizedDialogueGroups[localGroupIndex].rows[textIndexInDialogueGroup];
    }
    #endregion
}
