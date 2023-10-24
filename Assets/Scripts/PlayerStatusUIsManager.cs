using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public delegate void StatusSelectButtonClickedDelegate(int statusIndex);
public class PlayerStatusUIsManager : MonoBehaviour
{
    private int playersCount = 0;
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject playerStatusUIsManager;
    private List<PlayerFieldUI> playerFieldUIs;

    public StatusSelectButtonClickedDelegate statusSelectButtonClickedDelegate;
    public void SetUpPlayerStatusUI(List<Player> players)
    {
        // プレイヤーの人数が変わったときだけ更新する
        if (players.Count != playersCount)
        {
            // 動作確認まだ
            for (int i = 0; i < playersCount; i++)
            {
                Destroy(playerFieldUIs[i]);
            }
            playerFieldUIs = new List<PlayerFieldUI>();
            // ステータス画面のPrefabをInstantiateする
            for (int i = 0; i < players.Count; i++)
            {
                GameObject playerFieldUIObject = Instantiate(playerUIPrefab, playerStatusUIsManager.transform);
                PlayerFieldUI playerFieldUI = playerFieldUIObject.GetComponent<PlayerFieldUI>();
                // ステータスUIマネジャーに追加する
                playerFieldUIs.Add(playerFieldUI);
                // プレイヤーのUIに追加
                players[i].playerUI = playerFieldUI;
                // ステータス画面に情報を設定する
                playerFieldUI.SetPlayerStatus(players[i]);
                // UIの更新
                players[i].playerUI.UpdateHpSp();
                // ボタンに関数の追加
                Button button = playerFieldUIObject.GetComponent<Button>();
                int buttonIndex = i;
                button.onClick.AddListener(() => OnClickStatus(buttonIndex));
                EventTrigger eventTrigger = playerFieldUIObject.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((eventDate) => selectStatus(buttonIndex));
                eventTrigger.triggers.Add(entry);
            }
        }
        playersCount = players.Count;
    }

    public void selectStatus(int index)
    {
        // フレームをオフ
        for (int i = 0; i < playerFieldUIs.Count; i++)
        {
            playerFieldUIs[i].SetActivateSelectedFrame(false);
        }
        playerFieldUIs[index].SetActivateSelectedFrame(true);
    }

    // 全選択
    public void selectStatusAll(int index)
    {
        // フレームをオン
        for (int i = 0; i < playerFieldUIs.Count; i++)
        {
            playerFieldUIs[i].SetActivateSelectedFrame(true);
        }
    }
    // ステータス画面を開いているとき、キャラクターを選んだときに呼ばれる関数
    public void OnClickStatus(int statusIndex)
    {
        if (statusSelectButtonClickedDelegate != null) statusSelectButtonClickedDelegate(statusIndex);
    }
}