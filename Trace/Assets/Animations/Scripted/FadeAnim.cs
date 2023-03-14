using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;

public class FadeAnim : MonoBehaviour
{
    [Header("Fade Components")]
    [SerializeField] List<GameObject> objects = new List<GameObject>();
    [SerializeField] List<Image> imgs = new List<Image>();
    [SerializeField] List<TMP_Text> txts = new List<TMP_Text>();
    [SerializeField] private List<Color> initalColor = new List<Color>();
    [SerializeField] private List<Color> targetColor = new List<Color>();
    [SerializeField] private float fadeDuration;
    private Canvas canvas;
    
    [Header("Fade Options")] 
    [SerializeField] private int startSortOrder;
    [SerializeField] private int endSortOrder;
    [SerializeField] private bool moveToDisabledChild;
    [SerializeField] private Transform disabledParent;
    [SerializeField] private bool fadeInOnEnabled;
    [SerializeField] private float waitBeforeFade;
    
    //Todo: fade out all children and child game object to inactive 
    //Todo: optimze this so that it dosnt store whole color and accses things less
    //Todo: make Fade Options Work

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = startSortOrder;
        
        foreach (var obj in objects)
        {
            var colorableImage = obj.GetComponent<Image>();
            if (colorableImage != null)
            {
                imgs.Add(colorableImage);
            }
            var colorableText = obj.GetComponent<TMP_Text>();
            if (colorableText != null)
            {
                txts.Add(colorableText);
            }
        }
        
        foreach (var image in imgs)
        {
            initalColor.Add(image.color);
            targetColor.Add(new Color(image.color.r,image.color.g, image.color.b, 0));
        }
        foreach (var txt in txts)
        {
            initalColor.Add(txt.color);
            targetColor.Add(new Color(txt.color.r,txt.color.g, txt.color.b, 0));
        }
    }

    void OnEnable()
    {
        Debug.Log("FadeAnim: Enabled!");
    }
    
    public void FadeOut()
    {
        Debug.Log("FadeAnim: fading out");
        StartCoroutine(FadeOutCorutine());
    }
    private IEnumerator FadeOutCorutine()
    {
        yield return new WaitForSeconds(waitBeforeFade);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            int counter = 0;
            foreach (var image in imgs)
            {
                image.color = Color.Lerp(initalColor[counter], targetColor[counter], elapsedTime / fadeDuration);
                counter++;
            }
            foreach (var txt in txts)
            {
                txt.color = Color.Lerp(initalColor[counter], targetColor[counter], elapsedTime / fadeDuration);
                counter++;
            }
            
            yield return null;
        }

        canvas.sortingOrder = endSortOrder;
        gameObject.transform.parent = disabledParent;
    }
    
    public void FadeIn()
    {
        StartCoroutine(FadeInCorutine());
    }
    private IEnumerator FadeInCorutine()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            int counter = 0;
            foreach (var image in imgs)
            {
                image.color = Color.Lerp(targetColor[counter], initalColor[counter], elapsedTime / fadeDuration);
                counter++;
            }
            foreach (var txt in txts)
            {
                txt.color = Color.Lerp(targetColor[counter], initalColor[counter], elapsedTime / fadeDuration);
                counter++;
            }
            
            yield return null;
        }
    }
}
