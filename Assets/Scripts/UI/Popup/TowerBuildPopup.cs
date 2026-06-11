using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerBuildPopup : BasePopup
{
    [SerializeField] private List<Image> imagesSelected;
    [SerializeField] private TowerBase _towerPrefab1;
    [SerializeField] private TowerBase _towerPrefab2;
    [SerializeField] private TowerBase _towerPrefab3;

    private TowerBase _towerSelected;

    private void Start()
    {
        TryUnSelectUI();
    }
    public void OnSelected(int value)
    {
        TryUnSelectUI();
        switch (value)
        {
            case 0:
                _towerSelected = _towerPrefab1;
                imagesSelected[0].enabled = true;
                break;
            case 1:
                _towerSelected = _towerPrefab2;
                imagesSelected[1].enabled = true;
                break;
            case 2:
                _towerSelected = _towerPrefab3;
                imagesSelected[2].enabled = true;
                break;
        }
        TowerBuildManager.Instance.StartBuild(_towerSelected);

    }
    private void TryUnSelectUI()
    {
        foreach (var img in imagesSelected)
        {
            if (img.enabled)
            {
                img.enabled = false;
            }
        }
    }
}
