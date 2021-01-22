using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDDCollider : MonoBehaviour
{

    private void OnTriggerEnter(Collider other) {
        if (transform.parent.GetComponent<ServerInfo>().selected && other.CompareTag("Harddrive") && other.gameObject.GetComponent<HDDInfo>().moveHDD) {
            //then break the harddrive and try again
            AudioManager.Instance.SpawnWorldSFX(AudioManager.Instance.hardDriveBreak, transform.position);
            other.GetComponent<HDDInfo>().ResetHDD();
            other.GetComponent<HDDInfo>().broke = true;
            //other.GetComponent<HDDInfo>().task.BrokeDrive();
        }
    }
}
