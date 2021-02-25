using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disable_overTime : MonoBehaviour
{
    //GoTextの表示時間
    public float timeToDisable;

    // Update is called once per frame
    void Update()
    {
        //フレームごとに経過した時間分を減らす
        timeToDisable -= Time.deltaTime;
        //0になったら
        if(timeToDisable <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
