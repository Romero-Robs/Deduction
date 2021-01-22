using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskSandwichBuilder : TaskInfo
{
    //list to hold all the locations for the parts of the sandwich
    public List<GameObject> ingredientsLocations = new List<GameObject>();
    public GameObject ingredient;
    public List<GameObject> currentIngredients = new List<GameObject>();

    List<GameObject> recipeOrder = new List<GameObject>();
    List<GameObject> placedOrder = new List<GameObject>();

    public bool currentHolding;
    int holdingID;
    public GameObject plate;
    public TextMeshProUGUI recipe;
    GameObject p;

    public GameObject plateSpawn;

    public override void TaskOperation() {
        base.TaskOperation();
        //allow the player to cancel
        GameplayUIManager.Instance.pressToCancel.SetActive(true);

        RandomizeLocation();
        RandomizeRecipe();

        p = Instantiate(plate, plateSpawn.transform);
    }

    public void RandomizeLocation() {
        Debug.Log("Randomizing Location");
        List<GameObject> tempLocations = new List<GameObject>(ingredientsLocations);

        for (int i = 0; i < ingredientsLocations.Count; i++) {
            GameObject location = tempLocations[Random.Range(0, tempLocations.Count)];
            tempLocations.Remove(location);
            GameObject tIngredients = Instantiate(ingredient, location.transform);
            //change each value of the ingredients spawned
            tIngredients.GetComponent<IngredientInfo>().ID = i;
            tIngredients.GetComponent<IngredientInfo>().ChangeColor();
            tIngredients.GetComponent<IngredientInfo>().ChangeName();
            tIngredients.GetComponent<IngredientInfo>().lastPosition = location.transform;

            currentIngredients.Add(tIngredients);
        }
    }

    public void RandomizeRecipe() {
        Debug.Log("Randomizing Recipe");
        List<GameObject> tempRecipe = new List<GameObject>(currentIngredients);

        int breadCount = 0;
        GameObject bStart = null;
        GameObject bEnd = null;
        recipe.text = "Recipe: \n";
        for (int i = 0; i < currentIngredients.Count; i++) {
            GameObject tRecipe = tempRecipe[Random.Range(0, tempRecipe.Count)];
            if (tRecipe.name != "Bread") {
                tempRecipe.Remove(tRecipe);
                recipeOrder.Add(tRecipe);
            }
            else {
                breadCount++;
                if (breadCount == 1) {
                    bStart = tRecipe;
                }
                if(breadCount == 2) {
                    bEnd = tRecipe;
                }
                tempRecipe.Remove(tRecipe);
            }
        }

        recipeOrder.Insert(0, bStart);
        recipeOrder.Insert(recipeOrder.Count, bEnd);

        for(int k = 0; k < recipeOrder.Count; k++) {
            recipe.text += recipeOrder[k].name + "\n";
            Debug.Log("recipe" + recipeOrder[k].name);
        }
    }

    public override void CancelTask() {
        base.CancelTask();

        Destroy(p);
        for(int i = 0; i < currentIngredients.Count; i++) {
            //currentIngredients[i].GetComponent<IngredientInfo>().pickedUp = false;
            Destroy(currentIngredients[i]);
        }

        ResetOrder();
        recipe.text = "";
    }

    public void PickUpIngredient(IngredientInfo ingredient) {
        ingredient.collider.enabled = false;
        if (currentHolding && currentIngredients[holdingID].GetComponent<IngredientInfo>().pickedUp) {
            currentIngredients[holdingID].GetComponent<IngredientInfo>().pickedUp = false;
            currentIngredients[holdingID].transform.SetParent(currentIngredients[holdingID].GetComponent<IngredientInfo>().lastPosition, false);
            currentIngredients[holdingID].GetComponent<IngredientInfo>().collider.enabled = true;
        }

        currentHolding = true;
        holdingID = ingredient.ID;
        ingredient.transform.SetParent(MatchManager.Instance.myPlayer.GetComponent<PlayerInfo>().holder.transform, false);
        ingredient.transform.localPosition = Vector3.zero;
        ingredient.pickedUp = true;
    }

    public void PlaceOnPlate() {
        //called when placing the ingredients on the plate object (need reference to the plate)
        if (currentHolding) {
            Debug.Log("Adding the ingredient: " + currentIngredients[holdingID].name);
            currentIngredients[holdingID].transform.SetParent(p.transform, false);
            currentIngredients[holdingID].GetComponent<IngredientInfo>().pickedUp = false;
            currentIngredients[holdingID].GetComponent<IngredientInfo>().collider.enabled = false;
            currentHolding = false;
            placedOrder.Add(currentIngredients[holdingID]);
            if (placedOrder.Count == recipeOrder.Count) {
                Debug.Log("Checking the order");
                //check if they have the same order of ingredients similar to phone task
                CheckOrder();
            }
        }
    }

    void CheckOrder() {
        bool isEqual = true;
        for(int i = 0; i < recipeOrder.Count; i++) {
            if (recipeOrder[i].name != placedOrder[i].name) {
                Debug.Log("failed restarting task");
                StartCoroutine("DisplayMistake");

                isEqual = false;
                break;
            }
        }
        if (isEqual) {
            Debug.Log("Task Completed");
            //Task is completed and good
            CompleteTask();
        }
    }

    IEnumerator DisplayMistake() {
        recipe.text = "Incorrect Order...\n Restarting";

        yield return new WaitForSeconds(2f);
        RestartTask();
    }

    void RestartTask() {
        ResetOrder();
        RandomizeLocation();
        RandomizeRecipe();
    }

    void ResetOrder() {
        recipe.text = "";
        currentIngredients.Clear();
        recipeOrder.Clear();
        placedOrder.Clear();
        for(int i = 1; i < p.transform.childCount; i++) {
            p.transform.GetChild(i).GetComponent<IngredientInfo>().DestroyObject();
        }
    }
}
