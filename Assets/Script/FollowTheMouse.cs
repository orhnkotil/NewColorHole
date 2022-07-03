using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowTheMouse : MonoBehaviour
{
    public NavMeshAgent blackHole;
    private Vector3 firstClickOnScreen; // mouse ile dokunulan ilk nokta
    private Vector3 followMouse; // mouse ile dokunulan ilk nokta
    private Vector3 calculateThePoint; // moouse hareketi ile aynı doğruldudaki vektör
    private Ray rayPoint;
    private RaycastHit raycast; 
  
    // Update is called once per frame
    void Update()
    {
       FollowMousePositoin();
    }


    private void FollowMousePositoin()
    {

            rayPoint=Camera.main.ScreenPointToRay(Input.mousePosition);              
            if(Physics.Raycast(rayPoint,out raycast,100,64))
            {   
                MouseLeftClickDown(raycast.point); 
                MouseButtonGet(raycast.point);               
            }

    }

    private void MouseLeftClickDown(Vector3 firstClick)
    {
        if(Input.GetMouseButtonDown(0))
        {    
           firstClickOnScreen = firstClick; // A ilk nokta         
        }
    }

    private void MouseButtonGet(Vector3 follow)
    {
        if(Input.GetMouseButton(0))
        {
            followMouse=follow; // B son nokta
            calculateThePoint=transform.position+(followMouse-firstClickOnScreen); // u vektörü hesaplanarak deliğin gideceği koordinatlar bulundu
            transform.position=Vector3.Lerp(transform.position,calculateThePoint,0.01f);          
        }
    }

    

}
