using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HexMapEditor : MonoBehaviour
{
    private int activeTerrainTypeIndex;
    public HexGrid hexGrid;
    private int activeElevation;
    private int activeWaterLevel;
    private bool applyElevation = true;
    private bool applyWaterLevel = true;
    public Toggle toggle;
    public Toggle applyWaterLevelToggle;
    public Slider waterLevelSlider;
    private OptionalToggle riverMode, roadMode, walledMode;
    private bool isDrag;
    private HexDirection dragDirection;
    private HexCell previousCell;

    public void SetTerrainTypeIndex(int index)
    {
        activeTerrainTypeIndex = index;
    }
    
    public void SetApplyWaterLevel()
    {
        applyWaterLevel = applyWaterLevelToggle.isOn;
    }

    public void SetWaterLevel()
    {
        activeWaterLevel = (int)waterLevelSlider.value;
    }
    
    public void SetRiverMode(int mode)
    {
        riverMode = (OptionalToggle) mode;
    }

    public void SetRoadMode(int mode)
    {
        roadMode = (OptionalToggle) mode;
    }

    public void SetWalledMode(int mode)
    {
        walledMode = (OptionalToggle) mode;
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
        else
        {
            previousCell = null;
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            HexCell currentCell = hexGrid.GetCell(hit.point);
            if (previousCell && previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                isDrag = false;
            }
            EditCells(currentCell);
            previousCell = currentCell;
        }
        else
        {
            previousCell = null;
        }
    }

    void ValidateDrag(HexCell currentCell)
    {
        for (dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; dragDirection++)
        {
            if (previousCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true;
                return;
            }
        }

        isDrag = false;
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
            if (activeTerrainTypeIndex >= 0)
            {
                cell.TerrainTypeIndex = activeTerrainTypeIndex;
            }
            if (applyElevation)
                cell.Elevation = activeElevation;
            if (applyWaterLevel)
                cell.WaterLevel = activeWaterLevel;
            if (applySpecialIndex)
                cell.SpecialIndex = activeSpecialIndex;
            if (applyUrbanLevel)
                cell.UrbanLevel = activeUrbanLevel;
            if (applyFarmLevel)
                cell.FarmLevel = activeFarmLevel;
            if (applyPlantLevel)
                cell.PlantLevel = activePlantLevel;
            if (riverMode == OptionalToggle.No)
                cell.RemoveRiver();
            if (roadMode == OptionalToggle.No)
                cell.RemoveRoads();
            if (walledMode != OptionalToggle.Ignore)
                cell.Walled = walledMode == OptionalToggle.Yes;
            if (isDrag)
            {
                HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                if (otherCell)
                {
                    if (riverMode == OptionalToggle.Yes)
                        otherCell.SetOutgoingRiver(dragDirection);
                    if (roadMode == OptionalToggle.Yes)
                        otherCell.AddRoad(dragDirection);
                }
            }
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

    public void SetApplyElevation()
    {
        applyElevation = toggle.isOn;
    }

    public Toggle showUIToggle;
    public void ShowUI()
    {
        hexGrid.ShowUI(showUIToggle.isOn);
    }

    private int activeUrbanLevel, activeFarmLevel, activePlantLevel,activeSpecialIndex;
    public Slider activeUrbanLevelSlider, activeFarmLevelSlider, activePlantLevelSlider,activeSpecialSlider;
    private bool applyUrbanLevel, applyFarmLevel, applyPlantLevel,applySpecialIndex;
    public Toggle applyUrbanLevelToggle, applyFarmLevelToggle, applyPlantLevelToggle, applySpecialToggle;

    public void SetApplySpecialIndex()
    {
        applySpecialIndex = applySpecialToggle.isOn;
    }

    public void SetSpecialIndex()
    {
        activeSpecialIndex = (int) activeSpecialSlider.value;
    }

    public void SetApplyUrbanLevel()
    {
        applyUrbanLevel = applyUrbanLevelToggle.isOn;
    }

    public void SetUrbanLevel()
    {
        activeUrbanLevel = (int)activeUrbanLevelSlider.value;
    }

    public void SetApplyFarmLevel()
    {
        applyFarmLevel = applyFarmLevelToggle.isOn;
    }

    public void SetFarmLevel()
    {
        activeFarmLevel = (int)activeFarmLevelSlider.value;
    }

    public void SetApplyPlantLevel()
    {
        applyPlantLevel = applyPlantLevelToggle.isOn;
    }

    public void SetPlantLevel()
    {
        activePlantLevel = (int)activePlantLevelSlider.value;
    }

    public void Save()
    {
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        using (BinaryWriter write = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            write.Write(0);
            hexGrid.Save(write);
        }
    }

    public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            int header = reader.ReadInt32();
            if (header == 0)
            {
                hexGrid.Load(reader);    
            }
        }
    }
}