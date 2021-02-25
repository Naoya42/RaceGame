using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceinfoManager : MonoBehaviour
{
    //どこからでも呼び出せるように
    public static RaceinfoManager instance;

    //MAPの選択
    public string trackToLoad;
    //車の選択
    public CarController racerToUse;
    //aiの数
    public int noOfAI;
    //何周走るか
    public int noOfLaps;
    //クリア後にメニュー画面に戻る
    public bool enteredRace = false;
    public Sprite trackSprite, racerSprite;

    //public string trackToUnlock;
    

    public void Awake()
    {
        //インスタンスに何も設定されていない場合
        if (instance == null)
        {
            //インスタンスに設定
            instance = this;

            //シーン遷移で破壊不可能
            DontDestroyOnLoad(gameObject);
        }
        //何かが設定されている場合
        else
        {
            //破壊
            Destroy(gameObject);
        }
    }
}
