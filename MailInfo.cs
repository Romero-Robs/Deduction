using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailInfo : MonoBehaviour {

    public int num;
    public bool inCorrectBin;
    public bool pickedUp;
    public bool thrown;
    public Vector3 pickRotation;

    private int despawnTime = 5;

    public void MailThrown() {
        thrown = true;
        StartCoroutine(DespawnTimer());
    }

    private IEnumerator DespawnTimer() {
        yield return new WaitForSeconds(despawnTime);
        if(!inCorrectBin && transform.GetComponent<PhotonView>().IsMine) {
            DestroyMail();
        }
    }

    public void DestroyMail() {
        PhotonNetwork.Destroy(transform.parent.gameObject);
    }
}
