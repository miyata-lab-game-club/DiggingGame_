﻿using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.EventSystems;
using System.Linq;
public enum GameStatus
{
    DIGGING,
    MENU,
    ITEM,
    STATUS,
}

public enum ItemUseStatus
{
    SELECT_ITEM,
    SELECT_TARGET,
    USE_ITEM
}

public class Define
{
    public enum DirectionNumber
    {
        RIGHT_UP,
        RIGHT,
        RIGHT_DOWN,
        DOWN,
        LEFT_DOWN,
        LEFT,
        LEFT_UP,
        NONE
    }

    // 方向　右上、右、右下、下、左下、左、左上
    public static Vector2[] directions = { new Vector2(1, 1), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1), new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, 0) };
}


public class PlayerController : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    private Rigidbody2D rb;
    private float vx;
    private float vy;
    private bool pushFlag;
    private bool jumpFlag;
    private bool groundFlag;
    private Define.DirectionNumber currentDirectionNumber;
    [SerializeField] private Menu menu;

    public static GameStatus currentGameStatus;

    // プレイヤー 仮
    [SerializeField] private PlayerBase[] playerBase;

    [SerializeField] private GameObject[] playerUIs;
    private List<Player> players;

    // メニュー画面 =======================
    private int currentMenuCommandNum;
    // アイテム関係 ======================

    private ItemUseStatus currentItemUseStatus;
    private int currentItemNum;
    private List<ItemCellData> itemCellData;

    public EnhancedScroller itemPanel;
    public EnhancedScrollerCellView cellViewPrefab;
    public float cellSize = 100f;

    // アイテムの選択されているインデックス番号
    private int selectedItemIndex;

    // アイテムのターゲットのインデックス番号
    private int selectedItemTargetIndex;


    // ステータス関係 ==========================
    // ステータス画面の選択されているインデックス番号
    private int selectedStatusIndex;

    [SerializeField] private PlayerStatusUIsManager playerStatusUIsManager;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        players = new List<Player>();
        currentGameStatus = GameStatus.DIGGING;
        currentMenuCommandNum = 0;
        currentItemNum = 0;
        selectedItemIndex = 0;
        selectedStatusIndex = 0;
        selectedItemTargetIndex = 0;
        Player player = new Player(playerBase[0], 1);
        players.Add(player);
        player = new Player(playerBase[1], 1);
        players.Add(player);

        menu.menuSelectButtonClickedDelegate = SelectMenuButton;
        menu.itemSelectButtonHoverdDelegate = CellButtonOnHover;
        playerStatusUIsManager.statusSelectButtonClickedDelegate = SelectStatusButton;
    }

    // Update is called once per frame
    private void Update()
    {
        // 穴掘り中だったら
        if (currentGameStatus == GameStatus.DIGGING)
        {
            // サーチ
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // サーチ機能
            }
            // メニュー画面
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // メニュー画面をひらく
                currentGameStatus = GameStatus.MENU;
                menu.ActivateMenuPanel(true);
                menu.ActivateMenuSelectArrow((int)MenuCommand.ITEM);
            }
            // マップ
            if (Input.GetKeyDown(KeyCode.M))
            {
                // マップを開く
            }
            // 移動
            vx = 0;
            vy = 0;

            // A：左
            if (Input.GetKey(KeyCode.A))
            {
                vx = -speed;
            }
            else
            // D：右
            if (Input.GetKey(KeyCode.D))
            {
                vx = speed;
            }

            if (Input.GetKey("space") && groundFlag == true)
            {
                if (pushFlag == false)
                {
                    jumpFlag = true;
                    pushFlag = true;
                }
            }
            else
            {
                pushFlag = false;
            }
        }

        // メインパネルを選択中だったら
        else if (currentGameStatus == GameStatus.MENU)
        {
            HandleMenuSelect();
        }
        else if (currentGameStatus == GameStatus.ITEM)
        {
            HandleItemSelect();
        }
        else if (currentGameStatus == GameStatus.STATUS)
        {
            HandleStatusSelect();


        }
    }

    private void HandleMenuSelect()
    {

        /*
        // キー操作
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMenuCommandNum < (int)MenuCommand.END - 1)
            {
                currentMenuCommandNum++;
                menu.ActivateMenuSelectArrow((MenuCommand)currentMenuCommandNum);
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMenuCommandNum > 0)
            {
                currentMenuCommandNum--;
                menu.ActivateMenuSelectArrow((MenuCommand)currentMenuCommandNum);
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // メニューを選択
            // アイテムコマンドだったら
            if (currentMenuCommandNum == ((int)MenuCommand.ITEM))
            {
                // アイテムパネルを開く
                menu.ActivateItemPanel(true);
                currentGameStatus = GameStatus.ITEM;
                menu.ActivateMenuPanel(false);
                InitItem();
            }
            if (currentMenuCommandNum == (int)MenuCommand.STATUS)
            {
                menu.ActivateStatusPanel(true);
                currentGameStatus = GameStatus.STATUS;
                InitStatus();
            }
        }
        */
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // メニュー画面を閉じる
            menu.ActivateMenuPanel(false);
            currentGameStatus = GameStatus.DIGGING;
        }
    }

    // マウスでメニューを選択する
    public void SelectMenuButton(int selectMenuIndex)
    {
        currentMenuCommandNum = selectMenuIndex;
        // メニューを選択
        // アイテムコマンドだったら
        if (currentMenuCommandNum == ((int)MenuCommand.ITEM))
        {
            // アイテムパネルを開く
            menu.ActivateItemPanel(true);
            currentGameStatus = GameStatus.ITEM;
            menu.ActivateMenuPanel(false);
            InitItem();
        }
        if (currentMenuCommandNum == (int)MenuCommand.STATUS)
        {
            menu.ActivateStatusPanel(true);
            currentGameStatus = GameStatus.STATUS;
            InitStatus();
        }
    }
    private void HandleItemSelect()
    {
        if (currentItemUseStatus == ItemUseStatus.SELECT_ITEM)
        {
            /*
            // キー操作
            bool selectionChanged = false;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                // 値を範囲内にする
                selectedItemIndex = Mathf.Clamp(selectedItemIndex - 1, 0, itemCellData.Count - 1);
                selectionChanged = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedItemIndex = Mathf.Clamp(selectedItemIndex + 1, 0, itemCellData.Count - 1);
                selectionChanged = true;
            }

            // 選択中
            if (selectionChanged)
            {
                for (int i = 0; i < itemCellData.Count; i++)
                {
                    itemCellData[i].isSelected = (i == selectedItemIndex);
                }
                // アクティブセルに対してUIの更新をする
                itemPanel.RefreshActiveCellViews();

                if (selectedItemIndex >= itemPanel.EndCellViewIndex)
                {
                    itemPanel.JumpToDataIndex(selectedItemIndex, 1.0f, 1.0f);
                }
                else if (selectedItemIndex <= itemPanel.StartCellViewIndex)
                {
                    itemPanel.JumpToDataIndex(selectedItemIndex, 0.0f, 0.0f);
                }
            }

            // アイテムを使用する
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("アイテムを使うよ");
                // 使うアイテムの情報を保持
                Item item = players[0].Items[selectedItemIndex];

                // 回復アイテムだったら
                if (item.ItemBase.itemType == ItemType.HEAL_ITEM)
                {
                    // アイテムパネルを消す
                    menu.ActivateItemPanel(false);
                    // ステータス画面を開く
                    menu.ActivateStatusPanel(true);
                    // ステータス画面の更新
                    playerStatusUIsManager.SetUpPlayerStatusUI(players);
                    playerStatusUIsManager.selectStatus(selectedItemTargetIndex);
                }
                // ターゲットを選ぶステータスに移行
                currentItemUseStatus = ItemUseStatus.SELECT_TARGET;
            }
            */
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // アイテム画面を閉じてメニュー画面を開く
                menu.ActivateMenuPanel(true);
                menu.ActivateItemPanel(false);
                currentGameStatus = GameStatus.MENU;
                currentItemUseStatus = ItemUseStatus.SELECT_ITEM;
            }
        }
        else if (currentItemUseStatus == ItemUseStatus.SELECT_TARGET)
        {
            /*
            // キー操作
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (selectedItemTargetIndex < players.Count - 1)
                {
                    selectedItemTargetIndex++;
                    playerStatusUIsManager.selectStatus(selectedItemTargetIndex);
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (selectedItemTargetIndex > 0)
                {
                    selectedItemTargetIndex--;
                    playerStatusUIsManager.selectStatus(selectedItemTargetIndex);
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {   // アイテムの残りが0だったら
                if (players[0].Items[selectedItemIndex].ItemCount == 0)
                {
                    Debug.Log("使えないよ");
                }
                else
                {
                    Item item = players[0].Items[selectedItemIndex];
                    // アイテムの効果発動
                    players[selectedItemTargetIndex].TakeHeal(item);
                    players[selectedItemTargetIndex].playerUI.UpdateHpSp();

                    // アイテムの数を減らす
                    players[0].Items[selectedItemIndex].ItemCount--;
                }
            }
            */
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 0のものをアイテムリストから除外する
                if (players[0].Items[selectedItemIndex].ItemCount == 0)
                {
                    players[0].Items.RemoveAt(selectedItemIndex);
                    if (selectedItemIndex != 0)
                    {
                        selectedItemIndex--;
                    }
                }
                // アイテムパネルの更新
                LoadItemData();
                itemPanel.RefreshActiveCellViews();
                // ステータス画面を閉じてアイテム画面を開く
                menu.ActivateStatusPanel(false);
                menu.ActivateItemPanel(true);
                currentGameStatus = GameStatus.ITEM;
                currentItemUseStatus = ItemUseStatus.SELECT_ITEM;
            }
        }
    }

    private void HandleStatusSelect()
    {
        /*
        // キー操作
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectedStatusIndex < players.Count - 1)
            {
                selectedStatusIndex++;
                playerStatusUIsManager.selectStatus(selectedStatusIndex);
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectedStatusIndex > 0)
            {
                selectedStatusIndex--;
                playerStatusUIsManager.selectStatus(selectedStatusIndex);
            }
        }
        */
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // アイテム画面を閉じてメニュー画面を開く
            menu.ActivateMenuPanel(true);
            menu.ActivateStatusPanel(false);
            currentGameStatus = GameStatus.MENU;
        }
    }

    #region 
    // アイテム関係 ===========

    private void InitItem()
    {
        itemPanel.Delegate = this;
        LoadItemData();
        itemPanel.RefreshActiveCellViews();
        currentItemUseStatus = ItemUseStatus.SELECT_ITEM;
    }

    // アイテムのロード
    private void LoadItemData()
    {
        itemCellData = new List<ItemCellData>();
        for (int i = 0; i < players[0].Items.Count; i++)
        {
            // Debug.Log(players[0].Items[i].ItemBase.ItemName);
            itemCellData.Add(new ItemCellData()
            {
                isSelected = i == selectedItemIndex,// 選択されているか
                itemText = players[0].Items[i].ItemBase.ItemName,
                itemCountText = players[0].Items[i].ItemCount.ToString()
            });
        }
        itemPanel.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller playerItemPanel)
    {
        return itemCellData.Count;
    }

    public float GetCellViewSize(EnhancedScroller playerItemPanel, int dataIndex)
    {
        return cellSize;
    }

    // 表示するセルの取得
    public EnhancedScrollerCellView GetCellView(EnhancedScroller playerItemPanel, int dataIndex, int cellIndex)
    {
        ItemCellView cell = playerItemPanel.GetCellView(cellViewPrefab) as ItemCellView;
        cell.cellButtonClicked = CellButtonClicked;

        cell.name = "Cell" + dataIndex.ToString();
        cell.SetData(itemCellData[dataIndex]);
        GameObject cellViewObj = cell.gameObject;
        // イベントトリガーを追加してホバー時に実行する関数を指定する
        EventTrigger eventTrigger = cellViewObj.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventDate) => CellButtonOnHover(dataIndex));
        eventTrigger.triggers.Add(entry);
        return cell;
    }

    // セルにホバーしたとき実行する関数 
    public void CellButtonOnHover(int selectedIndex)
    {
        selectedItemIndex = selectedIndex;
        for (int i = 0; i < itemCellData.Count; i++)
        {
            itemCellData[i].isSelected = (i == selectedIndex);
        }
        // 見た目の更新
        itemPanel.RefreshActiveCellViews();
    }
    private void CellButtonClicked(int selectedIndex)
    {
        // アクティブセルに対してUIの更新をする
        itemPanel.RefreshActiveCellViews();
        Debug.Log("アイテムを使うよ");
        // 使うアイテムの情報を保持
        Item item = players[0].Items[selectedItemIndex];

        // 回復アイテムだったら
        if (item.ItemBase.itemType == ItemType.HEAL_ITEM)
        {
            HealItemBase healItem = players[0].Items[selectedItemIndex].ItemBase as HealItemBase;
            // アイテムパネルを消す
            menu.ActivateItemPanel(false);
            // ステータス画面を開く
            menu.ActivateStatusPanel(true);
            // ステータス画面の更新
            InitStatus();
        }
        // ターゲットを選ぶステータスに移行
        currentItemUseStatus = ItemUseStatus.SELECT_TARGET;
        Debug.Log("Cell Data Integer Button Clicked! Value = " + selectedItemIndex);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        /*
        for (int i = 0; i < player.Items.Count; i++)
        {
            Debug.Log(player.Items[i].ItemBase.ItemName + ":" + player.Items[i].ItemCount);
        }*/
        // 衝突したのがアイテムだったら
        if (other.tag == "Item")
        {
            Item newItem = other.GetComponent<Item>();
            // 同じアイテムがあるか検索
            if (players[0].Items.Count > 0)
            {
                for (int i = 0; i < players[0].Items.Count; i++)
                {
                    if (newItem.ItemBase.Id == players[0].Items[i].ItemBase.Id)
                    {
                        // あったら個数を増やして破棄
                        players[0].Items[i].ItemCount++;
                        Destroy(other.gameObject);
                        return;
                    }
                }
            }
            // なければ新しく追加する
            players[0].Items.Add(other.GetComponent<Item>());
            players[0].Items[players[0].Items.Count - 1].ItemCount++;
            Debug.Log(players[0].Items[0].ItemBase.ItemName);
            Destroy(other.gameObject);
            // idが早い順に並べる
            players[0].Items.Sort((x, y) => y.Id - x.Id);
        }
        LoadItemData();
    }
    #endregion
    #region 
    // ステータス関係　=======================
    private void InitStatus()
    {
        // ステータス選択画面だったら
        if (currentGameStatus == GameStatus.STATUS)
        {
            playerStatusUIsManager.SetUpPlayerStatusUI(players, false);
            playerStatusUIsManager.selectStatus(selectedStatusIndex);
        }
        else if (currentGameStatus == GameStatus.ITEM)
        {
            // ここ
            HealItemBase healItemBase = players[0].Items[selectedItemIndex].ItemBase as HealItemBase;
            Debug.Log("healItemIsAll" + healItemBase.IsAll);
            playerStatusUIsManager.SetUpPlayerStatusUI(players, healItemBase.IsAll);
            if (healItemBase.IsAll == false)
            {
                playerStatusUIsManager.selectStatus(selectedItemTargetIndex);
            }
        }
    }
    // マウスでステータスを選択　または　アイテムを使用するターゲットの選択
    public void SelectStatusButton(int selectStatusIndex)
    {
        // ステータス選択画面だったら
        if (currentGameStatus == GameStatus.STATUS)
        {
            selectedStatusIndex = selectStatusIndex;
            //playerStatusUIsManager.selectStatus(selectedStatusIndex);
            // 選択
            // 決定キーを押したら
            Debug.Log(selectedStatusIndex);
        }
        else if (currentGameStatus == GameStatus.ITEM)
        {
            List<bool> usedItem = new List<bool>();
            HealItemBase healItemBase = players[0].Items[selectedItemIndex].ItemBase as HealItemBase;
            Debug.Log("えらばれているのは" + selectedItemIndex);
            // 全体アイテムだったら
            if (healItemBase.IsAll)
            {
                Debug.Log("全体");
                // アイテムを使う
                // アイテムの残りが0だったら
                if (players[0].Items[selectedItemIndex].ItemCount == 0)
                {
                    Debug.Log("アイテムがないから使えないよ");
                }
                else
                {
                    Item item = players[0].Items[selectedItemIndex];
                    // アイテムの効果発動
                    for (int i = 0; i < players.Count; i++)
                    {
                        // 全員falseだったらアイテムを使わない
                        usedItem.Add(players[i].TakeHeal(item));
                        players[i].playerUI.UpdateHpSp();
                    }
                    // アイテムの数を減らす
                    if (usedItem.All(value => value == true))
                    {
                        players[0].Items[selectedItemIndex].ItemCount--;
                    }
                    else
                    {
                        Debug.Log("体力が満タンで使えないよ");
                    }
                }
            }
            // アイテムだったらターゲットを選択する
            else
            {
                Debug.Log("個別");
                selectedItemTargetIndex = selectStatusIndex;
                // アイテムを使う
                // アイテムの残りが0だったら
                if (players[0].Items[selectedItemIndex].ItemCount == 0)
                {
                    Debug.Log("アイテムがないから使えないよ");
                }
                else
                {
                    Item item = players[0].Items[selectedItemIndex];
                    // アイテムの効果発動 体力が満タンでなければ
                    if (players[selectedItemTargetIndex].TakeHeal(item) == true)
                    {
                        players[selectedItemTargetIndex].playerUI.UpdateHpSp();
                        // アイテムの数を減らす
                        players[0].Items[selectedItemIndex].ItemCount--;
                    }
                    else
                    {
                        Debug.Log("体力が満タンで使えないよ");
                    }
                }
            }
        }
    }
    #endregion
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(vx, rb.velocity.y);

        if (jumpFlag == true)
        {
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            jumpFlag = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        groundFlag = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        groundFlag = false;
    }

}