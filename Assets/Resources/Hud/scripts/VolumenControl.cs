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

	private const float step = 0.09f;

	public AudioMixer audioMixer;

	// Convierte de volumen 0 - 1 a decibelios para cambiar el volumen
    public void SetVolume(float volume)
    {
        float dB = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("Volume", dB);
    }

	public void onClick() {
		if(subir && VolumeManager.volumen < 1) {
			VolumeManager.volumen += step;
		} else if(!subir && VolumeManager.volumen > 0) {
			VolumeManager.volumen -= step;
		}
		text.text = (int)(VolumeManager.volumen * 100) + "";
		Debug.Log("Volumen " + VolumeManager.volumen);
		SetVolume(VolumeManager.volumen);
	}

}
