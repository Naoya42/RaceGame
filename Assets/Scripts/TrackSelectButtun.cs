using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackSelectButtun : MonoBehaviour
{
    //コースの名前
    public string trackSceneName;

    //レースのプレビュー画像
    public Image trackImage;

    //周回数
    public int raceLap = 3;

    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //選んだシーンを伝える？
    public void SelectTrack()
    {
        
            //コース名
            RaceinfoManager.instance.trackToLoad = trackSceneName;
            //周回数
            RaceinfoManager.instance.noOfLaps = raceLap;
            //トラックのイメーじを設定
            RaceinfoManager.instance.trackSprite = trackImage.sprite;
            //画像
            TitleManager.instance.trackSelectImage.sprite = trackImage.sprite;
            //パネルを閉じる
            TitleManager.instance.CloseTrackSelect();
            
    }
}
