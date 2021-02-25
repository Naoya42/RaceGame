using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{

    //セレクト画面、
    public GameObject raceSetupPanael, trackSelectPanel, racerSelectPanel;

    //選択中のコースと車の画像
    public Image trackSelectImage, racerSelectImager;

    public static TitleManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //すでにレースを一度遊んでいる状態ならq
        if(RaceinfoManager.instance.enteredRace)
        {
            //現在選択中のコースと車をパネルに設定
            trackSelectImage.sprite = RaceinfoManager.instance.trackSprite;
            racerSelectImager.sprite = RaceinfoManager.instance.racerSprite;

            //パネルを開く************************後で修正予定：現状ゲーム開始と同時にレースメニューを開いてしまう
            OpenRaceSetup();
        }
        //ロードの禁止？初だから調べる
        //PlayerPrefs.SetInt(RaceinfoManager.instance.trackToLoad + "_unlocked", 1);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        //判定
        RaceinfoManager.instance.enteredRace = true;

        //指定したシーンに画面を遷移させる
        SceneManager.LoadScene(RaceinfoManager.instance.trackToLoad);
    }
    public void QuitGame()
    {
        //アプリケーション終了のお知らせ
        Application.Quit();

        Debug.Log("Game Quit");
    }

    //セットアップ画面を開閉する関数
    public void OpenRaceSetup()
    {
        raceSetupPanael.SetActive(true);
    }
    public void CloseRaceSetup()
    {
        raceSetupPanael.SetActive(false);
    }

    //コースを選択する画面を開閉する関数
    public void OpenTrackSelect()
    {
        trackSelectPanel.SetActive(true);
        //パネルが二重で開かれないように前の選択画面は閉じる
        CloseRaceSetup();
    }
    public void CloseTrackSelect()
    {
        trackSelectPanel.SetActive(false);
        //前のパネルに戻ため開く
        OpenRaceSetup();
    }

    //車を選択する画面を開閉する関数
    public void OpenRacerSelect()
    {
        racerSelectPanel.SetActive(true);
        //パネルが二重で開かれないように前の選択画面は閉じる
        CloseRaceSetup();
    }
    public void CloseRacerPanel()
    {
        racerSelectPanel.SetActive(false);
        //前のパネルに戻ため開く
        OpenRaceSetup();
    }
}
