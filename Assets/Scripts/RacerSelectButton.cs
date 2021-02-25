using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RacerSelectButton : MonoBehaviour
{
    //車の画像
    public Image racerImage;

    //使用する車をセットするための変数
    public CarController racerToSet;


   public void SelectRacer()
    {
        //使用する車を設定
        RaceinfoManager.instance.racerToUse = racerToSet;
        RaceinfoManager.instance.racerSprite = racerImage.sprite;

        //選択した車の画像をセレクト画面に反映
        TitleManager.instance.racerSelectImager.sprite = racerImage.sprite;
        //選択したらレーサーパネルを閉じる
        TitleManager.instance.CloseRacerPanel();



    }
}
