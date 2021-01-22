using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class TaskPhoneCall : TaskInfo
{
    //position for player to be to start the task
    public GameObject spawnSort;
    public BoxCollider collider;

    public List<ButtonInfo> btnList = new List<ButtonInfo>();
    public List<GameObject> currentOrder = new List<GameObject>();
    List<int> numberOrder = new List<int>();

    public int extension;

    public TextMeshProUGUI machineText;
    public TextMeshProUGUI noteText;

    private string currInput = "";


    public override void TaskOperation() {
        base.TaskOperation();

        //MatchManager.Instance.myPlayer.GetComponent<PlayerInfo>().MovePlayer(spawnSort.transform.position, spawnSort.transform.eulerAngles.y + 90);
        InputManager.Instance.movementLock = true;
        GameplayUIManager.Instance.pressToCancel.SetActive(true);

        for(int i = 0; i < btnList.Count; i++) {
            btnList[i].active = true;
        }
        collider.enabled = false;
        //StartCoroutine(TaskProgressBarRoutine(barFill.GetComponent<Image>()));
        //InputManager.Instance.defaultControls.Default.Attack.performed += PressButton;

        RandomizeNumber();
    }

    public void RandomizeNumber() {
        numberOrder.Add(99);
        for (int i = 0; i < extension; i++) {
            numberOrder.Add(Random.Range(0, 10));
        }

        noteText.text = "#";
        for (int j = 1; j < numberOrder.Count; j++) {
            noteText.text += numberOrder[j].ToString();
        }
    }

    public override void CancelTask() {
        base.CancelTask();
        //InputManager.Instance.defaultControls.Default.Attack.performed -= PressButton;
        InputManager.Instance.SetPlayerLock(false);
        for (int i = 0; i < btnList.Count; i++) {
            btnList[i].active = false;
        }
        collider.enabled = true;

        ResetOrder();
        StartCoroutine(MachineText(taskComplete));
        noteText.text = "";
    }

    public void AddNumber(GameObject phoneNum) {
        currentOrder.Add(phoneNum);
        if (phoneNum.GetComponent<ButtonInfo>().btnID == 99) {
            currInput += "#";
        }
        else if(phoneNum.GetComponent<ButtonInfo>().btnID == 98) {
            currInput += "*";
        }
        else {
            currInput += phoneNum.GetComponent<ButtonInfo>().btnID;
        }
        machineText.text = currInput;
        machineText.color = Color.white;
        if (currentOrder.Count == numberOrder.Count) {
            CheckNumbers();
        }
    }

    void CheckNumbers() {
        bool isEqual = true;
        for (int i = 0; i < extension + 1; i++) {
            if (currentOrder[i].GetComponent<ButtonInfo>().btnID != numberOrder[i]) {
                isEqual = false;
                RestartTask();
                break;
            }
        }
        if (isEqual) {
            CompleteTask();
        }
    }

    void RestartTask() {
        ResetOrder();
        RandomizeNumber();
        StartCoroutine(MachineText(false));
    }

    IEnumerator MachineText(bool correct) {
        currInput = "";

        if (correct) {
            machineText.color = Color.green;
        }
        else {
            machineText.color = Color.red;
        }
        yield return new WaitForSeconds(1.5f);
        machineText.color = Color.white;
        machineText.text = currInput;
    }

    void ResetOrder() {
        currentOrder.Clear();
        numberOrder.Clear();
    }

    public override void CompleteTask() {
        base.CompleteTask();
    }
}
