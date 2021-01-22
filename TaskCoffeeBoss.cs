using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskCoffeeBoss : TaskInfo
{
    //references to objects to either spawn or pick up and interact with
    public GameObject coffeeCupPrefab;
    public GameObject coffeeCupSpawn;
    public GameObject plateSpawn;
    public GameObject platePrefab;
    public bool pickedUpCup;
    public bool onPlate;
    GameObject cup;
    GameObject plate;

    public override void TaskOperation() {
        base.TaskOperation();
        //starting the coffee task
        Debug.Log("Starting coffee task");
        GameplayUIManager.Instance.pressToCancel.SetActive(true);

        StartCoroutine("SpawnObjects");
        SpawnObjects();
    }

    public override void CancelTask() {
        base.CancelTask();
        pickedUpCup = false;
        StopAllCoroutines();
        if (!taskComplete) {
            Destroy(cup);
            Destroy(plate);
        }
    }

    IEnumerator SpawnObjects() {
        plate = Instantiate(platePrefab, plateSpawn.transform);
        yield return new WaitForSeconds(3f);
        //create the coffee and use reference to not destroy or move prefab
        cup = Instantiate(coffeeCupPrefab, coffeeCupSpawn.transform);
    }

    public void PickUpCoffee() {
        //picking up the reference object of the coffee cup
        if (!pickedUpCup) {
            pickedUpCup = true;
            cup.transform.SetParent(MatchManager.Instance.myPlayer.GetComponent<PlayerInfo>().holder.transform, false);
            cup.transform.localPosition = Vector3.zero;
        }
    }

    public void PlaceCoffee() {
        if (pickedUpCup && !onPlate) {
            pickedUpCup = false;
            onPlate = true;
            cup.transform.SetParent(plate.transform, false);

            CompleteTask();
        }
    }

}
