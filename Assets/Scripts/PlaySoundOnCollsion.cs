using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnCollsion : MonoBehaviour
{

    public AudioSource soundToPlay;

    public int groundLayerNo = 8;

    //障害物にぶつかった時の音
    private void OnCollisionEnter(Collision other)
    {
        //地面はならないように(これがないと動き始めで音がしてします)
        if(other.gameObject.layer != groundLayerNo)
        {
            //設定されている音源をストップ
            soundToPlay.Stop();

            //音の高さにランダム性
            soundToPlay.pitch = Random.Range(0.8f, 1.2f);
            //音の大きさを設定
            soundToPlay.volume = 0.3f;
            //設定されている音源を再生
            soundToPlay.Play();
        }
    }
}
