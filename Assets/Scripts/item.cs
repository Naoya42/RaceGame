using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
    //アイテムの判定に使用する
    public bool banana;
    public bool koura;
    public bool kinoko;

    public Rigidbody rg;
    // Start is called before the first frame update
    void Start()
    {
        if (koura)
        {
            //rg.velocity = rg.velocity * 10f;//前に進む
        }
        else if (banana)
        {
            //rg.velocity = new Vector3(0f, 7f, 10f);
            //rg.AddForce(new Vector3(0, 10f, 10f));//前に進む
        }
        else if (kinoko)
        {
            rg.velocity = new Vector3(0f, 0f, 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car" && banana)
        {
            
            other.gameObject.GetComponent<CarController>().Banana();
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Car" && koura)
        {
            other.gameObject.GetComponent<CarController>().Koura();
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Car" && kinoko)
        {
            other.gameObject.GetComponent<CarController>().Kinoko();
            Destroy(gameObject);
        }
    }



}
