using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleEnemyUI : MonoBehaviour
{
    // 敵
    [SerializeField] private TextMeshProUGUI enemyNameText;

    [SerializeField] private TextMeshProUGUI enemyLevelText;
    [SerializeField] private HPBar enemyHpBar;

    private Enemy enemy;

    // 敵の詳細
    [SerializeField] private GameObject selectedArrow;

    [SerializeField] private GameObject enemyDiscriptionPanel;
    [SerializeField] TextMeshProUGUI[] typeCompatibilityTexts;

    public GameObject SelectedArrow { get => selectedArrow; set => selectedArrow = value; }

    public void SetEnemyData(Enemy enemy)
    {
        this.enemy = enemy;
        enemyNameText.text = enemy.EnemyBattleName;
        enemyLevelText.text = "Lv." + enemy.Level.ToString();
        enemyHpBar.SetHP(enemy.currentHP, enemy.currentMaxHp);
        //weakImage.SetActive(false);
    }

    public void UpdateHp()
    {
        Debug.Log("enemyName" + enemy.EnemyBase.EnemyName + "hp" + enemy.currentHP + "maxhp" + enemy.currentMaxHp);
        // コルーチンの中でStartCoroutineは省略可能
        enemyHpBar.SetHP((float)enemy.currentHP, enemy.currentMaxHp);
        //enemyHpBar.SetHP((float)enemy.Hp,enemy.MaxHp);
    }

    // 詳細パネルをセットしてオン
    public void SetActivenessDiscriptionPanel(Enemy enemy)
    {
        enemyDiscriptionPanel.SetActive(true);
        for (int i = 0; i < (int)MagicType.END-2; i++)
        {
            typeCompatibilityTexts[i].text = "-";
        }
        for (int i = 0;i < enemy.EnemyBase.WeakTypes.Length; i++)
        {
            typeCompatibilityTexts[(int)enemy.EnemyBase.WeakTypes[i]].text = "弱";
        }
        for (int i = 0; i < enemy.EnemyBase.ResistanceTypes.Length; i++)
        {
            typeCompatibilityTexts[(int)enemy.EnemyBase.ResistanceTypes[i]].text = "耐";
        }
    }

    public void CloseDiscriptionPanel()
    {
        enemyDiscriptionPanel.SetActive(false);
    }

    public void SetActiveSelectedArrow(bool activeness)
    {
        SelectedArrow.SetActive(activeness);
    }

    public void UnActiveUIPanel()
    {
        this.GetComponent<Canvas>().enabled = false;
    }

    public void ActivateUIPanel()
    {
        this.GetComponent<Canvas>().enabled = true;
    }
}