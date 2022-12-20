using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Storage;
using Unity.VisualScripting;
using UnityEngine.Networking;
using UnityEngine.UI;
using Object = System.Object;

public class FbManager : MonoBehaviour
{
    //Dont Destroy
    public static FbManager instance;
    
    //Firebase References
    private DependencyStatus dependencyStatus;
    private FirebaseAuth auth;    
    private FirebaseUser fbUser;
    private DatabaseReference DBref;
    private FirebaseStorage storage;
    private StorageReference storageRef;
    
    //Trace User
    private TraceUser traceUser;
    
    [Header("Firebase")]
    [SerializeField] private String _storageReferenceUrl;

    [Header("ScreenManager")] 
    [SerializeField] private ScreenManager _screenManager;

    [Header("Settings")] 
    [SerializeField] private bool autoLogin;
    [SerializeField] private bool forceLogin;
    [SerializeField] private bool useAdminForLogin;
    [SerializeField] private string adminUser;
    [SerializeField] private string adminPass;

    
    [Header("Trace User Data")] 
    public Texture userImageTexture;
    
    [Header("Database Test Assets")]
    public RawImage rawImage;
    public RawImage testRawImage;
    
    //INITIALIZER
    void Awake()
    {
        if (instance != null)
        {Destroy(gameObject);}
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl(_storageReferenceUrl);
        
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }
    private void InitializeFirebase()
    {
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBref = FirebaseDatabase.DefaultInstance.RootReference;
    }
    private void Start()
    {
        if (forceLogin)
        {
            StartCoroutine(ForceLogin());
        } else if (autoLogin)
        {
            if (useAdminForLogin)
            {
                PlayerPrefs.SetString("Username", adminUser);
                PlayerPrefs.SetString("Password", adminPass);
            }
            else
            {
                if (PlayerPrefs.GetString("Username") == adminUser)
                {
                    PlayerPrefs.SetString("Username", null);
                    PlayerPrefs.SetString("Password", null);
                }
            }
        }
        else
        {
            PlayerPrefs.SetString("Username", null);
            PlayerPrefs.SetString("Password", null);
        }

        if (!forceLogin)
        { 
            StartCoroutine(AutoLogin());
        } 
    }

    #region This User
    
    #region Valdiation Of User
    
    public void LogOutOfAccount()
    {
        StartCoroutine(LogOut());
    }
    public IEnumerator ForceLogin()
    {
        //Todo: figure out which wait until to use...
        Debug.Log("Overiding Logging in");
        yield return new WaitForSeconds(0.5f); //has to wait until firebase async task is finished... (is there something better?)
        Debug.Log("Logging 0.4s");
        _screenManager.ChangeScreenDown("HomeScreen");
    }

    public IEnumerator AutoLogin()
    {
        //Todo: figure out which wait until to use...
        Debug.Log("Auto Logging in");
        yield return new WaitForSeconds(0.4f); //has to wait until firebase async task is finished... (is there something better?)
        Debug.Log("Auto Logging 0.4s");
        String savedUsername = PlayerPrefs.GetString("Username");
        String savedPassword = PlayerPrefs.GetString("Password");
        Debug.Log("saved user:" +  PlayerPrefs.GetString("Username"));
        if (savedUsername != "null" && savedPassword != "null")
        {
            Debug.Log("auto logging in");
            StartCoroutine(FbManager.instance.Login(savedUsername, savedPassword, (myReturnValue) => {
                if (!myReturnValue.IsSuccessful)
                {
                    Debug.LogError("FbManager: failed to auto login");
                    StartCoroutine(LogOut());
                }
                else
                {
                    Debug.Log("FbManager: Logged in!");
                    _screenManager.ChangeScreenDown("HomeScreen");
                }
            }));
        }
        else
        {
            Debug.Log("pulling up login options");
            _screenManager.WelcomeScreen();
        }
    }
    public IEnumerator Login(string _email, string _password,  System.Action<CallbackObject> callback)
    {
        Debug.Log("Login Started");
        CallbackObject callbackObject = new CallbackObject();
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);
        
        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = "Login Failed!";
            callbackObject.IsSuccessful = false;
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    callbackObject.message = message;
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    callbackObject.message = message;
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    callbackObject.message = message;
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    callbackObject.message = message;
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    callbackObject.message = message;
                    break;
            }
            Debug.Log("FBManager: failed to log in");
            callbackObject.IsSuccessful = false;
            callbackObject.message = message;
            callback(callbackObject); //return
            yield break;
        }

        fbUser = LoginTask.Result;
        Debug.LogFormat("User signed in successfully: {0} ({1})", fbUser.DisplayName, fbUser.Email);
        Debug.Log("logged In: user profile photo is: " + fbUser.PhotoUrl);
        callbackObject.IsSuccessful = true;
        //Load User Profile Texture
        /*StartCoroutine(FbManager.instance.GetMyUserProfilePhoto((myReturnValue) => {
            if (myReturnValue != null)
            {
                userImageTexture = myReturnValue;
            }
        }));*/
        
        //all database things that need to be activated
        var DBTaskSetIsOnline = DBref.Child("users").Child(fbUser.UserId).Child("isOnline").SetValueAsync(true);
        yield return new WaitUntil(predicate: () => DBTaskSetIsOnline.IsCompleted);
        
        //stay logged in
        PlayerPrefs.SetString("Username", _email);
        PlayerPrefs.SetString("Password", _password);
        PlayerPrefs.Save();
        
        callback(callbackObject);
    }
    private IEnumerator LogOut()
    {
        Debug.Log("FBManager: logging out");
        PlayerPrefs.SetString("Username", "null");
        PlayerPrefs.SetString("Password", "null");
        userImageTexture = null;
        auth.SignOut();
        yield return new WaitForSeconds(0.8f);
        _screenManager.WelcomeScreen();
        //_screenManager.PullUpOnboardingOptions();
    }
    #endregion

    #region Register New User
    private string GenerateUserProfileJson(string username, string name, string userPhotoLink, string email, string phone) {
        TraceUser user = new TraceUser(username, name, userPhotoLink, email, phone);
        string json = JsonUtility.ToJson(user);
        return json;
    }
    public IEnumerator RegisterNewUser(string _email, string _password, string _username, string _phoneNumber,  System.Action<String> callback)
    {
        if (_username == "")
        {
            callback("Missing Username"); //having a blank nickname is not really a DB error so I return a error here
            yield break;
        }

        //Call the Firebase auth signin function passing the email and password
        var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

        if (RegisterTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
            FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Register Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WeakPassword:
                    message = "Weak Password";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "Email Already In Use";
                    break;
            }
            Debug.LogWarning(message);
            callback(message);
            yield break;
        }

        //User has now been created
        fbUser = RegisterTask.Result;

        if (fbUser == null)
        {
            Debug.LogWarning("User Null");
            yield break;
        }

        //Create a user profile and set the username todo: set user profile image dynamically
        UserProfile profile = new UserProfile{DisplayName = _username, PhotoUrl = new Uri("https://firebasestorage.googleapis.com/v0/b/geosnapv1.appspot.com/o/ProfilePhotos%2FEmptyPhoto.jpg?alt=media&token=fbc8b18c-4bdf-44fd-a4ba-7ae881d3f063")};
        var ProfileTask = fbUser.UpdateUserProfileAsync(profile);
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
            Debug.LogWarning("Username Set Failed!");
            callback("Something Went Wrong, Sorry");
            yield break;
        }

        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogWarning("User Null");
            yield break;
        }

        Firebase.Auth.UserProfile userProfile = new Firebase.Auth.UserProfile
        {
            DisplayName = user.DisplayName,
        };
       
        user.UpdateUserProfileAsync(userProfile).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("UpdateUserProfileAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
            }
        });

        var json = GenerateUserProfileJson( _username, "null", "null",_email, _phoneNumber);
        DBref.Child("users").Child(fbUser.UserId.ToString()).SetRawJsonValueAsync(json);
        
        var DBTaskSetUsernameLinkToId = DBref.Child("usernames").Child(_username).SetValueAsync(fbUser.UserId);
        yield return new WaitUntil(predicate: () => DBTaskSetUsernameLinkToId.IsCompleted);
        
        var DBTaskSetPhoneNumberLinkToId = DBref.Child("phoneNumbers").Child(fbUser.UserId).Child(_phoneNumber).SetValueAsync(fbUser.UserId);
        yield return new WaitUntil(predicate: () => DBTaskSetPhoneNumberLinkToId.IsCompleted);

        var DBTaskSetUserFriends = DBref.Child("friendRequests").Child(fbUser.UserId).Child("null").SetValueAsync("null");
        yield return new WaitUntil(predicate: () => DBTaskSetUserFriends.IsCompleted);
        
        //if nothing has gone wrong try logging in with new users information
        StartCoroutine(Login(_email, _password, (myReturnValue) => {
            if (myReturnValue != null)
            {
                Debug.LogWarning("failed to login");
            }
            else
            {
                Debug.Log("Logged In!");
            }
        }));
        callback(null);
    }
    #endregion
    
    #region Edit This Users Information
    public IEnumerator SetUsername(string _username, System.Action<String> callback)
    {
        Debug.Log("Db SetUsername to :" + _username);
        var DBTask = DBref.Child("users").Child(fbUser.UserId).Child("Username").SetValueAsync(_username);
        
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            callback("successfully updated _username");
        }
    }
    public IEnumerator SetUserProfilePhotoUrl(string _photoUrl, System.Action<String> callback)
    {
        Debug.Log("Db update photoUrl to :" + _photoUrl);
        //Set the currently logged in user nickName in the database
        var DBTask = DBref.Child("users").Child(fbUser.UserId).Child("userPhotoUrl").SetValueAsync(_photoUrl);
        
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            callback("success");
        }
    }
    public IEnumerator SetUserNickName(string _nickName, System.Action<String> callback)
    {
        Debug.Log("Db update nick to :" + _nickName);
        //Set the currently logged in user nickName in the database
        var DBTask = DBref.Child("users").Child(fbUser.UserId).Child("NickName").SetValueAsync(_nickName);
        
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            callback("successfully updated nickName");
        }
    }
    public IEnumerator SetUserPhoneNumber(string _phoneNumber, System.Action<String> callback)
    {
        var DBTask = DBref.Child("users").Child(fbUser.UserId).Child("phoneNumber").SetValueAsync(_phoneNumber);
        
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            callback("success");
        }
    }
    #endregion

    #region Get This Users Components
    public IEnumerator GetMyUserProfilePhoto(System.Action<Texture> callback)
    {
        var request = new UnityWebRequest();
        var url = "";
        
        Debug.Log("test:");
        StorageReference pathReference = storage.GetReference("ProfilePhoto/"+fbUser.UserId+"/profile.png");
        Debug.Log("path refrence:" + pathReference);

        pathReference.GetDownloadUrlAsync().ContinueWithOnMainThread(task => {
            if (!task.IsFaulted && !task.IsCanceled) {
                Debug.Log("Download URL: " + task.Result);
                url = task.Result + "";
                Debug.Log("Actual  URL: " + url);
            }
            else
            {
                Debug.Log("task failed:" + task.Result);
            }
        });
        
        yield return new WaitForSecondsRealtime(1f); //hmm not sure why (needs to wait for GetDownloadUrlAsync to complete)
        
        request = UnityWebRequestTexture.GetTexture((url)+"");
        
        yield return request.SendWebRequest(); //Wait for the request to complete
        
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("error:" + request.error);
        }
        else
        {
            callback(((DownloadHandlerTexture)request.downloadHandler).texture);
        }
    }
    public IEnumerator GetMyUserNickName(System.Action<String> callback)
    {
        var DBTask = DBref.Child("users").Child(fbUser.UserId).Child("Friends").Child("nickName").GetValueAsync();
        
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        
        if (DBTask.IsFaulted)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            callback(DBTask.Result.ToString());
        }
    }
    public IEnumerator GetMyUserPhoneNumber(System.Action<String> callback)
    {
        var DBTask = DBref.Child("users").Child(fbUser.UserId).Child("Friends").Child("nickName").GetValueAsync();
        
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        
        if (DBTask.IsFaulted)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            callback(DBTask.Result.ToString());
        }
    }
    #endregion
    
    #endregion

    #region Other User
    
    #region Query Database For User
    public IEnumerator SearchForUserIDByUsername(String username, System.Action<CallbackObject> callback)
    {
        CallbackObject callbackObject = new CallbackObject();
        
        Debug.Log("DB searching for username:" + username);
        var DBTask = DBref.Child("usernames").Child(username).GetValueAsync();
        
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            callback(callbackObject);
        }
        else
        {
            callbackObject.ReturnValue = DBTask.Result.Value.ToString();
            callbackObject.IsSuccessful = true;
            callback(callbackObject);
        }
    }
    public IEnumerator SearchForUserByUsername(String username, System.Action<CallbackObject> callback)
    {
        CallbackObject callbackObject = new CallbackObject();
        
        Debug.Log("DB searching for user by username:" + username);
        string userID = "";
        yield return StartCoroutine(SearchForUserIDByUsername(username,(myReturnValue) =>
        {
            if (myReturnValue.IsSuccessful)
            {
                userID = myReturnValue.ReturnValue.ToString();
                Debug.Log("found user id");
            }
            else
            {
                Debug.LogWarning("failed to find user photo");
            }
        }));
        
        
        Debug.Log("getting user photo...");
        yield return StartCoroutine(GetUserProfilePhotoByUserID(userID,(myReturnValue) =>
        {
            if (myReturnValue.IsSuccessful)
            {
                Debug.Log("found userr photo");
                callbackObject.ReturnValue = myReturnValue.ReturnValue;
                callbackObject.message = userID;
                callbackObject.IsSuccessful = true;
                callback(callbackObject);
            }
            else
            {
                Debug.LogWarning("failed to find user photo");
            }
        }));
    }
    #endregion

    #region Query Database for Other Users Components
    public IEnumerator GetUserProfilePhotoByUrl(string _url, System.Action<CallbackObject> callback)
    {
        CallbackObject callbackObject = new CallbackObject();
        var request = new UnityWebRequest();
        var url = "";
        
        Debug.Log("test:");
        StorageReference pathReference = storage.GetReference(_url);
        Debug.Log("path refrence:" + pathReference);

        pathReference.GetDownloadUrlAsync().ContinueWithOnMainThread(task => {
            if (!task.IsFaulted && !task.IsCanceled) {
                Debug.Log("Download URL: " + task.Result);
                url = task.Result + "";
                Debug.Log("Actual  URL: " + url);
            }
            else
            {
                Debug.Log("task failed:" + task.Result);
            }
        });
        
        yield return new WaitForSecondsRealtime(0.5f); //hmm not sure why (needs to wait for GetDownloadUrlAsync to complete)
        
        request = UnityWebRequestTexture.GetTexture((url)+"");
        
        yield return request.SendWebRequest(); //Wait for the request to complete
        
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("error:" + request.error);
        }
        else
        {
            callbackObject.IsSuccessful = true;
            callbackObject.ReturnValue = ((DownloadHandlerTexture)request.downloadHandler).texture;
            callback(callbackObject);
        }
    }
    public IEnumerator GetUserProfilePhotoByUserID(String userID, System.Action<CallbackObject> callback)
    {
        CallbackObject callbackObject = new CallbackObject();
        
        Debug.Log("DB searching for user photo:" + userID);
        var DBTask = DBref.Child("users").Child(userID).Child("userPhotoUrl").GetValueAsync();
        
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            callback(callbackObject);
        }
        else
        {
            Debug.Log("found user profile photo url");
            Debug.Log("searching for user photo in storage");
            //get the photo from storage using the user photo url
            yield return StartCoroutine(GetUserProfilePhotoByUrl(DBTask.Result.Value.ToString(),(myReturnValue) =>
            {
                if (myReturnValue.IsSuccessful)
                {
                    Debug.Log("found user photo in storage");
                    callbackObject.ReturnValue = myReturnValue.ReturnValue;
                    callbackObject.IsSuccessful = true;
                    callback(callbackObject);
                }
                else
                {
                    Debug.LogWarning("failed to find user photo");
                }
            }));
        }
    }
    #endregion
    #endregion
    
    #region Friend Requests
    public IEnumerator MakeFriendshipRequest(string _userID, System.Action<CallbackObject> callback)
    {
        CallbackObject callbackObject = new CallbackObject();
        
        Debug.Log("Db making friendship reuest to:" + _userID);
        
        string key = DBref.Child("friendRequests").Child(fbUser.UserId).Push().Key;
        Dictionary<string, Object> childUpdates = new Dictionary<string, Object>();
        childUpdates["/friendRequests/" + fbUser.UserId + "/" + key] = _userID;
        DBref.UpdateChildrenAsync(childUpdates);

        yield return new WaitForSeconds(0.1f);
        
        callbackObject.IsSuccessful = true;
        callbackObject.message = "";
        callback(callbackObject);
    }
    
    public List<string> GetFriendShipRequests()
    {
        List<string> listOfFriends = new List<string>();
        FirebaseDatabase.DefaultInstance.GetReference("friendRequests").Child(fbUser.UserId).GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsFaulted)
                {
                    return;
                }
                else if (task.IsCompleted) {
                    DataSnapshot snapshot = task.Result;
                    foreach (var child in snapshot.Children)
                    {
                        Debug.Log("data snapshot of friends child value:"+child.Value);
                        listOfFriends.Add(child.Value.ToString());
                    }
                }
        });
        return listOfFriends;
    }
    public void SubscribeToFriendShipRequests()
    {
        var refrence = FirebaseDatabase.DefaultInstance.GetReference("friendRequests").Child(fbUser.UserId);
        refrence.ChildAdded += HandleChildAdded;
        refrence.ChildChanged += HandleChildChanged;
        refrence.ChildRemoved += HandleChildRemoved;
        refrence.ChildMoved += HandleChildMoved;

        void HandleChildAdded(object sender, ChildChangedEventArgs args) {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            // Do something with the data in args.Snapshot
            Debug.Log("child added:" +args.Snapshot);
            Debug.Log("value:" +  args.Snapshot.GetRawJsonValue());
        }

        void HandleChildChanged(object sender, ChildChangedEventArgs args) {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            // Do something with the data in args.Snapshot
            Debug.Log("child changed:" +args.Snapshot);
            Debug.Log("value:" +  args.Snapshot.GetRawJsonValue());
        }

        void HandleChildRemoved(object sender, ChildChangedEventArgs args) {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            // Do something with the data in args.Snapshot
            Debug.Log("child removed:" +args.Snapshot);
            Debug.Log("value:" +  args.Snapshot.GetRawJsonValue());
        }

        void HandleChildMoved(object sender, ChildChangedEventArgs args) {
            if (args.DatabaseError != null) {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            // Do something with the data in args.Snapshot
            Debug.Log("child moved:" +args.Snapshot);
            Debug.Log("value:" +  args.Snapshot.GetRawJsonValue());
        }
    }


    //Future Functions
    //GetFriendshipRequests
    //AcceptFriendshipRequest
    //getPhotos
    
    
    //TESTING
    public void AddFriend(String _username)
    {
        String _nickName = "null";
        StartCoroutine(FbManager.instance.AcceptFriend(_username, _nickName, (myReturnValue) => {
            if (myReturnValue != "Success")
            {
                Debug.LogError("failed to update freinds");
            }
            else
            {
                Debug.Log("updated friends");
            }
        }));
    }
    private IEnumerator AcceptFriend(string _username, string _nickName, System.Action<String> callback)
    {
        var DBTask = DBref.Child("users").Child(fbUser.UserId).Child("Friends").Child(_username).SetValueAsync(_nickName);
        
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        
        if (DBTask.IsFaulted)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            callback("Success");
        }
    }
    
    #endregion
    
    
    
    public void getTestImage()
    {
        StartCoroutine(GetTestImage((myReturnValue) => {
            if (myReturnValue != null)
            {
                testRawImage.texture = myReturnValue;
            }
        }));
    }
    private IEnumerator GetTestImage(System.Action<Texture> callback)
    {
        var request = new UnityWebRequest();
        
        request = UnityWebRequestTexture.GetTexture("https://firebasestorage.googleapis.com/v0/b/geosnapv1.appspot.com/o/ProfilePhoto%2FPVNKPFYFrWVRoPdhsTs0aAYH5cA3%2Fprofile.png?alt=media&token=894e50e6-7a46-4dec-aca7-20945d1bca58"); //Create a request

        yield return request.SendWebRequest(); //Wait for the request to complete
        
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("error:" + request.error);
        }
        else
        {
            callback(((DownloadHandlerTexture)request.downloadHandler).texture);
        }
    }
    //

    private void DeleteFile(String _location) 
    { 
        storageRef = storageRef.Child(_location);
        storageRef.DeleteAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted) {
                Debug.Log("File deleted successfully.");
            }
            else {
                // Uh-oh, an error occurred!
            }
        });
    }

    //get firebase storage database
    public void DownloadImage() {
        StartCoroutine(GetMyUserProfilePhoto((myReturnValue) => {
            if (myReturnValue != null)
            {
                rawImage.texture = myReturnValue;
            }
        }));
    }
    
    private IEnumerator TryLoadImage(string MediaUrl, System.Action<Texture> callback) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://firebasestorage.googleapis.com/v0/b/geosnapv1.appspot.com/o/"+ fbUser.UserId +"%2FnewFile.jpeg?alt=media"); //Create a request

        yield return request.SendWebRequest(); //Wait for the request to complete
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogWarning(request.error);
        }
        else
        {
            callback(((DownloadHandlerTexture)request.downloadHandler).texture);
        }
    }
}
