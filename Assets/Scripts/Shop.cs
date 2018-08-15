using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public enum ShopEnum
    {
        Shop,
        Cafe,
        WC
    }
    public ShopEnum ShopType;
    public int ShopID;
    public Transform Entrance;
    public float TimePerUnit;

}
