using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientInfo : ObjectInfo
{
    //public
    //list of the names for the different ingredients
    public List<string> iNames = new List<string>();
    public List<Material> matList = new List<Material>();
    //ID for each ingredient to dictate what material and name to have
    public int ID;
    public SphereCollider collider;
    public Transform lastPosition;

    //private


    public void ChangeColor() {
        //when function called it changes the color to match the index in each list
        gameObject.GetComponentInChildren<MeshRenderer>().material = matList[ID];
    }

    public void ChangeName() {
        gameObject.name = iNames[ID];
    }

    public void DestroyObject() {
        Destroy(gameObject);
    }
}
