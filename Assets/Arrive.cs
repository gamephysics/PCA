using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//====================================================================
// Class: Arrive 
// Desc : set position dest gameObjects & Order Unit Move to destination
//====================================================================
public class Arrive : MonoBehaviour
{
    public Squads   squads = null;
    public float    spacing = 4;

    private Vector3 center_of_dest = Vector3.zero;

    private GameObject dirobj = null;
    private GameObject centerobj = null;
    //======================================
    // Destination GameObject Initial Position
    //======================================
    private void Awake()
    {
        var objD = Resources.Load<GameObject>("Prefabs/Dir");
        dirobj = GameObject.Instantiate(objD);

        var objC = Resources.Load<GameObject>("Prefabs/Center");
        centerobj = GameObject.Instantiate(objC);
    }
    void Start()
    {
        if (squads != null && squads.units != null)
        {
            PCA();
        }
    }

    //======================================
    // Mouse Click & Set Formation Position & Move Unit
    //======================================
    void Update()
    {
        if (squads != null && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                squads.RandomDistribute(hit.point, 10);
                PCA();
            }
        }
    }



    //======================================
    // Matching Destination & Move to Matched Position
    //======================================
    void PCA()
    {
        if (squads == null || squads.units == null)
            return;

        Vector3 center = Vector3.zero;
        foreach (var unit in squads.units)
        {
            center += unit.transform.position;
        }
        center /= squads.units.Count;

        List<Vector3> positions = new List<Vector3>();
        foreach (var unit in squads.units)
        {
            positions.Add(unit.transform.position - center);
        }

        Vector3 dir = PrincipalAxis(positions);


        // SET GAME OBJECT POSITION & ROTATION
        if(dirobj != null)
        {
            dirobj.transform.position = center;
            dirobj.transform.forward  = dir;
        }
        if(centerobj != null)
        {
            centerobj.transform.position = center;
        }
    }
    Vector3 PrincipalAxis(List<Vector3> Points)
    {
        //============================================================
        // CHECK DATA
        //============================================================
        Int32 PointCount = (Int32)Points.Count;
        if (PointCount <= 1)
        {
            return Vector3.right;
        }

        //============================================================
        // COVARIANCE MATRIX
        //============================================================
        Vector3[] cov = new Vector3[3];
        for (Int32 u = 0; u < 3; ++u)
        {
            for (Int32 v = 0; v < 3; ++v)
            {
                for (Int32 i = 0; i < PointCount; ++i)
                {
                    cov[u][v] += Points[i][u] * Points[i][v];
                }
            }
            cov[u] /= (float)(PointCount - 1);
        }

        //============================================================
        // EIGEN VALUE, VECTOR
        //============================================================
        //  | a   b |
        //	|       |  Symmetric Matrix : (b == c) 
        //  | c   d |
        float a = cov[0][0];
        float b = cov[0][2];
        float c = cov[2][0];
        float d = cov[2][2];
        float T = a + d;
        float D = a * d - b * c;
        float L1 = T / 2.0f + Mathf.Sqrt(T * T / 4.0f - D);
        float L2 = T / 2.0f - Mathf.Sqrt(T * T / 4.0f - D);

        Vector3 X = Vector3.zero;
        Vector3 Z = Vector3.zero;
        if (Mathf.Abs(b) <= Mathf.Epsilon && Mathf.Abs(c) <= Mathf.Epsilon)
        {
            //|  1  |   |  0  |
            //|     | , |     |
            //|  0  |   |  1  |

            X = new Vector3(1, 0, 0);
            Z = new Vector3(0, 0, 1);
        }
        else if (Mathf.Abs(b) > Mathf.Epsilon)
        {
            //|	   b   |   |   b    |
            //|	       |,  |	    |
            //| L1 - a |   | L2 - a |

            X = new Vector3(b, 0, L1 - a);
            Z = new Vector3(b, 0, L2 - a);
            X.Normalize();
            Z.Normalize();
        }
        else if (Mathf.Abs(c) > Mathf.Epsilon)
        {
            //| L1 - d |   | L2 - d |
            //|        |,  |        |
            //|   c    |   |   c    |

            X = new Vector3(L1 - d, 0, c);
            Z = new Vector3(L2 - d, 0, c);
            X.Normalize();
            Z.Normalize();
        }

        Debug.Assert(Mathf.Abs((b*c - a * d) + (a + d)*L1 - L1 * L1) > Mathf.Epsilon);
        Debug.Assert(Mathf.Abs((b*c - a * d) + (a + d)*L2 - L2 * L2) > Mathf.Epsilon);

        return X;
    }
}
