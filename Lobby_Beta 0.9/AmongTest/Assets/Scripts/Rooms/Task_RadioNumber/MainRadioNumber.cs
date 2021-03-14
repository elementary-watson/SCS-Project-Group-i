﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class MainRadioNumber : MonoBehaviour
{
    private int fixcount = 3;
    private int count = 0;
    int rn1=0;
    int rn2=0;
    int rn3=0;
    int irn1=0;
    int irn2=0;
    int irn3=0;


    public GameObject winText;
    public Network _network;
    [SerializeField] GameObject RadioPanel;
    public TextMeshProUGUI randomnubertext;
    [SerializeField] Button[] button = new Button[0];
    [SerializeField] private TextMeshProUGUI[] currentvalue = new TextMeshProUGUI[0];
    [SerializeField] private Image[] Thump = new Image[0];

    // Start is called before the first frame update
    void Start()
    {
        Thump[0].enabled = false;
        Thump[1].enabled = false;
        Thump[2].enabled = false;
        generatenumber(); 
    }
                                                                              
    private void generatenumber()
    {
        rn1 = UnityEngine.Random.Range(1, 10)*10;
        rn2 = UnityEngine.Random.Range(1, 10)*5;
        rn3 = UnityEngine.Random.Range(1, 10);

        if (rn1 >= 50)
            irn1 = 0;//UnityEngine.Random.Range(1, 5) * 10;
        else
            irn1 = 100;//UnityEngine.Random.Range(5, 10) * 10;

        if (rn2 >= 25)
            irn2 = 0;//UnityEngine.Random.Range(1, 5) * 5;
        else
            irn2 = 50;//UnityEngine.Random.Range(5, 10) * 5;

        if (rn3 >= 5)
            irn3 = 0;//UnityEngine.Random.Range(1, 5);
        else
            irn3 = 10;//UnityEngine.Random.Range(5, 10);

        Invoke("setText",1); 

    }

    public void setText()
    {
        randomnubertext.text = "\t" + rn1 + ".\t" + rn2 + ".\t" + rn3 + "\t";
        currentvalue[0].text = irn1+"";
        currentvalue[1].text = irn2 + "";
        currentvalue[2].text = irn3 + "";
    }

    public void setradionumber(int aim, int collum)
    {
        if (collum == 0)
        {
            if (aim == -10 && irn1 == 0) { }
            else if (aim == 10 && irn1 == 100) { }
            else
                    {
                        irn1 += aim;
                        currentvalue[0].text = "" + irn1;
                    }

            if (irn1 == rn1)
            {
                button[0].interactable = false;
                button[1].interactable = false;
                Invoke("ShowThump", 1);

                checkcounter(1);
            }
        }
        print(aim + collum);
        if (collum == 1)
        {
            print(aim);
            print("Collum1");
            if (aim < 0 && irn2 == 0) { }
            else if (aim == 5 && irn2 == 50) { }
            else
            {
                print("Collum1add");
                irn2 += aim;
                currentvalue[1].text = "" + irn2;
            }

            if (irn2 == rn2)
            {
                button[2].interactable = false;
                button[3].interactable = false;
                Invoke("ShowThump", 1);

                checkcounter(1);

            }
        }

        if (collum == 2)
        {
            print("Collum2");

            if (aim < 0 && irn3 == 0) { }
            else if (aim == 1 && irn3 == 10) { }
            else
            {
                print("Collum1add");

                irn3 += aim;
                currentvalue[2].text = "" + irn3;
            }

            if (irn3 == rn3)
            {
                button[4].interactable = false;
                button[5].interactable = false;
                Invoke("ShowThump", 1);
                checkcounter(1);                
            }
        }
    }

    public void ShowThump()
    {
        if (irn1 == rn1)
        {
            Thump[0].enabled = true;
            currentvalue[0].text = "";
        }
       
        if (irn2 == rn2)
        {
            Thump[1].enabled = true;
            currentvalue[1].text = "";
        }
        
        if (irn3 == rn3)
        {
            Thump[2].enabled = true;
            currentvalue[2].text = "";
        }
        

    }

    public void checkcounter(int cp)
    {
        count = count + cp;
        if (count == fixcount)
        {
            winText.SetActive(true);
            Invoke("taskfinished", 2);
            _network.incrementTaskprogress(10);
        }
    }


    private void taskfinished()
    {
        RadioPanel.SetActive(false);
    }
}