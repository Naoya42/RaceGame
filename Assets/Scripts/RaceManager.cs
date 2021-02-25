using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    //どこからでも参照できるようにシングルトンにする
    public static　RaceManager instance;

    //チェックポイントを格納する配列
    public CheckPoint[] allCheckPoints;

    //周回するべき数
    public int totalLaps;

    public GameObject[] itemImages;//item画像

    public CarController playerCar;
    public List<CarController> allAICars = new List<CarController>();
    public int playerPositon;//Playerの順位
    public float timeBetweenPosCheck = .2f;//0.2秒ごとに順位チェックするための

    private float posChkCounter;//順位をチェックするための数値(0になったら順位を調べる)

    //AIの基本スピード、Playerの基本スピード、AIと差がついたときに引く数値、変化量δ
    public float aiDefaultSpeed = 30f, playerDefaultSpeed = 30f,rubberBandSpeedMod = 3.5f, rubberBandAccel =.5f;

    public bool isStarting, Appearance;//スタート判定＆お化け出現判定
    public float timeBetweenStartCount = 1f;
    private float startCounter;
    public int countdownCurrent = 3;

    //スタートのNo管理
    public int playerStartPosition,aiNumberToSpawn;
    //スタートの位置情報
    public Transform[] startPositions;
    //AIとしてリスポーンさせる車たち
    public List<CarController> carsToSpawns = new List<CarController>();
    //レースの終了判定
    public bool raceCompleted;

    //クリアしたレースのシーン名
    public string raceCompleteScene;


    public void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //設定の数値を周回数に反映
        totalLaps = RaceinfoManager.instance.noOfLaps;
        //設定の数値をAICarの数に反映
        aiNumberToSpawn = RaceinfoManager.instance.noOfAI;


        //チェックポイントに数を振り分ける(自動的に)
        for(int i = 0; i < allCheckPoints.Length; i++)
        {
            allCheckPoints[i].cpNo = i;
        }

        //スタート判定をいれる
        isStarting = true;
        Appearance = true;//お化け出現判定
        //カウンターを1秒にセット
        startCounter = timeBetweenStartCount;

        //UIにカウントダウンを反映する
        UIManager.instance.countDownText.text = countdownCurrent + "";

        //プレイヤーがリスポンする位置をランダムで設定
        playerStartPosition = Random.Range(0, aiNumberToSpawn);

        //PlayerCarを生成（ポジションをstartPositionsの指定位置に変更する
        playerCar = Instantiate(RaceinfoManager.instance.racerToUse, startPositions[playerStartPosition].position, startPositions[playerStartPosition].rotation);
        //aiCarではない設定
        playerCar.isAI = false;

        //プレイヤーだけAudioリスナーをオンにする
        playerCar.GetComponent<AudioListener>().enabled = true;

        //カメラのフォローをプレイヤーにする
        MainCamera.instance.SetTarget(playerCar);

        //PlayerCarのポジションをstartPositionsの指定位置に変更する
        /*playerCar.transform.position = startPositions[playerStartPosition].position;
        playerCar.rg.transform.position = startPositions[playerStartPosition].position;*/


        //スタート位置に車を配置(AI)
        for (int i = 0; i < aiNumberToSpawn + 1; i++)
        {
            //今の数値にプレイヤーがいない場合
            if(i != playerStartPosition)
            {
                //リスポンさせる車をランダムに選ぶ
                int selectedCar = Random.Range(0, carsToSpawns.Count);
                //リスポンさせつつ、レースで走る車のリストへ
                allAICars.Add(Instantiate(carsToSpawns[selectedCar],startPositions[i].position,startPositions[i].rotation));
                //車の種類が配置するべき車の数より多い場合
                if(carsToSpawns.Count > aiNumberToSpawn - 1)
                {
                    //削除する
                    carsToSpawns.RemoveAt(selectedCar);
                }

            }
        }


        //現在順位を反映
        UIManager.instance.rankText.text = (playerStartPosition + 1) + "/" + (allAICars.Count + 1);
    }

    // Update is called once per frame
    void Update()
    {
        //お化け出現時は戻る
        if(Appearance)
        {
            return;
        }

        //スタート時
        if(isStarting)
        {
            //マイフレームカウンターを減らす
            startCounter -= Time.deltaTime;

            //カウンターが0になったら
            if(startCounter <= 0)
            {
                //カウントダウンを１減らす
                countdownCurrent--;
                //カウンターを1秒にセット
                startCounter = timeBetweenStartCount;

                //UIにカウントダウンを反映する
                UIManager.instance.countDownText.text = countdownCurrent + "";

                //カウントダウン(3秒)が0秒になった時
                if (countdownCurrent == 0)
                {
                    isStarting = false;

                    //UIの表示と非表示を切り替える
                    UIManager.instance.countDownText.gameObject.SetActive(false);

                    UIManager.instance.goText.gameObject.SetActive(true);

                }
            }
        }


        //順位判定までにのタイマー。マイフレーム判定していては処理が重くなる
        posChkCounter -= Time.deltaTime;

        //0以下になったとき、順位を図る
        if(posChkCounter <= 0)
        {
            //順位
            playerPositon = 1;

            //AICarの情報を取得
            foreach (CarController aiCar in allAICars)
            {
                //playerよりlap数が上のAIがいるならば順位の数値を上げる
                if (aiCar.currentLap > playerCar.currentLap)
                {
                    playerPositon++;
                }
                //lap数が同じ場合は
                else if (aiCar.currentLap == playerCar.currentLap)
                {
                    //playerよりチェックポイントの数値がAIがいれば順位の数値を上げる
                    if (aiCar.nextCheckPoint > playerCar.nextCheckPoint)
                    {
                        playerPositon++;
                    }
                    //同じ区間を走ってる場合
                    else if (aiCar.nextCheckPoint == playerCar.nextCheckPoint)
                    {
                        //どちらが次のチェックポイントと比べて距離が近いのか
                        if (Vector3.Distance(aiCar.transform.position, allCheckPoints[aiCar.nextCheckPoint].transform.position)
                            < Vector3.Distance(playerCar.transform.position, allCheckPoints[aiCar.nextCheckPoint].transform.position))
                        {
                            playerPositon++;
                        }

                    }
                }
            }
            //時間経過をリセットする
            posChkCounter = timeBetweenPosCheck;

            //現在順位を反映
            UIManager.instance.rankText.text = playerPositon + "/" +(allAICars.Count + 1);
        }


        //ラバーバンド(輪ゴム：可変性があるAIのことでプレイヤーの独壇場を阻止する)
        //1位のとき
        if(playerPositon == 1)
        {
            //AIの調整
            foreach(CarController aiCar in allAICars)
            {
                //現在のスピードから、目的のスピードまで、引数３ずつ変化させる
                aiCar.maxSpeed = Mathf.MoveTowards(aiCar.maxSpeed,
                aiDefaultSpeed + rubberBandSpeedMod, rubberBandAccel * Time.deltaTime);
            }


            //Playerを現在のスピードから、目的のスピードまで、引数３ずつ変化させる
            playerCar.maxSpeed =
                Mathf.MoveTowards(playerCar.maxSpeed,
                playerDefaultSpeed - rubberBandSpeedMod, rubberBandAccel * Time.deltaTime);
        }
        //1位ではないとき
        else
        {
            //AIの調整
            foreach (CarController aiCar in allAICars)
            {
                //Playerを現在のスピードから、
                //目的のスピードまで((float型にしないと少数が切り捨てられる)順位が高い時ほどスピードが低くなるように)、引数３ずつ変化させる
                aiCar.maxSpeed = Mathf.MoveTowards(aiCar.maxSpeed,
                aiDefaultSpeed - (rubberBandSpeedMod * ((float)playerPositon / (float)allAICars.Count + 1)),
                rubberBandAccel * Time.deltaTime);
            }

            //Playerを現在のスピードから、
            //目的のスピードまで((float型にしないと少数が切り捨てられる)順位が低い時ほどスピードが出るようになる)、引数３ずつ変化させる
            /*playerCar.maxSpeed =
                Mathf.MoveTowards(playerCar.maxSpeed,
                playerDefaultSpeed + (rubberBandSpeedMod * ((float)playerPositon / (float)allAICars.Count + 1)),
                rubberBandAccel * Time.deltaTime);*/




        }
    }
    //終了判定の関数(ボタンに設置する)
    public void FinishRace()
    {
        
        raceCompleted = true;

        //プレイヤーの順位を表示
        UIManager.instance.raceResultText.text = "YOU RANK" + playerPositon + "";

        //ロックされたコースがあるならば
        /*if(RaceinfoManager.instance.trackToUnlock != "")
        {
            if (!PlayerPrefs.HasKey(RaceinfoManager.instance.trackToLoad + "_unlocked"))
            {
                PlayerPrefs.SetInt(RaceinfoManager.instance.trackToUnlock + "_unlocked", 1);
                UIManager.instance.trackUnlockedMassage.SetActive(true);
            }

        }*/

        //パネルを表示にする
        UIManager.instance.resultScreen.SetActive(true);
    }


    public void ExitRace()
    {
        SceneManager.LoadScene(raceCompleteScene);
    }

    public void Image(bool B,int x)
    {
        itemImages[x].SetActive(B);
    }
}
