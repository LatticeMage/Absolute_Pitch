using System.Collections;
using System.IO;
using LitJson;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class LoadSettings : MonoBehaviour
{
    public SlotManager slotManager;

    public TMPro.TextMeshProUGUI statusText;

    public enum LoadingState
    {
        Idle,
        Loading,
        Done,
        Length
    };

    private string[] LoadingStateText =
    {
        "Idle",
        "Loading",
        "Done",
        "Length"
    };

    public LoadingState currentState = LoadingState.Idle;

    public GridManager gridManager;

    private string searchPath = @"Settings/";
    private string searchPattern = @"*.ogg";

    private System.Diagnostics.Stopwatch stopWatch;

    public int doneLoadingCount = 0;

    [SerializeField]
    private int LoadingCount = 0;

    public void Awake()
    {
        stopWatch = new System.Diagnostics.Stopwatch();
        stopWatch.Start();

        currentState = LoadingState.Idle;
    }

    public JsonData LoadSettingJson()
    {
        string jsonPath = "";

        string[] files = Directory.GetFiles(searchPath);
        for (int i = 0; i < files.Length; ++i)
        {
            string file = files[i];
            if (file.ToLower().IndexOf(".json") == file.Length - 5)
            {
                Debug.Log(string.Format("Loading {0}", file));
                jsonPath = file;
                break;
            }
        }

        if (jsonPath.Length == 0)
        {
            Debug.LogWarning("Error. Using default json name.");
            jsonPath = searchPath + "settings.json";
        }

        string raw = File.ReadAllText(jsonPath);
        JsonData data = JsonMapper.ToObject(raw);

        return data;
    }

    public void LoadAudioFromSettings(JsonData data)
    {
        string audioPath = data["audioPath"].ToString();
        string[] files = Directory.GetFiles(searchPath + audioPath, searchPattern, SearchOption.AllDirectories);

        LoadingCount = files.Length;

        for (int i = 0; i < files.Length; ++i)
        {
            string file = files[i];
            // "settings/./mp3\Bb7.mp3" => "Bb7"
            string fileTrimmed = file.Substring(file.LastIndexOf("\\") + 1, file.Length - 4 - file.LastIndexOf("\\") - 1);
            System.Text.RegularExpressions.Match matches = System.Text.RegularExpressions.Regex.Match(fileTrimmed, @"([a-zA-Z]+)(\d+)");
            string scale = matches.Groups[1].ToString();
            string level = matches.Groups[2].ToString();
            SetupAudioFiles(scale, level, file);
        }
    }

    public void LoadSlotsFromSettings(JsonData data)
    {
        JsonData slots = data["slots"];
        for(int i = 0; i < slots.Count; ++i)
        {
            JsonData slot = slots[i];
            string name = slot["name"].ToString();
            string type = slot["type"].ToString();
            JsonData note = slot["note"];

            List<string> realNote = new List<string>();

            if (type == "multi")
            {
                for (int j = 0; j < note.Count; ++j)
                    realNote.Add(note[j].ToString());
            }
            else if (type == "single")
            {
                realNote.Add(note.ToString());
            }
            else if (type == "silence")
            {
                realNote.Add(note.ToString());
            }
            else
            {
                Debug.LogError("Warning Format: " + type);
            }


            SettingSlots newSlot = new SettingSlots();
            newSlot.name = name;
            newSlot.type = (SettingSlots.NoteType)SettingSlots.GetTypeByName(type);

            for (int j = 0; j < realNote.Count; ++j)
            {
                string noteName = realNote[j];
                int index = gridManager.GetGridByName(noteName);
                if(type != "silence" && (index == -1 || index >= gridManager.GeneratedGrid.Count))
                {
                    Debug.LogError("Unfound Note: " + noteName + ", skip");
                }
                else if(type == "silence")
                {
                    newSlot.ids.Add(int.Parse(realNote[0]));
                    break;
                }
                else
                {
                    newSlot.ids.Add(index);
                }                
            }

            slotManager.AddSlot(newSlot);
        }
    }

    public void Load()
    {       
        JsonData data = LoadSettingJson();

        LoadAudioFromSettings(data);

        LoadSlotsFromSettings(data);
    }

    void SetupAudioFiles(string scale, string level, string audioFilePath)
    {
        int index = gridManager.GetGridByName(scale + level);

        currentState = LoadingState.Loading;

        StartCoroutine(GetAudioClip(index, audioFilePath));
    }

    IEnumerator GetAudioClip(int index, string audioFilePath)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + Path.GetFullPath(audioFilePath), AudioType.OGGVORBIS))
        {
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                gridManager.AddAudioToGrid(index, myClip);
                ++doneLoadingCount;
            }
        }
    }


    private void Update()
    {
        statusText.text = "Loading State: "  + LoadingStateText[(int)currentState];
        if(currentState == LoadingState.Done)
        {
            statusText.text += ", elapsed " + stopWatch.ElapsedMilliseconds + "ms";
        }
        else
        {
            statusText.text += string.Format(", status: {0}/{1}", doneLoadingCount, LoadingCount);
        }

        if(doneLoadingCount == LoadingCount)
        {
            currentState = LoadingState.Done;
            if (stopWatch.IsRunning)
            {
                stopWatch.Stop();
                Debug.Log("Load Time: " + stopWatch.ElapsedMilliseconds);
            }
        }
    }
}
