using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounter : MonoBehaviour
{
    [SerializeField] private Counter counter;
    [SerializeField] private GameObject visualSelectedCounter;

    // Start
    private void Start()
    {
        Player.Instance.OnSelectingCounter  += Instance_OnselectingCounter;
    }

    private void Instance_OnselectingCounter(object sender, Player.OnSelectedCounterEventArgs e){
        if(e.selectedCounter == counter){
            show();
        }else{
            hide();
        }
    }

    private void show(){
        visualSelectedCounter.SetActive(true);
    }

    private void hide(){
        visualSelectedCounter.SetActive(false);
    }

}
