using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using MoreMountains.NiceVibrations;
public static class HelperMethods
{
    public static Sprite LoadSprite(string path, string filename, string fallBackFile = "")
    {
        var texture2D = Resources.Load<Texture2D>(path + filename);
        if (texture2D != null)
            return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        else
        {       
            texture2D = Resources.Load<Texture2D>(path + fallBackFile);
            if (texture2D != null)
                return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
            else
                return null;
        }
    }

    // public static List<T> AppendList<T>(List<T> sourceList, List<T> listToAppend)
    // {
    //     foreach (var item in sourceList)
    //     {
    //         listToAppend.Add(item);
    //     }
    //
    //     return listToAppend;
    // }
    
    public static List<T> AppendList<T>(this List<T> sourceList, List<T> listToAppend)
    {
        foreach (var item in listToAppend)
        {
            sourceList.Add(item);
        }

        return sourceList;
    }
    
    public static void MeshCulling(GameObject parent, int value)
    {
        Renderer[] renderComponents = parent.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderComponents.Length; i++)
            renderComponents[i].material.SetInt("_Cull", value);
    }

    public static void SetLayer(GameObject parent, int layer, bool includeChildrens = true)
    {
        // Setting the Layer for Proper Lighting
        parent.layer = layer;
        Transform[] layerTransforms = parent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < layerTransforms.Length; i++)
            layerTransforms[i].gameObject.layer = layer;
    }

    public static string ConvertActualString(string input)
    {

        Debug.LogError("ConvertActualString " + input);
        int character = input.Last();
        while (character < '!' || character > '~')
        {
            input = input.Remove(input.Count() - 1);
            character = input.Last();
        }


        return input;
    }

    public static IEnumerator DownloadImage(string url, Action<UnityWebRequest> OnSuccess, Action<string> OnFail)
    {
        Uri uriResult;
        bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        if (!result)
        {
            if (OnFail != null)
                OnFail.Invoke("Invalid Url");
        }
        else
        {
            //Debug.Log("Download Start url : " + url);
            UnityWebRequest m_Request = UnityWebRequestTexture.GetTexture(url);
#if UNITY_WEBGL
            m_Request.SetRequestHeader("Access-Control-Allow-Credentials", "true");
            m_Request.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
            m_Request.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            m_Request.SetRequestHeader("Access-Control-Allow-Origin", "*");
#endif
            yield return m_Request.SendWebRequest();

            if (m_Request.isNetworkError || m_Request.isHttpError)
            {
                Debug.Log("Download Error : " + m_Request.error + " url : " + url);
                if (OnFail != null)
                    OnFail.Invoke(m_Request.error);
            }
            else
            {
                //Debug.Log("Download Successfull url : " + url);
                if (OnSuccess != null)
                    OnSuccess.Invoke(m_Request);
            }
        }
    }

    public static IEnumerator DelayedCall(float delay, Action OnSuccess)
    {
        yield return new WaitForSeconds(delay);
        if (OnSuccess != null)
            OnSuccess.Invoke();
    }

    public static bool isBadName(string name)
    {
        string userEnteredName = name.ToLower();
        List<string> badNames = new List<string> ();
        foreach (string badName in badNames)
        {
            if (badName.Contains(userEnteredName))
            {
                return true;
            }
            else if (userEnteredName.Contains(badName))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsEmailValid(string emailId)
    {
        Regex mailValidator = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$");
        string stringToReturn = null;

        bool returnValue = false;

        if (string.IsNullOrEmpty(emailId))
        {
            stringToReturn = "Field cannot be empty";
            returnValue = false;
        }
        else if (mailValidator.IsMatch(emailId))
        {
            stringToReturn = null;
            returnValue = true;
        }
        else
        {
            stringToReturn = "Invalid Email Format";
            returnValue = false;
        }

        //return stringToReturn;
        return returnValue;
    }
    
    public static string IsValidPassword(string password)
    {
        Regex passwordValidator = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$");
        string stringToReturn = null;

        if (string.IsNullOrEmpty(password))
        {
            stringToReturn = "Password cannot be empty";
        }
        else if (password.Length >= 6)
        {
            stringToReturn = null;
            
            if (!passwordValidator.IsMatch(password))
            {
                stringToReturn = "Invalid Password: Must include atleast 1 Small character, \n1 Capital character, 1 Number, and 1 Special character";
            }
        }
        else
        {
            stringToReturn = "Length minimum 8 characters";
        }

        return stringToReturn;
    }

    public static string GetKeyFromQuery(string url)
    {
        string dataToReturn = null;

        Debug.Log("GetKeyFromQuery : URL : " + url);
        UriBuilder uri = new UriBuilder(url);
        Debug.Log("GetKeyFromQuery : Query : " + uri.Query);

        var data = url.Split('?');

        if (data == null || data.Length <= 0)
            dataToReturn = null;

        else if (data.Length > 1)
        {
            Debug.Log("GetKeyFromQuery : data : " + data[1]);
            
            var queryDataSplit = data[1].Split('=');

            if (queryDataSplit == null || queryDataSplit.Length <= 0)
                dataToReturn = null;
            else
                dataToReturn = queryDataSplit[0];
        }

        return dataToReturn;
    }
    
    public static List<string> GetEnumList<T>()
    {
        List<string> _list = new List<string>();
        
        foreach (var value in Enum.GetValues(typeof(T)))
        {
            _list.Add(value.ToString());
        }

        return _list;
    }

    public static IEnumerator TimedActionFunction(float timer, Action callback)
    {
        yield return new WaitForSeconds(timer);
        callback?.Invoke();
    }
    
    public static IEnumerator LerpScroll(ScrollRect _scrollRect,float target, float overTime)
	{
		float startTime = Time.time;
		while (Time.time < startTime + overTime)
		{
			_scrollRect.verticalNormalizedPosition = Mathf.Lerp(_scrollRect.verticalNormalizedPosition, target, (Time.time - startTime) / overTime);
			yield return null;
		}
		_scrollRect.verticalNormalizedPosition = target;
	}

    public static void PlayHeptics()
    {
        MMVibrationManager.Haptic(HapticTypes.LightImpact);
    }
    
    public static void OpenLink(string _Link)
    {
#if UNITY_WEBGL
        OpenWindow(_Link);
#else
        Application.OpenURL(_Link);
#endif
    }
#if UNITY_WEBGL

    [DllImport("__Internal")]
    private static extern void OpenWindow(string url);
#endif
}
