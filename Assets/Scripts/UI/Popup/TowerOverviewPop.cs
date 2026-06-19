using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerOverviewPop : BasePopup
{
    public static TowerOverviewPop Instance;
    [SerializeField] private TMP_Text _heal;
    [SerializeField] private TMP_Text _range;
    [SerializeField] private TMP_Text _damage;
    [SerializeField] private TMP_Text _fireRate;
    [SerializeField] private TMP_Text _requiredStone;
    [SerializeField] private TMP_Text _requiredWood;
    [SerializeField] private TMP_Text _reWood;
    [SerializeField] private TMP_Text _reStone;
    [SerializeField] private Button _upgrade, _destroy;
    private TowerBase _currentTowerSelected;
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        Hide();
    }
    public void FillData(TowerBase tower)
    {
        if (!tower) return;
        _currentTowerSelected = tower;
        if (_heal != null) _heal.text = tower.CurrentData.heal.ToString();
        if (_range != null) _range.text = tower.CurrentData.range.ToString("0.0");
        if (_damage != null) _damage.text = tower.CurrentData.damage.ToString("0");
        if (_fireRate != null) _fireRate.text = tower.CurrentData.attackSpeed.ToString("0.00") + "s";
        if (_requiredWood != null) _requiredWood.text = tower.CurrentData.woodCost.ToString("0");
        if (_requiredStone != null) _requiredStone.text = tower.CurrentData.stoneCost.ToString("0");
        if (_reWood != null) _reWood.SetText($"{tower.CurrentData.reWood}");
        if (_reStone != null) _reStone.SetText($"{tower.CurrentData.reStone}");

    }
    public void Start()
    {
        _upgrade.onClick.AddListener(TryUpgrade);
        _destroy.onClick.AddListener(TryDestroy);
    }
    private void TryUpgrade()
    {
        if (_currentTowerSelected)
        {
            if (InventoryManager.Instance.GetItemCount(ItemType.wood) >= _currentTowerSelected.CurrentData.woodCost && InventoryManager.Instance.GetItemCount(ItemType.stone) >= _currentTowerSelected.CurrentData.stoneCost)
            {
                InventoryManager.Instance.AddItemByType(ItemType.wood, -_currentTowerSelected.CurrentData.woodCost);
                InventoryManager.Instance.AddItemByType(ItemType.stone, -_currentTowerSelected.CurrentData.stoneCost);
                _currentTowerSelected.Upgrade();
                FillData(_currentTowerSelected);
            }
        }
    }
    private void TryDestroy()
    {
        if (_currentTowerSelected)
        {
            _currentTowerSelected.ReturnPool();
            Hide();
        }
    }
}
