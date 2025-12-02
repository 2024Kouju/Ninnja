using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Useninjutsu : MonoBehaviour
{
    

    public void Doppel()
    {
        PlayerController.Doppel = true;
        PlayerController.Denkousekka = false ;
        PlayerController.Cherry = false ;
        PlayerController.Slash = false;
        PlayerController.Dragon = false;
    }
    public void Denkousekka()
    {
        PlayerController.Doppel = false;
        PlayerController.Denkousekka = true;
        PlayerController.Cherry = false;
        PlayerController.Slash = false;
        PlayerController.Dragon = false;
    }
    public void Cherry()
    {
        PlayerController.Doppel = false;
        PlayerController.Denkousekka = false;
        PlayerController.Cherry = true;
        PlayerController.Slash = false;
        PlayerController.Dragon = false;
    }
    public void Slash()
    {
        PlayerController.Doppel = false;
        PlayerController.Denkousekka = false;
        PlayerController.Cherry = false;
        PlayerController.Slash = true;
        PlayerController.Dragon = false;
    }
    public void Dragon()
    {
        PlayerController.Doppel = false;
        PlayerController.Denkousekka = false;
        PlayerController.Cherry = false;
        PlayerController.Slash = false;
        PlayerController.Dragon = true;
    }
}
