using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    [Header("Controls")]

    public int row;
    public int column;

    public float width;
    public float height;

    public float gridWidth;
    public float gridHeight;

    [SerializeField]
    private string[] HelperRowText;

    [SerializeField]
    private string[] HelperColumnText;

    [SerializeField]
    private Color[] Lut;

    [Header("Other Instances")]

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private RectTransform origin;

    [SerializeField]
    private GameObject PrefabTemplate;

    [SerializeField]
    private GridManager gridManager;

    public LoadSettings settingLoader;

    void Start()
    {
        Generate();
        SetupHelperGrid();
        settingLoader.Load();
    }

    void Generate()
    {
        for (int i = 0; i < row; ++i)
        {
            for (int j = 0; j < column; ++j)
            {
                GameObject grid = GameObject.Instantiate(PrefabTemplate, canvas.transform);

                SetupGrid(grid, i, j);

                gridManager.AddGrid(grid);
            }
        }
    }

    public void SetupHelperColumnGrid(GameObject grid, int columnIndex)
    {
        RectTransform rectTransform = grid.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = origin.anchoredPosition + new Vector2(width * (columnIndex - column / 2f + 1f), height * (row / 2f + 1f));

        rectTransform.sizeDelta = new Vector2(gridWidth, gridHeight);

        // Setup Color
        grid.GetComponent<UnityEngine.UI.Image>().color = Color.gray;

        // Deal With Text
        TMPro.TextMeshProUGUI text = grid.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        text.text = HelperColumnText[columnIndex];

        GridBehaviour behaviour = grid.GetComponent<GridBehaviour>();
        if(behaviour) Destroy(behaviour);
    }

    public void SetupHelperRowGrid(GameObject grid, int rowIndex)
    {
        // Setup Position
        RectTransform rectTransform = grid.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = origin.anchoredPosition + new Vector2(width* -(column / 2f), height* (row / 2f - rowIndex));

        rectTransform.sizeDelta = new Vector2(gridWidth, gridHeight);

        // Setup Color
        grid.GetComponent<UnityEngine.UI.Image>().color = Lut[rowIndex];

        // Deal With Text
        TMPro.TextMeshProUGUI text = grid.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        text.text = HelperRowText[rowIndex];
        text.color = new Color(0, 0, 0, 0);

        GridBehaviour behaviour = grid.GetComponent<GridBehaviour>();
        if (behaviour) Destroy(behaviour);
    }

    public void SetupHelperGrid()
    {
        for (int i = 0; i < 1; ++i)
        {
            for (int j = 0; j < column; ++j)
            {           
                GameObject grid = GameObject.Instantiate(PrefabTemplate, canvas.transform);

                SetupHelperColumnGrid(grid, j);

                gridManager.AddHelperColumnGrid(grid);
            }
        }

        for (int i = 0; i < row; ++i)
        {
            for (int j = 0; j < 1; ++j)
            {
                GameObject grid = GameObject.Instantiate(PrefabTemplate, canvas.transform);

                SetupHelperRowGrid(grid, i);

                gridManager.AddHelperRowGrid(grid);
            }
        }
    }

    public void SetupGrid(GameObject grid, int rowIndex, int columnIndex)
    {
        grid.name = string.Format("Grid[{0}][{1}]", rowIndex, columnIndex);
        
        // Setup Position
        RectTransform rectTransform = grid.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = origin.anchoredPosition + new Vector2(width * (column / 2 - columnIndex), height * (row / 2 - rowIndex));

        rectTransform.sizeDelta = new Vector2(gridWidth, gridHeight);

        // Get GridBahaviour
        GridBehaviour behaviour = grid.GetComponent<GridBehaviour>();
        
        if (behaviour == null)  throw new System.Exception();

        behaviour.id = rowIndex * column + columnIndex;
        behaviour.lutColor = Lut[rowIndex];
        behaviour.audioSource = grid.GetComponent<AudioSource>();
        
        // Don't Setup Color, Conflict with GridBehaviour
        // grid.GetComponent<UnityEngine.UI.Image>().color = Color.white;

        // Deal With Text
        TMPro.TextMeshProUGUI text = grid.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        text.text = HelperRowText[rowIndex] + HelperColumnText[column - 1 - columnIndex];
        text.color = new Color(0,0,0,0);
    }

}
