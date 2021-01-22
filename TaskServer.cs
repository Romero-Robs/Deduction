using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TaskServer : TaskInfo {
    //need to have a list of spots/gameobjects of servers for the player to work on each server
    public List<GameObject> servers = new List<GameObject>();
    public List<GameObject> emptyServers = new List<GameObject>();
    List<GameObject> harddrives = new List<GameObject>();

    public GameObject hddHolder;
    public GameObject harddrive;

    public bool inPosition;
    GameObject hd;
    //public List<int> randServer = new List<int>();
    public int serverToFix = 4;
    public int currentDrive = 0;
    //this task needs to start when you look and click E on the HDDs that are on the desk in the server room
    public override void TaskOperation() {
        base.TaskOperation();
        currentDrive = 0;
        //allow the player to cancel
        GameplayUIManager.Instance.pressToCancel.SetActive(true);
        List<GameObject> tempServerList = new List<GameObject>(servers);

        //pick up a HDD that is on the table, walk in front of a server that has a red light
        //check if the player hits E in front of the red server lock the player in front of the location they hit E on


        //list of random numbers to pick which servers to do task on
        //randomize 4 different servers to do the task
        for (int i = 0; i < serverToFix; i++) {
            hd = Instantiate(harddrive, hddHolder.transform);
            hd.transform.position = new Vector3(hddHolder.transform.position.x, hddHolder.transform.position.y, hddHolder.transform.position.z - (.5f * i));
            harddrives.Add(hd);
            GameObject newServer = tempServerList[Random.Range(0, tempServerList.Count)];
            tempServerList.Remove(newServer);
            newServer.GetComponent<ServerInfo>().selected = true;
            emptyServers.Add(newServer);
            newServer.GetComponent<ServerInfo>().ChosenServer();

        }

        //show all the randomly chosen servers by changing the slot to show
        PickUpHarddrive();
    }

    public override void CancelTask() {
        base.CancelTask();
        inPosition = false;
        InputManager.Instance.SetPlayerLock(false);

        for (int i = 0; i < serverToFix; i++) {
            emptyServers[i].GetComponent<ServerInfo>().ResetServers();
            Destroy(harddrives[i]);
        }


        emptyServers.Clear();
        harddrives.Clear();
        ExitTask();
    }

    public void PickUpHarddrive() {
        harddrives[currentDrive].transform.SetParent(MatchManager.Instance.myPlayer.GetComponent<PlayerInfo>().holder.transform, false);
        harddrives[currentDrive].transform.localPosition = Vector3.zero;
    }

    public void LockPlayer(Transform sTransform) {
        //lock the player in the position in front of the server
        inPosition = true;
        InputManager.Instance.SetPlayerLock(true);
        StartCoroutine(LockPosition(sTransform));
        harddrives[currentDrive].GetComponent<HDDInfo>().inUse = true;
        InputManager.Instance.defaultControls.Default.Attack.performed += InsertHDD;
    }

    private IEnumerator LockPosition(Transform sTransform) {
        yield return new WaitForSeconds(0.1f);
        MatchManager.Instance.myPlayer.GetComponent<PlayerInfo>().MovePlayer(sTransform.transform.position, sTransform.transform.eulerAngles.y + 90);

    }

    private void InsertHDD(InputAction.CallbackContext context) {
        InputManager.Instance.defaultControls.Default.Attack.performed -= InsertHDD;
        //when they click after moving the HDD around just move the HDD in z axis forward and stop until it either fits in the slot or hits the wall and breaks
        harddrives[currentDrive].GetComponent<HDDInfo>().moveHDD = true;
    }

    private void Update() {
        if (!taskComplete && harddrives.Count > 0 && harddrives[currentDrive].GetComponent<HDDInfo>().broke) {
            //reset and let the user move the hdd forward again
            harddrives[currentDrive].GetComponent<HDDInfo>().broke = false;//reset the hdd
            InputManager.Instance.defaultControls.Default.Attack.performed += InsertHDD;
        }

        if (!taskComplete && harddrives.Count > 0 && harddrives[currentDrive].GetComponent<HDDInfo>().inSlot) {
            harddrives[currentDrive].transform.parent = null;
            inPosition = false;
            InputManager.Instance.SetPlayerLock(false);
            //move on to the next hdd to be used
            //harddrives.RemoveAt(currentDrive);
            currentDrive++;
            //start the task all over again with the new drive
            if (currentDrive == serverToFix) {
                CompleteTask();
            }
            else {
                PickUpHarddrive();
            }
        }
    }

    public override void CompleteTask() {
        base.CompleteTask();
        InputManager.Instance.SetPlayerLock(false);
    }

    private void ExitTask() {
        InputManager.Instance.defaultControls.Default.Attack.performed -= InsertHDD;
    }
}
