using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    [SerializeField]
    Slider volumeSlider;
    [SerializeField]
    float musicVolume;
    // Start is called before the first frame update
    void Start()
    {
        if(!PlayerPrefs.HasKey("musicVolume")){
            musicVolume = 1;
        }else{
            musicVolume = PlayerPrefs.GetFloat("musicVolume");
        }
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        Load();
    } 

    public void ChangeMusicVolume(){
        musicVolume = volumeSlider.value;
        AudioListener.volume = musicVolume;
        Save("musicVolume");
    }

    void Load(){
        volumeSlider.value = musicVolume;
    }

    void Save(string audioTypeKey){
        PlayerPrefs.SetFloat(audioTypeKey, volumeSlider.value);
    }
}
