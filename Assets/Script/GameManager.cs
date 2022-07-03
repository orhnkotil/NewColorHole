using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
#region  Varaible area
    [Tooltip("Sahnede uzak durulması gereken küpün Prefab' i")]
    public GameObject enemyCube;
    [Tooltip("Sahnede toplamamız gereken küpün Prefab' i")]
    public GameObject getCube;
    private GameObject[] enmeyPoint; // Düşman küplerin sahnede bulunacakları pozisyonları almak için
    private GameObject[] getCubePoint; // Toplanması gereken küplerin sahnede bulunacakları pozisyonları tutmak için
    private Vector3 positionOfCube= new Vector3(0,0.2f,0); // Üst üste dizilen küplerin pozisyonlarını ayarlamak için
    private CollectTheCube cube; // script e ulaşmak için 
    private GameObject holeObject; //karadelik üzerindeki compenentleri alabilmek için objesine ulaştık
    private Camera cam; // bölüm geçişinde cameranın takibi için cameraya eriştik
    private NavMeshAgent navMeshAgent; // level2 tamamlandığı zaman level3 e geçmek için 
    private FollowTheFinger followTheFinger; // bölüm geçişlerinde script i kapatmak için kullanıldı
    private Animator anim; // kapının üzerindeki animasyonu oynatmak için kullanıldı
    private int totalCubeNumber; // her bölümle kaç tane küp olduğunun sayısı tutuldu 
    private Slider[] sld; // progress barları tutmak için liste

    // Proggres barlara ulaşmak için kullanıldı
    private Slider slider2;
    private Slider slider3;
    private Text txt; // text üzerinden sonuçları yazdırmak için kullanıldı.
    private Text coinText; // Coin i yazdırmak için.
    Vector3 level3Position= new Vector3(1.5f,0.95f,9f);// level 3 teki adrese gidecek
    
# endregion

    // Start is called before the first frame update
    void Start()
    {
        
        FindObject();
        CreateEnemyCube();
        CreateGetCube();
        GetComponentFromBlackHole();
        StartCoroutine(HowManyCubeDestroy(2));
        StartCoroutine(ControlOfCanvas(0,totalCubeNumber));
        StartCoroutine(WrongCubeisDestroy());
    }

#region  Metot Area 
    private void FindObject()
    {
        enmeyPoint=GameObject.FindGameObjectsWithTag("EnemyPoint");
        getCubePoint=GameObject.FindGameObjectsWithTag("GetCubePoint"); 
        holeObject=GameObject.FindGameObjectWithTag("BlackHole");         
        cam=GameObject.FindObjectOfType<Camera>();    
        slider2=GameObject.FindGameObjectWithTag("slider2").GetComponent<Slider>();
        slider3=GameObject.FindGameObjectWithTag("slider3").GetComponent<Slider>();
        txt=GameObject.FindGameObjectWithTag("Text").GetComponent<Text>();
        coinText=GameObject.FindGameObjectWithTag("CoinText").GetComponent<Text>();
        anim=GameObject.FindGameObjectWithTag("Gate").GetComponent<Animator>();
        anim.enabled=false;
        
    }

    private void GetComponentFromBlackHole()
    {
        cube=holeObject.GetComponent<CollectTheCube>();
        navMeshAgent=holeObject.GetComponent<NavMeshAgent>();   
        followTheFinger= holeObject.GetComponent<FollowTheFinger>();

    }

    private void CreateEnemyCube()
    {
        //Enemy cube ler üretilecek
        for (int i = 0; i < enmeyPoint.Length; i++)
        {
            Instantiate(enemyCube,enmeyPoint[i].transform.position,Quaternion.identity); //düşman küpler üretildi
        }
    }

    private void CreateGetCube() 
    {
        //Level2 deki küpler waypointlere göre üretiliyor
        for (int i = 0; i < getCubePoint.Length; i++)
        {
            switch (i)
            {
                case 0:
                    HowManyCubeCreate(5,getCubePoint[i].transform.position);
                    break;
                case 1:
                    HowManyCubeCreate(3,getCubePoint[i].transform.position);
                    break;
                case 2:
                    HowManyCubeCreate(6,getCubePoint[i].transform.position);
                    break;
                case 3:
                    HowManyCubeCreate(6,getCubePoint[i].transform.position);
                    break;
            }
            
        }
    }

    private void HowManyCubeCreate(int number,Vector3 position)
    {
        // her bir waypoint' deki küp sayısı farklı olduğundan CreateGetCube metoduna yardıncı bir metot yazıldı
        for (var i = 0; i < number; i++)
        {
            Instantiate(getCube,position+positionOfCube*i,Quaternion.identity);
            totalCubeNumber+=1;
        }
    }

    private IEnumerator HowManyCubeDestroy(int levelNunber) // kazandınız yazdırmak için
    {
        while(totalCubeNumber!=cube.numberOfDestroy)
        {         
            yield return new WaitForSeconds(0.1f);                 
        }

        txt.text="TEBRİKLER Level "+levelNunber+" geçtiniz";
        yield return null;
    }

    private IEnumerator ControlOfCanvas(int sliderNumber, int maxCubNumber)
    {  
        sld =new Slider[2]{ slider2,slider3};
        sld[sliderNumber].maxValue=maxCubNumber;  

        while(sld[sliderNumber].maxValue!=sld[sliderNumber].value)
        {
            sld[sliderNumber].value=cube.numberOfDestroy;
            yield return new WaitForSeconds(0.1f);
        }

        StartCoroutine(AnimationIsActive(sliderNumber));       
        yield return null;
    }

    private IEnumerator AnimationIsActive(int whichLevel)
    {
        if(whichLevel==0)
        {
            anim.enabled=true;
            yield return new WaitForSeconds(1.2f);
            StartCoroutine(ReadyToGoLevel3());   
        }
        yield return null;

    }

    private IEnumerator WrongCubeisDestroy() 
    {       
        while(!cube.enemy)
        {
            yield return new WaitForSeconds(0.1f);          
        }  

        txt.text="Yanlış Küp :(";
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("SampleScene"); 
        yield return null;
    }


    private IEnumerator ReadyToGoLevel3()
    {         
            yield return new WaitForSeconds(0.2f);
            followTheFinger.enabled=false;
            holeObject.GetComponent<Collider>().isTrigger=false;
            navMeshAgent.SetDestination(level3Position);// level 3 teki adrese gidecek         
            StartCoroutine(FollowTheHole());
            yield return new WaitForSeconds(5f);
            StartCoroutine(Oldumu());

        yield return null;
    }

    private IEnumerator Oldumu()
    {
        while(holeObject.transform.position!=level3Position)
        {
            yield return new WaitForSeconds(0.1f);       
        }
        navMeshAgent.enabled=false;
        StartCoroutine(GoToLevelThree());
        
        yield return null;
    }

    private IEnumerator FollowTheHole()
    {
        while(cam.transform.position!=holeObject.transform.position)
        {
            cam.transform.position=new Vector3(0,10f,-2.5f)+holeObject.transform.position;
            yield return new WaitForSeconds(0.01f);
        }
        
        yield return null;
    }

    private IEnumerator GoToLevelThree()
    {      
        StopAllCoroutines();
        txt.text="";
        navMeshAgent.enabled=true;
        followTheFinger.enabled=true;
        holeObject.GetComponent<Collider>().isTrigger=true;
        StartCoroutine(ContiniuLevelThree());
        yield return null;
    }

    private IEnumerator ContiniuLevelThree()
    {
        totalCubeNumber=230;
        cube.numberOfDestroy=0;       
        StartCoroutine(ControlOfCanvas(1,totalCubeNumber));   
        StartCoroutine( WrongCubeisDestroy());  
        StartCoroutine(HowManyCubeDestroy(3));
        coinText.text=200.ToString();  
        yield return null;
    }

    
# endregion

}
