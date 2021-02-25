using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obake : MonoBehaviour
{
    float x;

    // Start is called before the first frame update
    void Start()
    {
        x = 1.7f;

        UIManager.instance.countDownText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(x > 0)
        {
            x -= Time.deltaTime;  
        }

        if (x <= 0)
        {
            Debug.Log("aaa");
            Possession();
        }
    }

    void Possession()
    {
        //回って小さくして車に入れる
        this.transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime;
        this.transform.Rotate(new Vector3(0, 90, 0) * Time.deltaTime * 10);

        if(this.transform.localScale.x <= 0)
        {
            Check();
        }
        
    }

    //カウントダウンが始まる
    void Check()
    {
        //お化け非表示
        this.gameObject.SetActive(false);
        //bool判定変える
        RaceManager.instance.Appearance = false;

        //UI表示
        UIManager.instance.countDownText.gameObject.SetActive(true);
    }
}
