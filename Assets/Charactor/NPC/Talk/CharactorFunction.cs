using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorFunction : MonoBehaviour
{
    public virtual void ExecuteCommand(string functionName,string animFuncName){

    }

    /// <summary>
    /// キャラクターの画像の向きとアニメーションを変更する
    /// </summary>
    /// <param name="charactor">キャラクターのゲームオブジェクト</param>
    /// <param name="vec">向き</param>
    protected void CharactorChangeVec(GameObject charactor, string vec){

        Animator anim = charactor.GetComponent<Animator>();

        switch(vec){
            case "Up":
                anim.SetFloat("x",0);
                anim.SetFloat("y",1);
                break;
            case "down":
                anim.SetFloat("x",0);
                anim.SetFloat("y",-1);
                break;
            case "Right":
                anim.SetFloat("x",1);
                anim.SetFloat("y",0);
                break;
            case "Left":
                anim.SetFloat("x",-1);
                anim.SetFloat("y",0);
                break;
        }
    }
    /// <summary>
    /// キャラクターオブジェクトを生成する関数
    /// </summary>
    /// <param name="gameObject">生成するゲームオブジェクト</param>
    /// <param name="Position">生成する位置</param>
    /// <param name="parent">生成するシーンのオブジェクト</param>
    /// <returns></returns>
    protected GameObject SpawnCharactor(GameObject gameObject, Vector3 Position, Transform parent){
        GameObject charactor;
        charactor = Instantiate(gameObject,Position,Quaternion.identity,parent);
        return charactor;
    }

    /// <summary>
    /// キャラクターを指定した位置に移動する
    /// </summary>
    /// <param name="gameObject">動かしたいキャラクター</param>
    /// <param name="position">移動させたい位置</param>
    protected void WarpCharactor(GameObject gameObject, Vector3 position){
        gameObject.transform.position = position;
    }

    /// <summary>
    /// まちに移動する
    /// </summary>
    protected void SideView2TopDown(){
        GameManager.instance.CurrentSceneIndex = (int)GameMode.TOWN_SCENE;
    }

    
}