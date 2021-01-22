using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinTrigger : MonoBehaviour
{

    public int bin;
    public TaskMailSort taskContainer;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Mail") && other.gameObject.GetComponent<MailInfo>().num == bin) {
            other.gameObject.tag = "Untagged";
            other.gameObject.GetComponent<MailInfo>().inCorrectBin = true;
            taskContainer.MailScored();
        }
    }
}

