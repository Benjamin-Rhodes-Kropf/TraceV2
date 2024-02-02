using UnityEngine;

public class CustomCircleMarkerManager : MonoBehaviour
{
    [Header("Press C key to draw circles" + "\n")]

    [SerializeField]
    //Circles data
    private Circles[] circles;
    //Accesing the markermanager so that we can create marker and asign variables
    [SerializeField]
    private OnlineMapsMarkerManager markerManager;
    //in case we need that drawing circle script
    [SerializeField]
    private DrawCircleAroundMarker circleDrawingManager;

    [Header("Adjust radius to draw different circles, e,g 0.01")]
    //radius just to make sure that size of circle markers are not uniform
    public float radiusKM;
    
    // Update is called once per frame
    void Update()
    {
        //if we press the c key then teh colored circle markers will be placed on map
        if (Input.GetKeyUp(KeyCode.C))
        {
            if (radiusKM <= 0.1f)//100meters
            {
                markerManager.defaultTexture = circles[0].circles;
                markerManager.defaultScale = 0.2f;
            }
            else if (radiusKM > 0.1f && radiusKM<=0.2f)//200meters
            {
                markerManager.defaultTexture = circles[1].circles;
                markerManager.defaultScale = 0.4f;
            }
            else if (radiusKM > 0.2f && radiusKM <= 0.3f)//300meters
            {
                markerManager.defaultTexture = circles[2].circles;
                markerManager.defaultScale = 0.6f;
            }
            else if (radiusKM > 0.3f && radiusKM <= 0.4f)//400meters
            {
                markerManager.defaultTexture = circles[3].circles;
                markerManager.defaultScale = 0.8f;
            }
            else//rest radius
            {
                markerManager.defaultTexture = circles[4].circles;
                markerManager.defaultScale = 1f;
            }
            //gerenrate the marker on map
            markerManager.GenerateMarker();
        }
    }
}
[System.Serializable]
public class Circles
{
    public string Name;
    public Texture2D circles;
}