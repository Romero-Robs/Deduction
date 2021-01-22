using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TaskMailSort : TaskInfo
{
    public GameObject spawnSort;
    public List<GameObject> mailPrefabs = new List<GameObject>();
    public List<string> mailPrefabStrings = new List<string>();
    public GameObject mailSpawn;

    GameObject mailObj;

    public int mailCount = 0;

    List<GameObject> allMailObj = new List<GameObject>();
    bool throwCooldown;
    float fill = 0f;

    PhotonView PV;

    private void Awake() {
        PV = GetComponent<PhotonView>();
    }

    public override void TaskOperation() {
        base.TaskOperation();

        MatchManager.Instance.myPlayer.GetComponent<PlayerInfo>().MovePlayer(spawnSort.transform.position, spawnSort.transform.rotation.y);
        InputManager.Instance.movementLock = true;
        PV.RPC("ChangeOccupied", RpcTarget.All, true);
        GameplayUIManager.Instance.pressToCancel.SetActive(true);

        StartCoroutine(MailSortLoop());
    }

    private IEnumerator MailSortLoop() {
        SpawnMailObj();

        while (TaskManager.Instance.taskInProgress) {
            if (MeetingManager.Instance.meetingActive || !TaskManager.Instance.taskInProgress) {
                CancelTask();
            }
            yield return new WaitForSeconds(.2f);
        }

        //InputManager.Instance.defaultControls.Default.Attack.performed -= ThrowMail;
        yield return null;
    }

    private void ThrowMail(InputAction.CallbackContext context) {
        InputManager.Instance.defaultControls.Default.Attack.performed -= ThrowMail;
        mailObj.transform.parent = gameObject.transform;
        mailObj.transform.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
        mailObj.transform.GetChild(0).GetComponent<MailInfo>().MailThrown();
        Vector3 vforce = 200 * mailObj.transform.forward;
        vforce += 100 * mailObj.transform.up;
        vforce += Random.Range(-20, 20) * mailObj.transform.right;
        mailObj.transform.GetChild(0).GetComponent<Rigidbody>().AddForce(vforce);

        if (!throwCooldown && !taskComplete) {
            StartCoroutine(SpawnMail());
        }
    }

    IEnumerator SpawnMail() {
        throwCooldown = true;
        yield return new WaitForSeconds(1f);
        throwCooldown = false;
        if (taskInProgress) {
            SpawnMailObj();
        }
    }

    private void SpawnMailObj() {
        mailObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", mailPrefabStrings[Random.Range(0, mailPrefabs.Count)]), mailSpawn.transform.position, Quaternion.identity);
        mailObj.transform.rotation = Quaternion.identity;
        allMailObj.Add(mailObj);
    }

    public void PickUpMail() {
        InputManager.Instance.defaultControls.Default.Attack.performed += ThrowMail;
        mailObj.transform.SetParent(MatchManager.Instance.myPlayer.GetComponent<PlayerInfo>().holder.transform, false);
        mailObj.transform.localPosition = Vector3.zero;
        mailObj.transform.localRotation = Quaternion.identity;
        mailObj.transform.GetChild(0).Rotate(mailObj.transform.GetChild(0).GetComponent<MailInfo>().pickRotation);
        mailObj.transform.GetChild(0).GetComponent<MailInfo>().pickedUp = true;
    }

    public void MailScored() {
        //code for networked players (not the one doing task)
        if(MatchManager.Instance.myPlayer.GetComponent<PlayerInfo>().currentTask != this) {
            return;
        }

        mailCount++;
        if (mailCount < taskLength) {
            fill = mailCount / taskLength;
            barFill.GetComponent<Image>().fillAmount = fill;
        }
        else {
            taskComplete = true;
            InputManager.Instance.movementLock = false;
            InputManager.Instance.rotationLock = false;
            barFill.GetComponent<Image>().fillAmount = 1f;
            barText.SetActive(true);
            TaskManager.Instance.TaskComplete(taskID);
            GameplayUIManager.Instance.pressToCancel.SetActive(false);
            CancelTask();
        }
    }

    public override void CancelTask() {
        base.CancelTask();
        ExitTask();
    }

    private void ExitTask() {
        InputManager.Instance.defaultControls.Default.Attack.performed -= ThrowMail;
        if (mailObj.transform.GetChild(0).GetComponent<MailInfo>().thrown == false) {
            mailObj.transform.GetChild(0).GetComponent<MailInfo>().DestroyMail();
        }

        if (taskComplete) {
            PV.RPC("ChangeOccupied", RpcTarget.All, false);
            if (allMailObj.Count > 0) {
                for (int i = 0; i < allMailObj.Count; i++) {
                    if (allMailObj[i] != null) {
                        allMailObj[i].transform.GetChild(0).GetComponent<MailInfo>().DestroyMail();
                    }
                }
            }
            allMailObj.Clear();
        }
    }

    [PunRPC]
    private void ChangeOccupied(bool occupied) {
        if (MatchManager.Instance.myPlayer.GetComponent<PlayerInfo>().currentTask != this) {
            taskOccupied = occupied;
        }
    }
}
