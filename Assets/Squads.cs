using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//====================================================================
// Class: Squads
// Desc : Spawn Units from (public int unitCount = 4;)
//====================================================================
public class Squads : MonoBehaviour
{
    public int unitCount = 4;
    // UNIT
    [ReadOnly]
    public List<GameObject> units = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        units.Clear();

        var obj = Resources.Load<GameObject>("Prefabs/Unit");

        for (int i = 0; i < unitCount; ++i)
        {
            var unit = GameObject.Instantiate(obj);
            unit.transform.parent = this.transform;
            units.Add(unit);
        }

        RandomDistribute(Vector3.zero, 10);
    }

    public void RandomDistribute(Vector3 center, float size)
    {
        // Random dir 
        Vector3 dir = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
        dir.Normalize();
        // dir Rotation Quaternion
        var rot = Quaternion.LookRotation(dir);
        
        size = Mathf.Abs(size);

        foreach (var unit in units)
        {
            // random position
            unit.transform.position = center + rot * new Vector3(UnityEngine.Random.Range(-size, size), 0, UnityEngine.Random.Range(-size/4, size/4));
        }

    }
}
