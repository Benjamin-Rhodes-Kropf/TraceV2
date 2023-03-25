using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using Newtonsoft.Json.Bson;

public class SwipeUpManager : MonoBehaviour, IDragHandler, IEndDragHandler
{
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
			StartCoroutine(SwitchToSelectUsersScreen());
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
	
	IEnumerator SwitchToSelectUsersScreen()
	{
		//wait until arrow has exited the screen
		while(changeInYVal < changeInYvalGoTrigger)  {
			yield return null;
		}
		Debug.Log("SwipeUpManager:" + "Switch To Selector Screen");
		BackToMainScene();
		yield return new WaitForSeconds(0.7f);
		ScreenManager.instance.ChangeScreenUpSlideOver("SelectFriends");
	}
	
	public void BackToMainScene() {
		//change the bool so that the main canavs can be enabled after the main scene is loaded
		ScreenManager.instance.isComingFromCameraScene = true;
		SceneManager.LoadScene(0);
		ScreenManager.instance.camManager.cameraPanel.SetActive(false);//disabling the camera panel
	}
}