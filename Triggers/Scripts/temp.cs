using System;
using System.Collections;
using System.Collections.Generic;
using ScottEwing.Triggers;
using UnityEngine;

public class temp : MonoBehaviour{
    [SerializeField] private LookInteractTrigger _trigger;
    [SerializeField] private GameObject _gameObject;

    private void Update() {
        print("Active Self: " + _gameObject.activeSelf);
        print("Active In Hierarchy: " + _gameObject.activeInHierarchy);
        
        print("Enabled: " + _trigger.enabled);

    }
}
