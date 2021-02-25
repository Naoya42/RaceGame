using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Car")//車と衝突したとき
        {

            this.gameObject.SetActive(false);//非表示にする
            
            Invoke("Appearance", 5f);//5秒後に関数呼ぶ
        }
    }

    public void Appearance()//表示させる
    {
        gameObject.SetActive(true);
    }
}
