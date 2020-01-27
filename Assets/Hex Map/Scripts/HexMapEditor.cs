using System;
using UnityEngine;
using UnityEngine.UI;

public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;
    public HexGrid hexGrid;
    private Color activeColor;
    private int activeElevation;
    private bool applyColor;
    private bool applyElevation = true;
    public Toggle toggle;

    private void Awake()
    {
        SelectColor(0);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            EditCells(hexGrid.GetCell(hit.point));
        }
    }

    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX +brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x,z)));
            }
        }

        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--,r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x,z)));
            }
        }
    }
    
    void EditCell(HexCell cell)
    {
        if (cell)
        {
            if (applyColor)
            {
                cell.Color = activeColor;    
            }
            if (applyElevation)
                cell.Elevation = activeElevation;    
        }
    }

    public Slider slider;
    public Slider brushSlider;
    private int brushSize;

    public void SetBrushSize()
    {
        brushSize = (int) brushSlider.value;
    }
    
    public void SetElevation()
    {
        activeElevation = (int)slider.value;
    }

    public void SelectColor(int index)
    {
        applyColor = index >= 0;
        if (applyColor)
            activeColor = colors[index];
    }

    public void SetApplyElevation()
    {
        applyElevation = toggle.isOn;
    }

    public Toggle showUIToggle;
    public void ShowUI()
    {
        hexGrid.ShowUI(showUIToggle.isOn);
    }
}