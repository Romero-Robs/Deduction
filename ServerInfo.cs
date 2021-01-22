using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerInfo : MonoBehaviour
{
    public GameObject slot;
    public GameObject spawnPlayer;

    public GameObject redLight;
    public GameObject whiteLight;

    public bool selected;
    public bool inUse;

    public void ChosenServer() {
        slot.SetActive(false);
        SetLight(false);
    }

    public void ResetServers() {
        slot.SetActive(true);
        SetLight(true);
        selected = false;
    }

    public void FixedServer() {
        AudioManager.Instance.SpawnWorldSFX(AudioManager.Instance.hardDriveStartup, transform.position);
        SetLight(true);
        selected = false;
    }

    private void SetLight(bool isWhite) {
        redLight.SetActive(!isWhite);
        whiteLight.SetActive(isWhite);
    }
}
