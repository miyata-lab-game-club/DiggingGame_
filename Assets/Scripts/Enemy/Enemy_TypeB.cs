using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Enemy_TypeB : FieldEnemy
{

#if UNITY_EDITOR
    [CustomEditor(typeof(FieldEnemy))]
#endif

    [SerializeField] private Rigidbody2D rb; //リジッドボディを参照
    [SerializeField] private float speed; //移動スピード
    [SerializeField] private float stepDis; //通常時の移動距離
    [SerializeField] private float jumpPower;//ジャンプパワー
    [SerializeField] private float searchAngle = 45.0f; //サーチ角度 
    [SerializeField] private float chaseTime;//追跡時間

    private GameObject player; //プレイヤーのゲームオブジェクト

    private float vx; //移動ベロシティx
    private float vy; //移動ベロシティy
    private float stopingTime; //停止している時間
    private float chaseTimer;//追跡時間タイマー
    private bool moveFlag = true; //移動可能か
    private bool waitFlag = false; //待機中か
    private bool leftFlag = true; //左方向を向いているか
    private bool jumpFlag = false;
    private bool chaseFlag = false;
    private Vector3 moveStartPos; //移動開始位置
    private Vector3 previousPos; //前フレームの位置
    private IEnumerator wait; //待機関数
    private Coroutine waitCoroutine; //待機用コルーチン


    protected override void Start()
    {
        base.Start();
        moveStartPos = transform.position;
        previousPos = transform.position;
        vx = -speed;
    }

    protected override void Update()
    {
        base.Update();

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void FieldMove()
    {
        bool isHitWall = hitWallCheck();
        bool isOverStepDis = WalkDistanceCheck();

        if (chaseFlag == true) //追跡フラグがtrueなら
        {
            chaseTimer += Time.deltaTime;//追跡時間を追加
            if (chaseTimer > chaseTime)
            {
                chaseFlag = false;//追跡終了
                moveStartPos = transform.position; //移動開始位置をリセット
                chaseTimer = 0;
                return;
            }

            if (waitCoroutine != null)
            { //Waitコルーチンのnullチェック
                StopCoroutine(waitCoroutine);//Waitコルーチンを止める
                wait = null; //コルーチンを破棄
            }

            moveFlag = true;
            waitFlag = false;

            if (player.transform.position.x < transform.position.x && leftFlag == false)
            {
                Flip();
                leftFlag = !leftFlag;
            }
            else if (player.transform.position.x > transform.position.x && leftFlag == true)
            {
                Flip();
                leftFlag = !leftFlag;
            }
        }
        else
        {
            if (isHitWall || isOverStepDis) //壁に当たる、もしくは移動可能量を上回ったら
            {
                moveFlag = false; //いったん待機

                if (waitFlag == false) //待機フラグが立っていないなら
                {
                    wait = Wait(1, () => //1秒後に方向転換するコルーチン
                    {
                        Flip(); //キャラの左右反転
                        leftFlag = !leftFlag; //方向転換
                        waitFlag = false; //待機解除
                        moveStartPos = transform.position; //移動開始位置をリセット
                    });
                    waitCoroutine = StartCoroutine(wait);//Waitコルーチンをスタート

                    waitFlag = true; //待機フラグをtrue
                }
            }
            else
            {
                moveFlag = true;
            }
        }

        if (StopCheck() == true && waitFlag == false) //壁に当たってない＆＆移動可能距離内なのに止まってたら
        {
            jumpFlag = true;
        }

        //Debug.Log(moveFlag);
        if (moveFlag)
        {
            if (leftFlag == true)
            {
                if (chaseFlag == true)
                {
                    vx = -speed * 1.3f; //追跡中なら加速
                }
                else
                {
                    vx = -speed;
                }
            }
            else
            {
                if (chaseFlag == true)
                {
                    vx = speed * 1.3f;//追跡中なら加速
                }
                else
                {
                    vx = speed;
                }
            }

            rb.velocity = new Vector2(vx, rb.velocity.y);
            if (jumpFlag == true)
            {
                rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
                jumpFlag = false;
            }
        }
        else
        {
            vx = 0;
            vy = 0;
            rb.velocity = new Vector2(vx, rb.velocity.y);
        }

        //Debug.Log(rb.velocity);
        //Debug.Log(Vector2.Distance(transform.position,previousPos));
        previousPos = transform.position;
    }

    protected override void Search()
    {
        base.Search();
    }

    private void Flip()
    {
        Vector3 myScale = transform.localScale;
        myScale = new Vector3(-myScale.x, myScale.y, myScale.z);
        transform.localScale = myScale;
    }

    private bool hitWallCheck()
    {
        bool isHit = false;
        Vector2 rayDir;

        if (leftFlag)//どちらを向いているか
        {
            rayDir = -transform.right;
        }
        else
        {
            rayDir = transform.right;
        }

        Ray2D ray = new Ray2D(transform.position, rayDir); //レイを作成
        LayerMask layerMask = ~(1 << 9) & ~(1 << 10); //レイヤーマスクを指定(自分と視界のコリジョンに当たらないように)
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 0.7f, layerMask); //レイを発射
        Debug.DrawRay(ray.origin, ray.direction * 0.7f, Color.green, 0.015f);

        //障害物があればtrueを返す
        if (hit)
        {
            isHit = true;
            //Debug.Log(hit.collider.gameObject);
        }
        else
        {
            isHit = false;
        }

        return isHit;
    }

    private bool WalkDistanceCheck()
    {
        float dis = Math.Abs(this.transform.position.x - moveStartPos.x);
        //Debug.Log(dis);
        //Debug.Log(dis > stepDis);

        if (dis > stepDis)
        { //ステップ距離より移動量が大きければ
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool StopCheck()
    {

        if (0 == Vector2.Distance(transform.position, previousPos)) //移動距離がゼロなら
        {
            stopingTime += Time.deltaTime;  //タイマーに値を追加
            if (stopingTime > 0.2) //タイマーが規定秒数を上回ったらなら
            {
                stopingTime = 0;
                return true;
            }
            return false;
        }
        else
        {
            stopingTime = 0;
            return false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")//コライダー内にプレイヤーが入ったら
        {
            if (chaseFlag == false)
            {
                Vector2 toPlayer = other.transform.position - transform.position;
                Vector2 myDir;

                if (leftFlag)
                {
                    myDir = -transform.right;
                }
                else
                {
                    myDir = transform.right;
                }

                float playerAngle = Vector2.Angle(myDir, toPlayer);

                if (searchAngle > playerAngle)
                {//サーチ（視界）内にいるか確認
                    //Debug.Log("視界の範囲内");

                    LayerMask layerMask = ~(1 << 10) & ~(1<<11); //レイヤーマスクを指定(自分と視界のコリジョンに当たらないように)
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer.normalized, toPlayer.magnitude, layerMask);//プレイヤーへレイを飛ばす
                    Debug.DrawRay(transform.position, toPlayer.normalized * toPlayer.magnitude, Color.red, 0.015f);

                    if (hit.collider == other)
                    {
                        player = other.gameObject;
                        chaseFlag = true;
                    }
                }
            }
        }
    }

    private IEnumerator Wait(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }
}
