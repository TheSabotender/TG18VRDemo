using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetRegistry : MonoBehaviour
{
    public static AssetRegistry instance;

    public Sprite[] logos;
    public Font[] fonts;

    void Start()
    {
        instance = this;
    }
}
