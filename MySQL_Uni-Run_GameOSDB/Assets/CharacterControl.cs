using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public GameObject M4A1;
    public GameObject WA2000;
    public GameObject G41;

    GameObject gameManagerObject;
    string gunActive;

    void Start()
    {
        gameManagerObject = GameObject.Find("GameManager");
        gunActive = gameManagerObject.GetComponent<GameManager>().thisGunName;

        if (gunActive == "M4A1")
            M4A1.SetActive(true);
        if (gunActive == "WA2000")
            WA2000.SetActive(true);
        if (gunActive == "G41")
            G41.SetActive(true);
    }
}
