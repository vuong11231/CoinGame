//using System.Collections.Generic;
//using UnityEngine;
//using Amazon.S3;
//using Amazon.S3.Model;
//using Amazon.Runtime;
//using Amazon.CognitoIdentity;
//using Amazon;
//using System;
//using System.IO;
//using Newtonsoft.Json;

//public class AwsS3Manager : Singleton<AwsS3Manager>
//{

//    // *********************** Properties ***********************
//    //
//    //

//    [Header("AWS S3 Bucket")]
//    public string           BUCKET_NAME                 = "BUCKET_NAME";
//    public RegionEndpoint   BUCKET_REGION               = RegionEndpoint.USEast1;

//    [Header("AWS S3 Cognito Identity")]
//    public string           COGNITO_IDENTITY_POOL_ID    = "COGNITO_IDENTITY_POOL_ID";
//    public RegionEndpoint   COGNITO_REGION              = RegionEndpoint.USEast1;

//    [Header("Debug")]
//    public bool IsShowDebug = false;

//    bool _IsInit = false;
//    object _DataModel;

//    // *********************** Get - Set ***********************
//    //
//    //

//    RegionEndpoint CognitoIdentityRegion
//    {
//        get { return RegionEndpoint.GetBySystemName(COGNITO_REGION.SystemName); }
//    }

//    RegionEndpoint BucketRegion
//    {
//        get { return RegionEndpoint.GetBySystemName(BUCKET_REGION.SystemName); }
//    }


//    AWSCredentials _Credentials;
//    private AWSCredentials Credentials
//    {
//        get
//        {
//            if (_Credentials == null)
//                _Credentials = new CognitoAWSCredentials(COGNITO_IDENTITY_POOL_ID, COGNITO_REGION);
//            return _Credentials;
//        }
//    }

//    IAmazonS3 _s3Client;
//    private IAmazonS3 Client
//    {
//        get
//        {
//            if (_s3Client == null)
//            {
//                _s3Client = new AmazonS3Client(Credentials, BUCKET_REGION);
//            }
//            //test comment
//            return _s3Client;
//        }
//    }




//    // *********************** Initalize ***********************
//    //
//    //

//    void Start()
//    {
//        UnityInitializer.AttachToGameObject(this.gameObject);
//        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;

//        _IsInit = true;

//        DontDestroyOnLoad(this.gameObject);

//#if UNITY_IOS
//        // Forces a different code path in the BinaryFormatter that doesn't rely on run-time code generation (which would break on iOS).
//        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
//#endif
//    }

//    // Must call this Init before handle Sync System
//    public void Init(object DataModel)
//    {
//        _DataModel = DataModel;
//    }


//    // *********************** Initalize ***********************
//    //
//    //
//    public void DebugLog(string log)
//    {
//        if(IsShowDebug)
//        {
//            Debug.LogError("AwsS3Manager: " + log);
//        }
//    }


//    // *********************** Sync System - GET, POST, DELETE ***********************
//    //
//    //

//    public void GetObject(string filename, Action successCallback = null, Action failCallback = null)
//    {
//        DebugLog(string.Format("S3 - Fetching {0} from bucket {1}", filename, BUCKET_NAME));

//        // Create a request model
//        GetObjectRequest request = new GetObjectRequest
//        {
//            BucketName = BUCKET_NAME,
//        };

//        // Start to Get Object from S3
//        Client.GetObjectAsync(BUCKET_NAME, filename, (responseObj) =>
//        {
//            var response = responseObj.Response;

//            // There is response from server
//            if (response.ResponseStream != null)
//            {
//                DebugLog("GetObject - Get Object Succesfull");

//                if (successCallback != null)
//                {
//                    successCallback.Invoke();
//                }
//            }
//            // Get Object Fail
//            else
//            {
//                DebugLog("GetObject - Get Object Fail");

//                if (failCallback != null)
//                {
//                    failCallback.Invoke();
//                }
//            }
//        });
//    }

//    public void PostObject(string Email, Action callbackSuccess = null, Action callbackFail = null)
//    {
//        if (_IsInit && CheckAndCreateUserDataFile(Email))
//        {
//            string filename = "user_data/" + Email + ".json";
//            string path = Application.persistentDataPath + Path.DirectorySeparatorChar + Email + ".json";

//            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

//            // Create a request
//            var request = new PostObjectRequest()
//            {
//                Bucket = BUCKET_NAME,
//                Key = filename,
//                InputStream = stream,
//                CannedACL = S3CannedACL.PublicReadWrite,
//                Region = BUCKET_REGION
//            };

//            // Start to Post Object
//            Client.PostObjectAsync(request, (responseObj) =>
//            {
//                if (responseObj.Exception == null)
//                {
//                    DebugLog(string.Format("PostObject -> object {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket));

//                    if (callbackSuccess != null)
//                        callbackSuccess.Invoke();
//                }
//                else
//                {
//                    DebugLog(string.Format("PostObject -> receieved error {0}", responseObj.Response.HttpStatusCode.ToString()));

//                    if (callbackFail != null)
//                        callbackFail.Invoke();
//                }

//                stream.Dispose();
//                stream.Close();
//            });
//        }
//    }

//    public void DeleteObject(string Email, Action successCallback = null, Action failCallback = null)
//    {
//        string filename = "user_data/" + Email + ".json";

//        List<KeyVersion> objects = new List<KeyVersion>();
//        objects.Add(new KeyVersion()
//        {
//            Key = filename
//        });

//        // Create a request
//        var request = new DeleteObjectsRequest()
//        {
//            BucketName = BUCKET_NAME,
//            Objects = objects
//        };

//        // Delete Object
//        Client.DeleteObjectsAsync(request, (responseObj) =>
//        {
//            if (responseObj.Exception == null) // Success
//            {               

//                responseObj.Response.DeletedObjects.ForEach((dObj) =>
//                {
//                });

//                if (successCallback != null)
//                    successCallback.Invoke();
//            }
//            else // Fail
//            {
//                if (failCallback != null)
//                    failCallback.Invoke();
//            }
//        });
//    }


//    // *********************** Utils ***********************
//    //
//    //

//    bool CheckAndCreateUserDataFile(string userEmail)
//    {
//        bool isValid = false;

//        string filename = userEmail + ".json";

//        try
//        {
//            if (!File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + filename))
//            {
//                DebugLog("CreateUserDataFile: CREATE");

//                string data = JsonConvert.SerializeObject(_DataModel);

//                // file is automatically closed after reaching the end of the using block
//                using (var writer = new StreamWriter(File.Create(Application.persistentDataPath + Path.DirectorySeparatorChar + filename)))
//                {
//                    writer.WriteLine(data);
//                }

//                isValid = true;
//            }
//            else
//            {
//                DebugLog("CreateUserDataFile: ALREADY HAVE");

//                string data = JsonConvert.SerializeObject(_DataModel);

//                // file is automatically closed after reaching the end of the using block
//                using (var writer = new StreamWriter(File.Create(Application.persistentDataPath + Path.DirectorySeparatorChar + filename)))
//                {
//                    writer.WriteLine(data);
//                } 

//                isValid = true;
//            }
//        }
//        catch
//        {
//            isValid = false;
//        }

//        return isValid;
//    }
//}
