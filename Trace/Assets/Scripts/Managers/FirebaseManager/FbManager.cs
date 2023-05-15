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
using DownloadHandler = Networking.DownloadHandler;
using Object = System.Object;


public partial class FbManager : MonoBehaviour
{
    [Header("Dont Destroy")]
    public static FbManager instance;
    
    [Header("Firebase References")]
    [SerializeField] private DependencyStatus dependencyStatus;
    [SerializeField] private String firebaseStorageReferenceUrl;
    [SerializeField] private FirebaseAuth _firebaseAuth;    
    [SerializeField] private FirebaseUser _firebaseUser;
    [SerializeField] private DatabaseReference _databaseReference;
    [SerializeField] private FirebaseStorage _firebaseStorage;
    [SerializeField] private StorageReference _firebaseStorageReference;

    [Header("Login Settings")] 
    [SerializeField] private bool autoLogin;
    [SerializeField] private bool forceLogin;
    [SerializeField] private bool useAdminForLogin;
    [SerializeField] private string adminUser;
    [SerializeField] private string adminPass;
    [SerializeField] private bool resetPlayerPrefs;

    [Header("User Data")] 
    public Texture userImageTexture;
    public bool firstTimeUsingTrace;
    public string userId;
    public List<TraceObject> receivedTraces;
    public List<TraceObject> sentTraces;

    
    [Header("Essential Properties")] 
    [SerializeField] private float _timeToRepeatForCheckingRequest =   2f;
    public UserModel thisUserModel;

    [Header("Refrences")] 
    [SerializeField] private TraceManager traceManager;

    [Header("Testing")] 
    [SerializeField] private Texture test;
    public bool IsFirebaseUserInitialised
    {
        get;
        private set;
    }
    public List<UserModel> AllUsers
    {
        get { return users; }
    }

    private List<UserModel> users;
    void Awake()
    {
        if (resetPlayerPrefs)
        {
            PlayerPrefs.DeleteAll();
        }

        IsFirebaseUserInitialised = false;
        
        PlayerPrefs.SetInt("NumberOfTimesLoggedIn", PlayerPrefs.GetInt("NumberOfTimesLoggedIn")+1);
        if (PlayerPrefs.GetInt("NumberOfTimesLoggedIn") == 1)
        {
            Debug.Log("FbManager: First Time Logging In!");
        }
        
        //makes sure nothing can use the db until its enabled
        dependencyStatus = DependencyStatus.UnavailableUpdating;
        
        if (instance != null)
        {Destroy(gameObject);}
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        _firebaseStorage = FirebaseStorage.DefaultInstance;
        _firebaseStorageReference = _firebaseStorage.GetReferenceFromUrl(firebaseStorageReferenceUrl);
        
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
        Debug.Log("initializing firebase");
        _firebaseAuth = FirebaseAuth.DefaultInstance;
        _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        _previousRequestFrom = new List<string>();
        _allReceivedRequests = new List<FriendRequests>();
        _allSentRequests = new List<FriendRequests>();
        _allFriends = new List<FriendModel>();
        _allFriendRequests = new Dictionary<string, eFriendRequestType>();
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

    #region Current User
    #region -User Login/Logout
    public IEnumerator ForceLogin()
    {
        //Todo: figure out which wait until to use...
        Debug.Log("Overiding Logging in");
        yield return new WaitForSeconds(0.5f); //has to wait until firebase async task is finished... (is there something better?)
        Debug.Log("Logging 0.5f");
        ScreenManager.instance.ChangeScreenFade("HomeScreen");
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
        if (savedUsername != "null" || savedPassword != "null")
        {
            Debug.Log("auto logging in");
            StartCoroutine(FbManager.instance.Login(savedUsername, savedPassword, (myReturnValue) => {
                if (myReturnValue.IsSuccessful)
                {
                    Debug.Log("FbManager: Logged in!");
                    ScreenManager.instance.ChangeScreenFade("HomeScreen");
                    userId = _firebaseUser.UserId;
                }
                else
                {
                    Debug.LogError("FbManager: failed to auto login");
                    LogOut();
                }
            }));
        }
        else
        {
            Debug.Log("pulling up login options");
            ScreenManager.instance.WelcomeScreen();
        }
    }
    public IEnumerator Login(string _email, string _password,  System.Action<CallbackObject> callback)
    {
        Debug.Log("Login Started");
        CallbackObject callbackObject = new CallbackObject();
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = _firebaseAuth.SignInWithEmailAndPasswordAsync(_email, _password);
        
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

        _firebaseUser = LoginTask.Result;
        Debug.LogFormat("User signed in successfully: {0} ({1})", _firebaseUser.DisplayName, _firebaseUser.Email);
        Debug.Log("logged In: user profile photo is: " + _firebaseUser.PhotoUrl);
        callbackObject.IsSuccessful = true;
        //Load User Profile Texture
        /*StartCoroutine(FbManager.instance.GetMyUserProfilePhoto((myReturnValue) => {
            if (myReturnValue != null)
            {
                userImageTexture = myReturnValue;
            }
        }));*/
        
        //all database things that need to be activated
        var DBTaskSetIsOnline = _databaseReference.Child("users").Child(_firebaseUser.UserId).Child("isOnline").SetValueAsync(true);
        yield return new WaitUntil(predicate: () => DBTaskSetIsOnline.IsCompleted);
        
        //stay logged in
        PlayerPrefs.SetString("Username", _email);
        PlayerPrefs.SetString("Password", _password);
        PlayerPrefs.Save();
        callback(callbackObject);
        if (callbackObject.IsSuccessful == false)
        {
            StartCoroutine(SetUserLoginStatus(true, isSusscess =>
            {
                if (isSusscess)
                {
                    print("Done !");
                }
            }));
        }
        //once user is logged in
        GetAllUserNames();
        GetCurrentUserData(_password);
         // StartCoroutine(RetrieveFriendRequests());
        // StartCoroutine(RetrieveSentFriendRequests());
        GetAllFriendRequestsFromDatabase();
        StartCoroutine(RetrieveFriends());
        ContinuesListners();
        InitializeFCMService();
    }

    private void ContinuesListners()
    {
         // StartCoroutine(CheckForFriendRequest());
         // StartCoroutine(CheckIfFriendRequestRemoved());
        StartCoroutine(CheckForNewFriends());
        StartCoroutine(CheckIfFriendRemoved());
        AllRequestListners();
    }

    private void GetCurrentUserData(string password)
    {
       
        // Get a reference to the "users" node in the database
        DatabaseReference usersRef = _databaseReference.Child("users");
        
        // Attach a listener to the "users" node
        usersRef.Child(_firebaseUser.UserId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                // Iterate through the children of the "users" node and add each username to the list
                DataSnapshot snapshot = task.Result;
                    string email = snapshot.Child("email").Value.ToString();
                    string frindCount = snapshot.Child("friendCount").Value.ToString();
                    string displayName = snapshot.Child("name").Value.ToString();
                    string username = snapshot.Child("username").Value.ToString();
                    string phoneNumber = snapshot.Child("phone").Value.ToString();
                    string photoURL = snapshot.Child("userPhotoUrl").Value.ToString();
                    thisUserModel = new UserModel(_firebaseUser.UserId,email,int.Parse(frindCount),displayName,username,phoneNumber,photoURL, password);

                    IsFirebaseUserInitialised = true;
            }
            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception);
                // Handle the error
            }
        });
    }
    
    public void LogOut()
    {
        Debug.Log("FBManager: logging out");
        PlayerPrefs.SetString("Username", "null");
        PlayerPrefs.SetString("Password", "null");
        userImageTexture = null;
        _firebaseAuth.SignOut();
        receivedTraces.Clear();
        StartCoroutine(SetUserLoginStatus(false, isSusscess =>
        {
            if (isSusscess)
                print("Updated Login Status");
            
            ScreenManager.instance.ChangeScreenForwards("Welcome");
        }));
    }
    #endregion
    #region -User Registration
    private string GenerateUserProfileJson(string username, string name, string userPhotoLink, string email, string phone) {
        TraceUserInfoStructure traceUserInfoStructure = new TraceUserInfoStructure(username, name, userPhotoLink, email, phone);
        string json = JsonUtility.ToJson(traceUserInfoStructure);
        return json;
    }
    public IEnumerator RegisterNewUser(string _email, string _password, string _username, string _phoneNumber,  System.Action<String,AuthError> callback)
    {
        if (_username == "")
        {
            callback("Missing Username", AuthError.None); //having a blank nickname is not really a DB error so I return a error here
            yield break;
        }
        Task<FirebaseUser> RegisterTask  =null;
        string message = "";
        AuthError errorCode =  AuthError.None;
        var creationTask =  _firebaseAuth.CreateUserWithEmailAndPasswordAsync(_email, _password).ContinueWith(task =>
        {
            RegisterTask = task;
            
            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");        
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                errorCode = (AuthError)firebaseEx.ErrorCode;
                Debug.LogError("Error Code :: " + errorCode);
                message = "Register Failed!";
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
            }
           
            // Firebase user has been created.
            _firebaseUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                _firebaseUser.DisplayName, _firebaseUser.UserId);
        });


        while (!creationTask.IsCompleted)
            yield return new WaitForEndOfFrame();

        if (RegisterTask.Exception != null)
        {
            callback(message,errorCode);
            yield break; 
        }
        
        if (_firebaseUser == null)
        {
            Debug.LogError("User Null");
            yield break;
        }
        else
        {
            print("User Email :: "+_firebaseUser.Email);
        }
        
        var json = GenerateUserProfileJson( _username, "null", "null",_email, _phoneNumber);
        _databaseReference.Child("users").Child(_firebaseUser.UserId.ToString()).SetRawJsonValueAsync(json);
       
        var DBTaskSetUserFriends = _databaseReference.Child("Friends").Child(_firebaseUser.UserId).Child("Don't Delete This Child").SetValueAsync("*");
        while (DBTaskSetUserFriends.IsCompleted is false)
            yield return new WaitForEndOfFrame();
        
        
        var DBTaskSetUserFriendRequests = _databaseReference.Child("FriendRequests").Child(_firebaseUser.UserId).Child("Sent").Child("Don't Delete This Child").SetValueAsync(true);
        DBTaskSetUserFriendRequests = _databaseReference.Child("FriendRequests").Child(_firebaseUser.UserId).Child("Received").Child("Don't Delete This Child").SetValueAsync(true);

        while (DBTaskSetUserFriends.IsCompleted is false)
            yield return new WaitForEndOfFrame();
        
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
        callback(null, errorCode);
    }

    #endregion
    #region -User Edit Information
    public IEnumerator SetUsername(string _username, System.Action<bool> callback)
    {
        Debug.Log("Db SetUsername to :" + _username);
        var DBTask = _databaseReference.Child("users").Child(_firebaseUser.UserId).Child("username").SetValueAsync(_username);
        
        // yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        while (DBTask.IsCompleted is false)
            yield return new WaitForEndOfFrame();
        
        
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            callback(false);
        }
        else
        {
            callback(true);
        }
    }
    public IEnumerator SetUserProfilePhotoUrl(string _photoUrl, System.Action<bool> callback)
    {
        Debug.Log("Db update photoUrl to :" + _photoUrl);
        //Set the currently logged in user nickName in the database
        var DBTask = _databaseReference.Child("users").Child(_firebaseUser.UserId).Child("userPhotoUrl").SetValueAsync(_photoUrl);
        
        while (DBTask.IsCompleted is false)
            yield return new WaitForEndOfFrame();

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            callback(false);
        }
        else
        {
            GetCurrentUserData("**********");
            callback(true);
        }
    }
    public IEnumerator SetUserNickName(string _nickName, System.Action<bool> callback)
    {
        Debug.Log("Db update nick to :" + _nickName);
        //Set the currently logged in user nickName in the database
        var DBTask = _databaseReference.Child("users").Child(_firebaseUser.UserId).Child("name").SetValueAsync(_nickName);
        
        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        while (DBTask.IsCompleted is false)
            yield return new WaitForEndOfFrame();
        
        if (DBTask.Exception != null)
        {
            callback(false);
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            callback(true);
        }
    }
    public IEnumerator SetUserPhoneNumber(string _phoneNumber, System.Action<bool> callback)
    {
        Debug.LogError("Is Database Reference is Null  ? "+ _databaseReference == null);
        var DBTask = _databaseReference.Child("users").Child(_firebaseUser.UserId).Child("phoneNumber").SetValueAsync(_phoneNumber);

        Debug.LogError("Is Database Completion is Null  ? "+ DBTask == null);

        while (DBTask.IsCompleted is false)
            yield return new WaitForEndOfFrame();
        
        // yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            callback(false);
        }
        else
        {
            callback(true);
        }
    }
    
    public IEnumerator SetUserLoginStatus(bool _isLoggedIn, System.Action<bool> callback)
    {
        if (_firebaseUser != null)
        {
            var DBTask = _databaseReference.Child("users").Child(_firebaseUser.UserId).Child("isLogedIn")
                .SetValueAsync(_isLoggedIn);
            while (DBTask.IsCompleted is false)
                yield return new WaitForEndOfFrame();

            // yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
                callback(false);
            }
            else
            {
                callback(true);
            }
        }
        else
            callback(false);
    }


    public IEnumerator UploadProfilePhoto(byte[] _picBytes, System.Action<bool,string> callback)
    {
        StorageReference imageRef = _firebaseStorage.GetReference("ProfilePhoto/"+_firebaseUser.UserId+".png");

        var task = imageRef.PutBytesAsync(_picBytes);

        while (task.IsCompleted is false)
            yield return new WaitForEndOfFrame();
        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.LogError("Task Faulted Due To :: "+ task.Exception.ToString());
            callback(false,"");
        }
        else
        {
            Debug.LogError("Image Uploaded Successfully");
            Debug.Log("Download URL: " + task.Result);
            var url = task.Result.Path + "";
            Debug.Log("Actual  URL: " + url);
            callback(true,url);
        }
    }
    #endregion
    #region -User Info
    public IEnumerator GetProfilePhotoFromFirebaseStorageRoutine(string userId, System.Action<Texture> callback)
    {
        // var request = new UnityWebRequest();
        var url = "";
        
        Debug.Log("test:");
        StorageReference pathReference = _firebaseStorage.GetReference("ProfilePhoto/"+userId+".png");
        Debug.Log("path refrence:" + pathReference);

        var task = pathReference.GetDownloadUrlAsync();

        while (task.IsCompleted is false)
            yield return new WaitForEndOfFrame();

        if (!task.IsFaulted && !task.IsCanceled) {
            Debug.Log("Download URL: " + task.Result);
            url = task.Result + "";
            Debug.Log("Actual  URL: " + url);
        }
        else
        {
            try
            {
                Debug.Log("task failed:" + task.Result);
            }
            catch (Exception e)
            {
                
            }
        
    }

        DownloadHandler.Instance.DownloadImage(url, callback, () =>
        {
            callback(null);
        });
    }
    public IEnumerator GetMyUserNickName(System.Action<String> callback)
    {
        var DBTask = _databaseReference.Child("users").Child(_firebaseUser.UserId).Child("Friends").Child("nickName").GetValueAsync();
        
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
        var DBTask = _databaseReference.Child("users").Child(_firebaseUser.UserId).Child("Friends").Child("nickName").GetValueAsync();
        
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

    private void GetAllUserNames()
    {
        // Create a list to store the usernames
        users = new List<UserModel>();
        
        // Get a reference to the "users" node in the database
        DatabaseReference usersRef = _databaseReference.Child("users");
        
        // Attach a listener to the "users" node
        usersRef.GetValueAsync().ContinueWith(task =>
        {
             if (task.IsCompleted)
             {
                 // Iterate through the children of the "users" node and add each username to the list
                 DataSnapshot snapshot = task.Result;
                 var  allUsersSnapshots = snapshot.Children.ToArrayPooled();
                 for (var userIndex = 0; userIndex < allUsersSnapshots.Length; userIndex++)
                 {
                     string userId = allUsersSnapshots[userIndex].Key; 
                     string email = allUsersSnapshots[userIndex].Child("email").Value.ToString();
                     string frindCount = allUsersSnapshots[userIndex].Child("friendCount").Value.ToString();
                     string displayName = allUsersSnapshots[userIndex].Child("name").Value.ToString();
                     string username = allUsersSnapshots[userIndex].Child("username").Value.ToString();
                     string phoneNumber = allUsersSnapshots[userIndex].Child("phone").Value.ToString();
                     string photoURL = allUsersSnapshots[userIndex].Child("userPhotoUrl").Value.ToString();
                     bool isLoggedin =  Convert.ToBoolean( allUsersSnapshots[userIndex].Child("isLogedIn").Value.ToString());
                     UserModel userData = new UserModel(userId,email,int.Parse(frindCount),displayName,username,phoneNumber,photoURL,"",isLoggedin );
                     users.Add(userData);
                 }
             }
             if (task.IsFaulted)
             {
                 Debug.LogError(task.Exception);
                 // Handle the error
             }
        });
    }

    public List<string> GetMyFriendShipRequests()
    {
        List<string> listOfFriends = new List<string>();
        FirebaseDatabase.DefaultInstance.GetReference("friendRequests").Child(_firebaseUser.UserId).GetValueAsync().ContinueWithOnMainThread(task => {
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
    } //not sure if this works
    #endregion
    #region -User Actions
    public IEnumerator ActionFriendRequest(string _userID, System.Action<CallbackObject> callback)
    {
        CallbackObject callbackObject = new CallbackObject();
        
        Debug.Log("Db making friendship reuest to:" + _userID);
        Debug.Log("Signed In User ID "+_firebaseUser.UserId);
        
        string key = _databaseReference.Child("friendRequests").Child(_firebaseUser.UserId).Push().Key;
        Dictionary<string, Object> childUpdates = new Dictionary<string, Object>();
        childUpdates["/friendRequests/" + _firebaseUser.UserId + "/" + key] = _userID;
        _databaseReference.UpdateChildrenAsync(childUpdates);

        yield return new WaitForSeconds(0.1f);
        
        callbackObject.IsSuccessful = true;
        callbackObject.message = "";
        callback(callbackObject);
    }
    private IEnumerator ActionAcceptFriend(string _username, string _nickName, System.Action<String> callback)
    {
        var DBTask = _databaseReference.Child("users").Child(_firebaseUser.UserId).Child("Friends").Child(_username).SetValueAsync(_nickName);
        
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
    #region -User Subscriptions
    public void SubscribeToFriendShipRequests()
    {
        var refrence = FirebaseDatabase.DefaultInstance.GetReference("friendRequests").Child(_firebaseUser.UserId);
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
    #endregion
    #endregion
    #region Other User
    #region -Search for User
    public IEnumerator SearchForUserIDByUsername(String username, System.Action<CallbackObject> callback)
    {
        CallbackObject callbackObject = new CallbackObject();
        
        Debug.Log("DB searching for username:" + username);
        var DBTask = _databaseReference.Child("usernames").Child(username).GetValueAsync();
        
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
    #region -Get User Info
    public IEnumerator GetUserProfilePhotoByUrl(string _url, System.Action<CallbackObject> callback)
    {
        CallbackObject callbackObject = new CallbackObject();
        var request = new UnityWebRequest();
        var url = "";
        
        Debug.Log("test:");
        StorageReference pathReference = _firebaseStorage.GetReference(_url);
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
    
    // TODO: Redundant function
    public IEnumerator GetUserProfilePhotoByUserID(String userID, System.Action<CallbackObject> callback)
    {
        CallbackObject callbackObject = new CallbackObject();
        
        Debug.Log("DB searching for user photo:" + userID);
        var DBTask = _databaseReference.Child("users").Child(userID).Child("userPhotoUrl").GetValueAsync();
        
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
    
    //TESTING
    //Future Functions
    //GetFriendshipRequests
    //AcceptFriendshipRequest
    //getPhotos
    
    public void AddFriend(String _username)
    {
        String _nickName = "null";
        StartCoroutine(FbManager.instance.ActionAcceptFriend(_username, _nickName, (myReturnValue) => {
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
    public void getTestImage()
    {
        StartCoroutine(GetTestImage((myReturnValue) => {
            if (myReturnValue != null)
            {
                // testRawImage.texture = myReturnValue;
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
    private void DeleteFile(String _location) 
    { 
        _firebaseStorageReference = _firebaseStorageReference.Child(_location);
        _firebaseStorageReference.DeleteAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted) {
                Debug.Log("File deleted successfully.");
            }
            else {
                // Uh-oh, an error occurred!
            }
        });
    }
    public void GetProfilePhotoFromFirebaseStorage(string userId, Action<Texture> onSuccess, Action<string> onFailed) {
        StartCoroutine(GetProfilePhotoFromFirebaseStorageRoutine(userId, (myReturnValue) => {
            if (myReturnValue != null)
            {
                onSuccess?.Invoke(myReturnValue);
            }

            {
                onFailed?.Invoke("Image not Found");
            }
        }));
    }
    private IEnumerator TryLoadImage(string MediaUrl, System.Action<Texture> callback) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://firebasestorage.googleapis.com/v0/b/geosnapv1.appspot.com/o/"+ _firebaseUser.UserId +"%2FnewFile.jpeg?alt=media"); //Create a request

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
