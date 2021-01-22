using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (transform.parent.GetComponent<ServerInfo>().selected && other.CompareTag("Harddrive") && other.gameObject.GetComponent<HDDInfo>().moveHDD) {
            //let it move for like a second then tell the object to hdd to stop moving and keep it inside the slot
            StartCoroutine("StickHDDInSlot", other.gameObject);
        }
    }

    IEnumerator StickHDDInSlot(GameObject hdd) {
        yield return new WaitForSeconds(0.125f);
        hdd.GetComponent<HDDInfo>().InSlot(transform.position);
        transform.parent.GetComponent<ServerInfo>().FixedServer();
        //hdd.GetComponent<HDDInfo>().task.SlotScored();
    }
}
