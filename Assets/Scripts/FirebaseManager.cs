using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class FirebaseManager : MonoBehaviour
{
    public Program program;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
    DatabaseReference reference;

    bool req_bookmark_f = true;

    public string GetBookmark()
    {
        Debug.Log("GetBookmark");
        if (user != null)
        {
            if (reference != null)
            {
                reference.Child("Bookmark").Child(user.UserId).GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {

                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot data = task.Result;
                        Debug.Log("GetBookmark: " + data.GetValue(false).ToString());
                        program.LoadBookmarkFromJsonString(data.GetValue(false).ToString());
                        program.ReqUpdateUI();
                    }
                });
            }
            else
            {
                Debug.Log("Reference is null");
            }
        }
        return "";
    }

    public void SetBookmark(string bookmark)
    {
        if (user != null)
        {
            //Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            //{
            //    DisplayName = bookmark
            //};
            //user.UpdateUserProfileAsync(profile).ContinueWith(task => {
            //    if (task.IsCanceled)
            //    {
            //        Debug.LogError("UpdateUserProfileAsync was canceled.");
            //        return;
            //    }
            //    if (task.IsFaulted)
            //    {
            //        Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
            //        return;
            //    }

            //    Debug.Log("User profile updated successfully.");
            //});
            if (reference != null)
            {
                reference.Child("Bookmark").Child(user.UserId).SetValueAsync(bookmark);
                Debug.Log("SetBookmark to " + user.UserId);
            }
            else
            {
                Debug.Log("Reference is null");
            }
        }
    }

    public void LoginByEmail(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    public void JoinByEmail(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    public void Logout()
    {
        auth.SignOut();
    }
    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("FirebaseManager:Start");

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        InitializeFirebase();

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                var app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                Debug.Log(app.Options.ApiKey);

                // Set up the Editor before calling into the realtime database.
                FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://tapbible.firebaseio.com/");

                // Get the root reference location of the database.
                reference = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (req_bookmark_f)
        {
            if (user != null)
            {
                if (reference != null)
                {
                    req_bookmark_f = false;
                    GetBookmark();
                }
            }
        }
    }

    private void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
//        Firebase.Auth.FirebaseAuth.DefaultInstance.App.Dispose();
    }

    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Debug.Log("AuthStateChange");
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                //displayName = user.DisplayName ?? "";
                //emailAddress = user.Email ?? "";
                //photoUrl = user.PhotoUrl ?? "";
            }
        }
    }
}
