using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuLogic : MonoBehaviour
{
    public Slider PlayerSlider;
    public GameObject singlePlayer;
    public TextMeshProUGUI val;
    private bool _isSinglePlayer = false;
    public AudioSource music;

    private enum Screens
    {
        Starting, Loading, Multiplayer, Options, Info
    }

    private Dictionary<string, GameObject> _object;
    private Screens _currScreen;
    private void Awake() //happens before the game starts
    {
        InitAwake();
    }
    void Start()
    {
        InitStart();
        PlayerVal();

    }


    private void InitStart() //starting screen positions
    {

        _currScreen = Screens.Starting;
        _object["Starting_screen"].SetActive(true);
        _object["Info_screen"].SetActive(false);
        _object["Loading_screen"].SetActive(false);
        _object["Multiplayer_screen"].SetActive(false);
        _object["Options_screen"].SetActive(false);
        singlePlayer.SetActive(false);
    }



    private void InitAwake()
    {
        _object = new Dictionary<string, GameObject>();
        GameObject[] screen = GameObject.FindGameObjectsWithTag("ScreenObj");
        foreach (GameObject obj in screen)
        {
            _object.Add(obj.name, obj);
        }
        Debug.Log(_object.Count);
    }
    
    
    //controll func
    private void ChangeScreen(Screens toScreen) //changing screen func
    {
        _object[_currScreen + "_screen"].SetActive(false);
        _object[toScreen + "_screen"].SetActive(true);

        _currScreen = toScreen;
    }

    
    //button controls
    public void SinglePlayerButton()
    {
        _isSinglePlayer = true;
        LoadingPath();
        ChangeScreen(Screens.Loading);

    }

    public void BackButton()
    {
        ChangeScreen(Screens.Starting);
    }

    public void MultiplayerButton()
    {
        ChangeScreen(Screens.Multiplayer);
    }

    public void OptionsButton()
    {
        ChangeScreen(Screens.Options);
    }

    public void InfoButton()
    {
        ChangeScreen(Screens.Info);
    }

    public void PlayButton()
    {
        ChangeScreen(Screens.Loading);
    }

    //player slider controlls

    public void PlayerVal()
    {
        if (val != null && PlayerSlider)
        {
            int roundedValue = Mathf.RoundToInt(PlayerSlider.value / 5f) * 5; // go up in fives
            val.text = $"{roundedValue}$";
        }
    }


    // loading to game function

    public void LoadingPath()
    {
        if (_isSinglePlayer)
        {
            StartCoroutine(HideLoadingScreen(3f));
        }
        else
        {
            _object["Loading_screen"].SetActive(false);

        }
    }


    // hiding loading screen after delay
    private IEnumerator HideLoadingScreen(float delay)
    {
        yield return new WaitForSeconds(delay);
        _object["Loading_screen"].SetActive(false);
        music.GetComponent<AudioSource>().Stop();
        singlePlayer.SetActive(true);
    }

}
