using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CF_Event5 : CharactorFunction
{
    [SerializeField] private StoryEventScript storyEventScript;
    [SerializeField] private GameObject player_Field;
    [SerializeField] private GameObject player_Story;
    [SerializeField] private GameObject syo_FieldPrefab;
    [SerializeField] private GameObject syo_StoryPrefab;
    [SerializeField] private GameObject mao_StoryPrefab;
    [SerializeField] private GameObject boss_StoryPrefab;

    [SerializeField] private Transform FieldParent;
    [SerializeField] private Transform StoryParent;

    private GameObject syo;

    private GameObject mao;

    private GameObject boss;

    private GameObject sontyo;

    public override void ExecuteCommand(string functionName, string animFuncName)
    {
        if (functionName != null)
        {
            switch (functionName)
            {
                case "SpawnSyo_Field":
                    SpawanSyo_Filed();
                    break;
                case "SpawnSyo_Story":
                    SpawnSyo_Story();
                    break;
                case "SpawnMao_Story":
                    SpawnMao_Story();
                    break;
                case "SpawnBoss_Story":
                    SpawnBoss_Story();
                    break;

                case "Move2Village":
                    Move2Village();
                    break;
       
                case "BossMove":
                    StartCoroutine(BossMove());
                    break;
                case "SyoMove":
                    SyoMove();
                    break;
                case "SyoStop":
                    SyoStop();
                    break;
                case "SyoSecondMove":
                    SyoSecondMove();
                    break;
                case "MaoMove":
                    MaoMove();
                    break;

                case "MaoStop":
                    MaoStop();
                    break;
                case "MaoSecondMove":
                    MaoSecondMove();
                    break;

            }
        }

        if (animFuncName != null)
        {
            switch (animFuncName)
            {
                case "VecUp_Syo":
                    CharactorChangeVec(syo, "Up");
                    break;
            }
        }
    }

    /// <summary>
    /// ショウをフィールドシーン上に生成する
    /// </summary>
    private void SpawanSyo_Filed()
    {
        syo = SpawnCharactor(syo_FieldPrefab, player_Field.transform.position + new Vector3(-3f, 5f), FieldParent);
    }

    /// <summary>
    /// ショウをストーリーシーン上に生成する関数
    /// </summary>
    private void SpawnSyo_Story()
    {
        syo = SpawnCharactor(syo_StoryPrefab, player_Story.transform.position + new Vector3(-3f, 5f), StoryParent);
    }

    /// <summary>
    /// 村長をストーリーシーン上に生成する関数
    /// </summary>
    private void SpawnMao_Story()
    {
        mao = SpawnCharactor(mao_StoryPrefab, player_Story.transform.position + new Vector3(-2f, 5f), StoryParent);
    }

    private void SpawnBoss_Story()
    {
        boss = SpawnCharactor(boss_StoryPrefab, player_Story.transform.position + new Vector3(-3f, 5f), StoryParent);
    }

    /// <summary>
    /// 村へ移動する関数
    /// </summary>
    private void Move2Village()
    {
        SideView2TopDown();
    }

    /// <summary>
    /// maoが移動する関数
    /// </summary>
    private void MaoMove()
    {
        storyEventScript.moveFlag = true;
        Animator maoAnim = mao.GetComponent<Animator>();
        maoAnim.SetBool("isWalk", true);

        var maoPosition = mao.transform.position;
        mao.transform.DOMove(maoPosition - new Vector3(0, 4f, 0), 4f)
            .OnComplete(MaoStop); // アニメーションの完了時に SyoStop を呼び出す
        SyoMove();
        var maoCameraPosition=Camera.main.transform.DOMove(Camera.main.transform.position - new Vector3(0, 4f, 0), 4f);
        
    }

    private void MaoStop()
    {
        storyEventScript.moveFlag = true;
        Animator maoAnim = mao.GetComponent<Animator>();
        maoAnim.SetBool("isWalk", false);
    }


    private void MaoSecondMove()
    {
        storyEventScript.moveFlag = true;
        Animator maoAnim = mao.GetComponent<Animator>();
        maoAnim.SetBool("isWalk", true);

        var MaosecondPosition = mao.transform.position;
        var MaoStep = new Vector3(0, 1, 0);

        mao.transform.DOMove(MaosecondPosition - MaoStep * 4, 4f)
            .OnComplete(MaoStop);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
        

    }



    private void SyoMove()
    {
        //storyEventScript.moveFlag = true;
        Animator syoAnim = syo.GetComponent<Animator>();
        syoAnim.SetBool("isWalk", true);

        var syoPosition = syo.transform.position;
        syo.transform.DOMove(syoPosition - new Vector3(0, 4f, 0), 4f)
            .OnComplete(SyoStop); // アニメーションの完了時に SyoStop を呼び出す
        Camera.main.transform.DOMove(Camera.main.transform.position - new Vector3(0, 4f, 0), 4f);

    }

    private void SyoStop()
    {
        storyEventScript.moveFlag = true;
        Animator syoAnim = syo.GetComponent<Animator>();
        syoAnim.SetBool("isWalk", false);
    }


    private void SyoSecondMove()
    {
        storyEventScript.moveFlag = true;
        Animator syoAnim = syo.GetComponent<Animator>();
        syoAnim.SetBool("isWalk", true);

        var SyosecondPosition = syo.transform.position;
        var SyoStep = new Vector3(0, 1, 0);

        syo.transform.DOMove(SyosecondPosition - SyoStep * 4, 4f)
            .OnComplete(SyoStop);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();

    }

    private IEnumerator BossMove()
    {


        storyEventScript.moveFlag = true;
        Animator bossAnim = boss.GetComponent<Animator>();
        bossAnim.SetBool("isWalk", true);

        var bossPosition = boss.transform.position;
        var OneStep = new Vector3(0, 2f, 0);

        // カメラの初期位置を新しい位置にリセット
        var cameraInitialPosition = new Vector3(1f, 5f, Camera.main.transform.position.z);

        Camera.main.transform.position = cameraInitialPosition;

        // カメラのオフセットを設定
        var cameraOffset = Camera.main.transform.position - bossPosition;

        for (int i = 1; i <= 3; i++)
        {

            boss.transform.DOMove(bossPosition - OneStep * i, 2f);
            // カメラを動かす
            Camera.main.transform.DOMove(bossPosition - OneStep * i + cameraOffset, 2f)
                .OnComplete(() =>
                {
                // カメラを揺らす
                Camera.main.DOShakePosition(0.7f, 2f); // 0.5秒間、強度2で揺らす
            });


            yield return new WaitForSeconds(2);
            bossAnim.SetBool("isWalk", false);
            yield return new WaitForSeconds(2);
            bossAnim.SetBool("isWalk", true);
        }

        bossAnim.SetBool("isWalk", false);
        yield return new WaitForSeconds(2);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }



    /// <summary>
    /// 移動が完了したら実行する関数
    /// </summary>
    private void moveCompleteFunc()
    {
        Animator anim = sontyo.GetComponent<Animator>();
        anim.SetBool("isWalk", false);
        storyEventScript.moveFlag = false;
        storyEventScript.ReadNextMessage();
    }
}
