using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelvesManager : Singleton<ShelvesManager>
{
    [SerializeField] private Transform[] shelvesRooms;
    public Transform GetPlaceInShelves(int index)
    {
        return shelvesRooms[index];
    }
}
