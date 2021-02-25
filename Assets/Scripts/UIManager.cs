using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    //どこからでもアクセスできるようにシングルトンに
    public static UIManager instance;

    //テキスト　現在のラップ数、最速のラップタイム、現在走っているラップのタイム、順位,カウントダウン、出発、結果表示テキスト
    public TMP_Text lapCounterText, bestLapTimeText,curentLapTimerText,rankText,countDownText,goText,raceResultText;

    //クリアパネル,一時停止画面,コースロック解除メッセージ
    public GameObject resultScreen,pauseScreen,trackUnlockedMassage;
    //ポーズ判定
    public bool isPaused;

    private void Awake()
    {
        //インスタンスにセット
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ESCが押されたらポーズパネルを開く
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnPause();
        }
    }
    //
    public void ExitRace()
    {
        //ゲーム内時間の設定(pause画面でゲームを抜けると0にされたままなのでここで修正)
        Time.timeScale = 1f;
        RaceManager.instance.ExitRace();
    }

    public void PauseUnPause()
    {
        //現在のboolとは反対にする
        isPaused = !isPaused;

        pauseScreen.SetActive(isPaused);


        if(isPaused)
        {
            //ゲーム内時間の設定
            Time.timeScale = 0;
        }
        else
        {
            //ゲーム内時間の設定
            Time.timeScale = 1f;
        }

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
