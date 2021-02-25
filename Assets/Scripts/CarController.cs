using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody rg;//剛体格納
    public float maxSpeed;//最高速度の制限
    public float forwardAccel = 8f, reverseAccesl = 4f;//アクセルとブレーキ
    float turnStrength = 150f;//回転するときに加える力の数値
    public Transform groundRayPoint, groundRayPoint2;//タイヤが接触している面は、どのレイヤーか判断するオブジェクトの位置？
    public LayerMask whatIsGround;//確認するレイヤーを取得する(エディターで設定する)
    public float groundRayLength = .75f;//
    public bool grounded;//接触判定
    public float gravityMod = 10f;//空中で加えられる力
    public Transform leftFrontWheel, rightFrontWheel;//Carオブジェクトのbpdyについている前輪のポジション
    public float maxWheerTurn = 25f;//前輪を回転させる角度
    public AudioSource enginSound, skitSound;//エンジン音,ドリフト音
    public float skidFadeSpeed;
    public int currentLap;//現在のラップ数
    public float lapTime, bestLapTime;//ラップで経過しているtime,一番早いtime
    public bool isAI;//AIが操作するのか判定
    public int currentTarget;//現在目指すべきチェックポイントの数値

    //AIのアクセルスピード、AIの曲がる時のスピード、チェックポイントとの許容範囲距離、チェックポイントの位置をランダム化する数値,曲がる角度の最大値
    public float aiAccelerateSpead = 1f, aiTurnSpeed = .8f, aiReachPointRange = 5f, aiPointVariance = 3f, aiMaxTurn = 15f;
    public int nextCheckPoint;//次のチェックポイントを指定
    public float resetCooldown = 2f;//リセットまでの感覚


    private float turnInput;//Horizontal入力の値を受け取る
    private float speedInput;//移動のために力を加える数値
    private float dragOnGround;//抗力(数値が高ければ止まりやすくなるやつ)
    private Vector3 targetPoint;//AI用の次のチェックポイントの位置

    //AI用の移動のために力を加える数値,スピードに差をつけるための数値
    private float aiSpeedInput, aiSpeedMod;

    private float resetCounter;//リセットボタンのクールタイム

    //アイテム系
    public bool item_speedUp = false;
    public bool item_slip = false;
    public GameObject FierPoint_Front, FierPoint_Buck;

    public GameObject[] items;
    bool itemCheck;//アイテム所持判定
    int randomIndex;//所持しているアイテムの番号

    bool startBounas = false;//スタートダッシュボーナス判定
    bool firstStart = false;

    
    float startTime = 3f, Addspeed = 1.8f;//スタートダッシュボーナスに使用する数値、アイテムが進むスピードに追加する数値


    public GameObject carBody;

    // Start is called before the first frame update
    void Start()
    {
        
        itemCheck = false;

        //rgの位置情報を親依存から抜け出させる？
        rg.transform.parent = null;

        //抗力を設定
        dragOnGround = rg.drag;

        //AIの最初の目的地を設定
        if (isAI)
        {
            targetPoint = RaceManager.instance.allCheckPoints[currentTarget].transform.position;
            //少しランダム化
            RandomiseAITarget();

            //数値をランダムセット
            aiSpeedMod = Random.Range(.8f, 1.1f);

            //エンジン音を消す
            enginSound.Stop();
        }

        //現在のラップ数に反映
        UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;

        //ゲーム開始と同時にクールダウンの秒数設定
        resetCounter = resetCooldown;
    }

    // Update is called once per frame
    void Update()
    {

        if (RaceManager.instance.isStarting && Input.GetKey(KeyCode.W))//スタートダッシュ関係
        {
            startTime -= Time.deltaTime;

            if (enginSound.pitch < 2.5)
            {
                enginSound.pitch += 0.01f;
            }
            
        }
        else
        {
            if(enginSound.pitch > 0)
            {
                enginSound.pitch -= 0.01f;
            }
            
        }


        if(!firstStart && !RaceManager.instance.isStarting)
        {
            if (startTime > 0 && startTime < 1.5f)
            {
                startBounas = true;
            }
            firstStart = true;
        }

            //スタートカウント時は動けない
        if (!RaceManager.instance.isStarting)
        {
            

        //ラップタイムにフレーム間で経過した時間を足す
        lapTime += Time.deltaTime;

           


            if (!isAI)
            {
            

            //システムから経過した時間を取得
            var ts = System.TimeSpan.FromSeconds(lapTime);
            UIManager.instance.curentLapTimerText.text = string.Format("{0:00}m{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);


            //マイフレーム移動スピード初期化
            speedInput = 0f;

            //入力が+の場合はspeedInputにアクセルの数値をかける
            if (Input.GetAxis("Vertical") > 0)
            {
                speedInput = Input.GetAxis("Vertical") * forwardAccel;
            }
            //入力が-の場合はspeedInputにブレーキの数値をかける
            else if (Input.GetAxis("Vertical") < 0)
            {
                speedInput = Input.GetAxis("Vertical") * reverseAccesl;
            }

            //毎フレーム横軸の入力を受け取る
            turnInput = Input.GetAxis("Horizontal");


            if (resetCounter > 0)
            {
                //リセットのクールタイムの減少
                resetCounter -= Time.deltaTime;
            }
            //最後のチェックポイント
            if (Input.GetKeyDown(KeyCode.R) && resetCounter <= 0)
            {
                ResetToTrack();
            }

            
            //アイテム所持の判定と仕様関数を呼ぶコード記述
            if(Input.GetMouseButtonDown(0) && itemCheck)
                {
                    UseItem(5);
                }

            else if(Input.GetMouseButtonDown(1) && itemCheck)
                {
                    UseItem(-5);
                }
            }

            //AIの場合
            else
            {
            //現在位置と目的地の高さは同じに設定
            targetPoint.y = transform.position.y;

            //目的地と現在位置の距離よりaiReachPointが大きい場合は
            if (Vector3.Distance(transform.position, targetPoint) < aiReachPointRange)
            {
                //次のチェックポイントを設定
                SetNextAITarget();
            }

            //ターゲットとの距離を取得
            Vector3 targetDir = targetPoint - transform.position;
            //ターゲットとAICarの正面からの角度を取得(transform.forwardは今オブジェクト(正面)が向いている方にオブジェクトを動かす)
            float angle = Vector3.Angle(targetDir, transform.forward);
            //ターゲットとの距離をローカル座標に置き換える(全体座標からその距離を親として見た場合の座標)
            Vector3 localPos = transform.InverseTransformPoint(targetPoint);

            //x軸が目的地とずれている場合
            if (localPos.x < 0f)
            {
                //ずれている場合は目的地から左にそれていることになる。
                //なのでそのまま角度分引いてあげて近づく
                angle = -angle;
            }

            //横軸の値&角度(angleの数値を調整、最小、最大値)
            turnInput = Mathf.Clamp(angle / aiMaxTurn, -1f, 1f);

            //角度の絶対値よりaiMAXTrunが大きいとき(曲がれる角度に余裕がある時)
            if (Mathf.Abs(angle) < aiMaxTurn)
            {
                //曲がりながらスピードを上げる(現在のスピード、目的のスピード、変化量(1f))
                aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, 1f, aiAccelerateSpead);
            }
            //曲がれる角度に余裕がない時
            else
            {
                //曲がりながらスピードを下げる(現在のスピード、目的のスピード(0.8)、変化量(1f))
                aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, aiTurnSpeed, aiAccelerateSpead);
            }

            //AIの車を直進させる(最後のはスピードに差をつける数値)
            speedInput = aiSpeedInput * forwardAccel * aiSpeedMod;

                if(itemCheck)
                {
                    UseItem(5);
                }
                
            }

        

        //スピードに合わせて、音の高さを上げる&&AIではないとき(AIの車うるさすぎw)
        if (enginSound != null && !isAI)
        {
            enginSound.pitch = 0.3f + ((rg.velocity.magnitude / maxSpeed) * 2f);
                Debug.Log(enginSound.pitch);
        }

        //ドリフト音が設定されている時&&AIではないとき(AIの車うるさすぎw)
        if (skitSound != null && !isAI)
        {
            //turnInputの絶対値が0.1以上の場合
            if (Mathf.Abs(turnInput) > 0.8f && grounded && rg.velocity.magnitude > 24)
            {
                skitSound.volume = 0.3f;
            }
            else
            {
                //turnINputの入力値が少ない場合は、徐々に音を下げる
                //(現在の音量、目的の音量、最大の音量変化量)
                skitSound.volume = Mathf.MoveTowards(skitSound.volume, 0f, skidFadeSpeed * Time.deltaTime);
            }
        }

        }


        

    }

    private void FixedUpdate()
    {
        //地面と接触しているか判定
        grounded = false;

        RaycastHit hit;

        //角度０
        Vector3 normalTarget = Vector3.zero;

        //当たり判定の確認1（元の位置、確認するべきポイント、長さ、どのレイヤーを見るか）
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            //衝突した面の角度？(vector3の型で取得)。平面なら０のまま
            normalTarget = hit.normal;
        }
        //当たり判定の確認2（元の位置、確認するべきポイント、長さ、どのレイヤーを見るか）
        if (Physics.Raycast(groundRayPoint2.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            //衝突した面の角度？(vector3の型で取得)
            //後輪部分の接触があれば上書き(前輪部分＋後輪の当たり判定を2で割ることで平均を出す)
            //こうすれば角度の差があっても対応できる
            normalTarget = (normalTarget + hit.normal) / 2f;
        }

        //地面にいて角度が変わる時？(坂を登る時)
        if (grounded)
        {
            //角度を変更する　＝　角度を変更する（開始角度(緑の軸)、目的の角度）＊現在の角度をかける
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
        }

        //地面に接触している時
        if (grounded)
        {
            rg.drag = dragOnGround;

            //rgに入力から得られた数値分力を加える(forwardはVector3とQuaternionを足したものになる。なのでこれで曲がれる)
            if(!startBounas && !RaceManager.instance.isStarting)
            {
                
                rg.AddForce(transform.forward * speedInput * 1000f);
            }
            else if(startBounas && !RaceManager.instance.isStarting)//スタート後かつボーナス真偽値がtrueの時
            {
                
                rg.velocity = new Vector3(0,0,1) * maxSpeed * 1.5f;
                startBounas = false;
            }
            
        }
        //空中にいる時
        else
        {
            //抗力を指定
            rg.drag = .1f;

            //下に引っ張る力を加える
            rg.AddForce(-Vector3.up * gravityMod * 100f);
        }

        //ベロシティーの数値(ベクター３型の移動速度.距離)がマックススピードより大きい場合
        if (rg.velocity.magnitude > maxSpeed && item_speedUp == false)
        {
            rg.AddForce(new Vector3(0,0,-200f));
            //Debug.Log(rg.velocity.magnitude);
            //rg.velocity = rg.velocity.normalized * maxSpeed;//velocityがmaxspeedを超えないように設定
        }
        



        //位置を合わせている
        transform.position = rg.position;


        //縦入力がある時(少しでも前に動いている時)
        if (grounded && speedInput /*Input.GetAxis("Vertical")*/ != 0)
        {
            //回転をいじっている(現在の回転＋yを軸にした回転)。
            //time.deltatimeは1フレームの経過時間。つまり1フレームの変化時間を掛けている
            //Mathf.Sigh(float f)は数値が+なら１、ーなら-1を返す。
            //velocityはベクトルを扱える(物理特性の影響を受けない動きができる。スピード出ていれば出ているほど最大の力で回れる
            //マグニチュードはベクトルの (x *x+y* y+z* z) の平方根の長さを返します　　　　turnStrengthの数値変えれば操作性変わりそう
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (rg.velocity.magnitude / maxSpeed), 0f));
        }
    }

    public void Kinoko()
    {
        
        rg.velocity = rg.velocity.normalized * maxSpeed * 3f;
    }

    public void Banana()
    {
        if(!isAI)
        {
            MainCamera.instance.ResetTarget();
            Invoke("Refollow", 0.4f);
        }
        StartCoroutine(Slip());
    }

    public void Koura()
    {
        if (!isAI)
        {
            MainCamera.instance.ResetTarget();
            Invoke("Refollow", 0.4f);
        }
        StartCoroutine(SlipKoura());
    }

    //カメラのフォローを戻す
    public void Refollow()
    {
        MainCamera.instance.SetTarget(this);
    }




//次のチェックポイントを指定
    public void CheckpointHit(int cpNo)
    {
        //通過したチェックポイントとネクストチェックポイントが等しいか
        if (cpNo == nextCheckPoint)
        {

            nextCheckPoint++;

            //ネクストチェックが最後だったら
            if (nextCheckPoint == RaceManager.instance.allCheckPoints.Length)
            {
                //次のチェックポイントを0にする
                nextCheckPoint = 0;

                //ラップ数を追加する
                LapCompleted();
            }
        }
        //AIの場合
        if (isAI)
        {
            //AIが目的のチェックポイントに触れた場合
            if (cpNo == currentTarget)
            {
                //関数を呼ぶ
                SetNextAITarget();
            }
        }
    }

    //AIのチェックポイントを設定
    public void SetNextAITarget()
    {
        //次のターゲットへ
        currentTarget++;
        //チェックポイントの数値が配列の最後の要素だったら
        if (currentTarget >= RaceManager.instance.allCheckPoints.Length)
        {
            currentTarget = 0;
        }

        //AIの目的地を設定&少しランダム化
        targetPoint = RaceManager.instance.allCheckPoints[currentTarget].transform.position;
        RandomiseAITarget();
    }

    //周回した時の処理
    public void LapCompleted()
    {
        //ラップ数の増加
        currentLap++;

        //この時タイムを更新または初の周回の時にベストタイムを更新する
        if (lapTime < bestLapTime || bestLapTime == 0)
        {
            bestLapTime = lapTime;
        }

        //現在の周回数が最後ではない時
        if (currentLap <= RaceManager.instance.totalLaps)
        {

            //次の周回時はラップタイムを初期化する
            lapTime = 0f;

            if (!isAI)
            {
                //システムから経過した時間を取得？
                var ts = System.TimeSpan.FromSeconds(bestLapTime);
                //UIに経過時間を表示
                UIManager.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);


                //現在のラップ数に反映
                UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;


            }
        }
        //最後の周回だった場合
        else
        {
            if (!isAI)
            {
                //ラップ終了後は自動運転！
                isAI = true;

                aiSpeedMod = 1f;

                //エンジン音を消す
                enginSound.Stop();

                //AIの目的地を設定&少しランダム化
                targetPoint = RaceManager.instance.allCheckPoints[currentTarget].transform.position;
                RandomiseAITarget();


                //UIに経過時間を表示
                //UIManager.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);
                //関数の呼び出し(クリア時)
                RaceManager.instance.FinishRace();

            }
        }
    }
    //AIの目的地に少し差を出す
    public void RandomiseAITarget()
    {
        targetPoint += new Vector3(Random.Range(-aiPointVariance, aiPointVariance), 0f, Random.Range(-aiPointVariance, aiPointVariance));
    }

    //最後のチェックポイントに移動する
    public void ResetToTrack()
    {
        //最後のチェックポイントの数値にする(0から数値が始まるため-1)
        int pointToGoTo = nextCheckPoint - 1;

        if (pointToGoTo < 0)
        {
            //最後のチェックポイントの数値にする(リストなどは0から数値が始まるため-1)
            pointToGoTo = RaceManager.instance.allCheckPoints.Length - 1;
        }

        //現在地とrgの現在位置のポジションを最後のチェックポイントに戻す
        transform.position = RaceManager.instance.allCheckPoints[pointToGoTo].transform.position;
        rg.transform.position = transform.position;
        //この時、加えられている力は0にする
        rg.velocity = Vector3.zero;
        speedInput = 0f;
        turnInput = 0f;


        resetCounter = resetCooldown;

    }

    

    IEnumerator Slip()//バナナスリップ
    {
        for(int i = 0; i < 36; i++)
        {
            //回転するのはボディだけにできそう
            carBody.transform.Rotate(new Vector3(0, 10, 0));

            yield return new WaitForSeconds(0.01f);
        }
        
    }

    IEnumerator SlipKoura()//甲羅スリップ
    {
        rg.velocity = new Vector3(0, 0, 0);


        for (int i = 0; i < 36; i++)
        {
            //carBody.transform.position = transform.position + new Vector3(0.05f, 1.2f, 0);
            carBody.transform.Rotate(new Vector3(10, 0, 0));

            yield return new WaitForSeconds(0.01f);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "itemBox")
        {
            
            //関数呼ぶ
            RandomItem();
        }
    }

    public void RandomItem()//ランダムでアイテム取得
    {
        randomIndex = Random.Range(0, items.Length);
        itemCheck = true;

        if(!isAI)//プレイヤーの時だけ画面に取得したアイテムを表示する
        {
            RaceManager.instance.Image(itemCheck, randomIndex);
        }
        
    }

    

    public void UseItem(int z)//アイテムを使用する
    {
        if (!isAI)//プレイヤーの時だけ処理を行う
        {
            GameObject item;
            itemCheck = false;//アイテム所持判定をoff


            if(items[randomIndex] == items[1])//キノコなら
            {
                Kinoko();
            }
            else if (z == 5)
            {
                itemCheck = false;
                //生成（前に）
                item = Instantiate(items[randomIndex], FierPoint_Front.transform.position + transform.forward, Quaternion.identity) as GameObject;
                //生成したアイテムに力を加える
                item.GetComponent<Rigidbody>().velocity = rg.velocity.normalized * maxSpeed * Addspeed;
            }
            else
            {
                itemCheck = false;
                //生成（後ろに）
                item = Instantiate(items[randomIndex], FierPoint_Buck.transform.position - transform.forward, Quaternion.identity) as GameObject;
                //生成したアイテムに力を加える
                item.GetComponent<Rigidbody>().velocity = -rg.velocity.normalized * maxSpeed * Addspeed;
            }

            //画像を消す
            RaceManager.instance.Image(itemCheck, randomIndex);
        }
        

    }

}
