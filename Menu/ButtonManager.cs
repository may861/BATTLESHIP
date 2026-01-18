using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{

    public MenuLogic menuLogic;


    public void SinglePlayerBtn()
    {
        if (menuLogic != null)
        {
            menuLogic.SinglePlayerButton();
        }
    }

    public void PlayBtn()
    {
        if (menuLogic != null)
        {
            menuLogic.SinglePlayerButton();
        }
    }

    public void BackBtn()
    {
        if (menuLogic != null)
        {
            menuLogic.BackButton();
        }
    }

    public void MultilayerBtn()
    {
        if (menuLogic != null)
        {
            menuLogic.MultiplayerButton();
        }
    }

    public void OptionsBtn()
    {
        if (menuLogic != null)
        {
            menuLogic.OptionsButton();  
        }
    }

    public void InfoBtn()
    {
        if (menuLogic != null)
        {
            menuLogic.InfoButton();
        }
    }
}
