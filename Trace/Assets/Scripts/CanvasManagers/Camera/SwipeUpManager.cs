using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using Newtonsoft.Json.Bson;
using UnityEngine.Events;

[Serializable]
public class SwipeUpManager : MonoBehaviour, IDragHandler, IEndDragHandler
{
	[Header("CameraManager")] 
	[SerializeField] private CameraManager cameraManager;
	[SerializeField] private bool isSendingPhoto;
	
	[Header("Swipe Physics")]
	RectTransform m_transform = null;
	[SerializeField] private float initailYVal;
	[SerializeField] private float changeInYVal;
	[SerializeField] private float Dy;
	[SerializeField] private float friction = 0.95f;
	[SerializeField] private float frictionWeight = 1f;
	[SerializeField] private float changeInYvalGoLimit;
	[SerializeField] private float changeInYvalGoTrigger;
	[SerializeField] private bool hasBegunScreenSwitch;
	[SerializeField] private float dyLimitForScreenSwitch;
	[SerializeField] private bool isDragging;
	[SerializeField] private AnimationCurve slideFrictionCurve;
	[SerializeField] private AnimationCurve slideRestitutionCurve;



	private void OnEnable()
	{
		hasBegunScreenSwitch = false;
	}
	void Start () {
		m_transform = GetComponent<RectTransform>();
		initailYVal = m_transform.position.y;
	}
	private void Update()
	{
		changeInYVal =  m_transform.position.y-initailYVal;
		if (!isDragging)
		{
			m_transform.position = new Vector3(m_transform.position.x, m_transform.position.y + Dy*frictionWeight + slideRestitutionCurve.Evaluate(changeInYVal)*100f);
		}

		//only apply friction before screen switch
		if (!hasBegunScreenSwitch)
		{
			Dy *= friction;
		}
		
		//if they throw the arrow off the bottom of the screen
		if (changeInYVal < -800)
		{
			Dy = 0;
		}

		if (changeInYVal > changeInYvalGoLimit && !hasBegunScreenSwitch && !isDragging && Dy > dyLimitForScreenSwitch)
		{
			StartCoroutine(TraceArrowSlidUp());
			hasBegunScreenSwitch = true;
		}
	}
	
	public void OnDrag(PointerEventData eventData)
	{
		isDragging = true;
		Dy += eventData.delta.y;
		m_transform.position += new Vector3(0, eventData.delta.y * slideFrictionCurve.Evaluate(changeInYVal));
    }

	public void OnEndDrag(PointerEventData eventData)
	{
		isDragging = false;
	}

	private void Reset()
	{
		hasBegunScreenSwitch = false;
		changeInYVal = 0;
		Dy = 0;
		m_transform.position = new Vector3(m_transform.position.x, initailYVal);
	}

	IEnumerator TraceArrowSlidUp()
	{
		//wait until arrow has exited the screen
		while(changeInYVal < changeInYvalGoTrigger)  {
			yield return null;
		}
		//determine which function to call
		if (isSendingPhoto)
		{
			cameraManager.ShareImage();
		}
		else
		{
			cameraManager.ShareVideo();
		}
		BackToMainScene();
		Reset();
		ScreenManager.instance.ChangeScreenUpSlideOver("SelectFriends");
	}
	
	public void BackToMainScene() {
		ScreenManager.instance.camManager.cameraPanel.SetActive(false);//disabling the camera panel
		ScreenManager.instance.camManager.videoPreviewPanel.SetActive(false);//disabling the camera panel
		ScreenManager.instance.camManager.imagePreviewPanel.SetActive(false);//disabling the camera panel

	}
}