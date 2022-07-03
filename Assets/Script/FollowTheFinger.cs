using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTheFinger : MonoBehaviour
{

    private Vector3 firstClick; // parmak ile dokunulan ilk nokta
    private Ray rayPoint;
    private RaycastHit raycast; 

    // Update is called once per frame
    void Update()
    {
        CatchTheFinger();
    }

    private void CatchTheFinger()
    {
        if(Input.touchCount>0)
        {
            Touch touch= Input.GetTouch(0);
                     
            rayPoint=Camera.main.ScreenPointToRay(touch.position);   

            if(Physics.Raycast(rayPoint,out raycast,100,64))
            {   
                switch (touch.phase)
                {
                case TouchPhase.Began:
                    FirstClickPosition(raycast.point);
                    break;
                case TouchPhase.Moved:
                    FollowTheFingerPosition(raycast.point);
                    break;                  
                }           
            }
        }
    }

    private void FirstClickPosition(Vector3 firstPos)
    {
        firstClick= firstPos;
    }

    private void FollowTheFingerPosition(Vector3 catchFinger)
    {
        Vector3 calculateThePoint=transform.position+(catchFinger-firstClick);  
        transform.position=Vector3.Lerp(transform.position,calculateThePoint,0.01f);  
    }
}
