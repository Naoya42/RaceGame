using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointChecker : MonoBehaviour
{
    //Carcontrollerを取得
    public CarController theCar;

    private void OnTriggerEnter(Collider other)
    {
        //当たり判定
        if(other.tag == "Checkpoint")
        {
            
            //コンソールに当たったチェックポイントのNoを表示する
            //Debug.Log("Hit cp" + other.GetComponent<CheckPoint>().cpNo);

            //チックポイントの関数を呼び出す(引数：ぶつかったチェックポイントの番号)
            theCar.CheckpointHit(other.GetComponent<CheckPoint>().cpNo);
        }
    }
}
