using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedFBManager : MonoBehaviour
{
    //Dont Destroy
    public static SimulatedFBManager instance;

    private void Awake()
    {
        if (instance != null)
        {Destroy(gameObject);}
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        StartCoroutine(SimulatedLogin());
    }

    IEnumerator SimulatedLogin()
    {
        Debug.Log("SimulatedFBManager: logging in...");
        yield return new WaitForSeconds(1f);
        Debug.Log("SimulatedFBManager: logged in!");
        
        //Succsesfully logged in
        //_screenManager.Login();
        
        //user is not logged in
        ScreenManager.instance.WelcomeScreen();
    }

    public void Register()
    {
        StartCoroutine(SimulatedRegister());
    }
    IEnumerator SimulatedRegister()
    {
        Debug.Log("SimulatedFBManager: simulating register in...");
        yield return new WaitForSeconds(2f);
        Debug.Log("SimulatedFBManager: registered");
        //Succsesfully logged in
        //ScreenManager.instance.Login();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
