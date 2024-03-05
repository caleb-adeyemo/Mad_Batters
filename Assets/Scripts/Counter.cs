using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] private KitchenObjectsSO kitchenObjectsSO;
    [SerializeField] private GameObject spawnPoint;
    public void Interact(){
        Debug.Log("Interacting");
        // Instantiate the tomatoPrefab as a GameObject
        GameObject kitchenObject = Instantiate(kitchenObjectsSO.prefab, spawnPoint.transform.position, Quaternion.identity);

        // Optionally, you can parent the tomatoObject to the spawnPoint
        kitchenObject.transform.parent = spawnPoint.transform;
    }
}
