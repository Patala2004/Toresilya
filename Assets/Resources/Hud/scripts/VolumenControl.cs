using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumenControl : MonoBehaviour
{
	public TextMeshProUGUI text;
	public bool subir;

	private const int step = 10;

	public AudioMixer audioMixer;

	// Convierte de volumen 0 - 1 a decibelios para cambiar el volumen
    public void SetVolume(float volume)
    {
		float dB;
		if(volume != 0) {
        	dB = Mathf.Log10(volume) * 20;
		} else {
			dB = -80;
		}
        audioMixer.SetFloat("Volume", dB);
    }

	public void Start() {
		text.text = VolumeManager.volumen + "";
	}

	public void onClick() {
		if(subir && VolumeManager.volumen < 100) {
			VolumeManager.volumen += step;
		} else if(!subir && VolumeManager.volumen > 0) {
			VolumeManager.volumen -= step;
		}
		text.text = VolumeManager.volumen + "";
		SetVolume((float)VolumeManager.volumen / 100);
	}

}
