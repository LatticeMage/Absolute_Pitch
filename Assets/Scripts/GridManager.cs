using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridGenerator))]
public class GridManager : MonoBehaviour
{
    public List<GameObject> GeneratedGrid;

    public List<GameObject> HelperRowGrid;
    public List<GameObject> HelperColumnGrid;

    private GridGenerator generator;
    private GridAudioManager audioManager;

    private void Awake()
    {
        generator = GetComponent<GridGenerator>();
        if (audioManager == null)
        {
            audioManager = GetComponent<GridAudioManager>();
        }
    }

    public void PlayGrid(int[] ids, float gridBehaviourDelayLimit)
    {
        if (ids.Length == 0)
            return;

        AudioSource master = GeneratedGrid[ids[0]].GetComponent<AudioSource>();

        for (int i = 0; i < ids.Length; ++i)
        {
            GameObject grid = GeneratedGrid[ids[i]];

            GridBehaviour behaviour = grid.GetComponent<GridBehaviour>();

            if (i != 0)
            {
                behaviour.masterAudioSource = master;
            }

            behaviour.SetDelayLimit(gridBehaviourDelayLimit);

            behaviour.Trigger();            
        }
    }

    public void AddGrid(GameObject grid)
    {
        GeneratedGrid.Add(grid);
    }

    public void AddHelperColumnGrid(GameObject grid)
    {
        HelperColumnGrid.Add(grid);
    }

    public void AddHelperRowGrid(GameObject grid)
    {
        HelperRowGrid.Add(grid);
    }

    public int GetGridByName(string name)
    {
        int index = -1;
        for (index = 0; index < GeneratedGrid.Count; ++index)
        {
            if (GeneratedGrid[index].GetComponentInChildren<TMPro.TextMeshProUGUI>().text == name)
            {
                break;
            }
        }
        return index;
    }

    public void AddAudioToGrid(int index, AudioClip clip)
    {
        AudioSource gridAudioSource = GeneratedGrid[index].GetComponent<AudioSource>();
        gridAudioSource.clip = clip;
        audioManager.AudioSources.Add(gridAudioSource);
    }

    public void Update()
    {
        for (int i = 0; i < GeneratedGrid.Count; ++i)
        {
            var grid = GeneratedGrid[i];
            int columnIndex = i % generator.column;
            int rowIndex = i / generator.column;
            generator.SetupGrid(grid, rowIndex, columnIndex);
        }

        for (int i = 0; i < HelperColumnGrid.Count; ++i)
        {
            var grid = HelperColumnGrid[i];
            generator.SetupHelperColumnGrid(grid, i);
        }

        for (int i = 0; i < HelperRowGrid.Count; ++i)
        {
            var grid = HelperRowGrid[i];
            generator.SetupHelperRowGrid(grid, i);
        }
    }

    public List<GameObject> DebugGeneratedGridInstance()
    {
        return GeneratedGrid;
    }
}
