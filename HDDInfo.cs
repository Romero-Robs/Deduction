using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDDInfo : MonoBehaviour
{
    //need to update the movement of the HDD to move with mouse and check if player is currently locked
    public bool inUse;
    public bool moveHDD;
    public float speed = 1f;
    public bool broke;
    public bool inSlot;

    public TaskServer task;

    private void Update() {
        if (inUse) {
            //move the HDD based on the movement of the mouse
            Vector3 temp = Input.mousePosition;
            temp.z = 1.2f;
            transform.position = Camera.main.ScreenToWorldPoint(temp);
        }

        if (moveHDD) {
            //move the hdd forward until it hits the server wall or slot
            inUse = false;
            transform.localPosition += Vector3.forward * Time.deltaTime * speed;
        }
        if (inSlot) {
            inUse = false;
            moveHDD = false;
        }
    }

    public void ResetHDD() {
        inUse = true;
        moveHDD = false;
    }

    public void InSlot(Vector3 pos) {
        inSlot = true;
        //transform.position = pos;
    }
}
