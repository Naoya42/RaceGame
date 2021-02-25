using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static MainCamera instance;


    public Cinemachine.CinemachineVirtualCamera cineCam;
    //public   ;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetTarget(CarController PlayerCar)
    {
        cineCam.m_Follow = PlayerCar.transform;
        cineCam.m_LookAt = PlayerCar.transform;
    }

    public void ResetTarget()
    {
        //cineCam.transform.position = PlayerCar.transform.position;

        //cineCam.m_Follow = null;
        cineCam.m_LookAt = null;
    }
}
