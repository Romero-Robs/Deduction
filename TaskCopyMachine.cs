using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskCopyMachine : TaskInfo
{
    public override void TaskOperation() {
        base.TaskOperation();

        InputManager.Instance.movementLock = true;
        InputManager.Instance.rotationLock = true;
        StartCoroutine(TaskProgressBarRoutine(barFill.GetComponent<Image>()));
    }
}
