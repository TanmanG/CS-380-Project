using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour{
    //public AudioMixer audioMixer;
    public void SetVolume(float volume){
        Debug.Log(volume);
        //To do: Get audio and use the following commands
        //audioMixer.SetFloat("Volume",volume);
    }

    public void SetQuality(int qualityIndex){
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    
    public void BacktoMenu(){
        SceneManager.LoadSceneAsync("Pause Menu");
        
    }
}
