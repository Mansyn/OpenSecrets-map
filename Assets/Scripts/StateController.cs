using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
//using System.Reflection;
//using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StateController : MonoBehaviour, IFocusable, IInputClickHandler
{
    public string Abbreviation;
    
    private AudioSource clickSound;

    public Material mat;
    public Material hoverMat;

    private GameObject selectState;
    private GameObject firstSenator;
    private GameObject secondSenator;

    private GameObject firstContributorOne;
    private GameObject firstContributorTwo;
    private GameObject firstContributorThree;
    private GameObject secondContributorOne;
    private GameObject secondContributorTwo;
    private GameObject secondContributorThree;

    private GameObject firstSenatorProfile;
    private GameObject secondSenatorProfile;
    
    private string apiUrl = "http://hololensapi20170428035710.azurewebsites.net/api/opensecrets/";

    private void Start()
    {
        selectState = GameObject.Find("SelectState");
        firstSenator = GameObject.Find("FirstSenator");
        secondSenator = GameObject.Find("SecondSenator");
        firstContributorOne = GameObject.Find("FirstContributorOne");
        firstContributorTwo = GameObject.Find("FirstContributorTwo");
        firstContributorThree = GameObject.Find("FirstContributorThree");
        secondContributorOne = GameObject.Find("SecondContributorOne");
        secondContributorTwo = GameObject.Find("SecondContributorTwo");
        secondContributorThree = GameObject.Find("SecondContributorThree");

        firstSenatorProfile = GameObject.Find("ProfileOne");
        secondSenatorProfile = GameObject.Find("ProfileTwo");

        clickSound = GameObject.Find("ClickSound").GetComponent<AudioSource>();
    }

    public void OnFocusEnter()
    {
        gameObject.GetComponent<Renderer>().material = hoverMat;
    }

    public void OnFocusExit()
    {
        gameObject.GetComponent<Renderer>().material = mat;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        clickSound.Play();
        selectState.GetComponent<TextMesh>().text = gameObject.name;

        StartCoroutine(GetLegislators());
    }

    IEnumerator GetLegislators()
    {
        string requestUrl = apiUrl + Abbreviation;
        UnityWebRequest www = UnityWebRequest.Get(requestUrl);
        //Debug.Log("Clicked state " + Abbreviation);

        yield return www.Send();

        if (www.isError)
        {
            Debug.Log(www.error);
        }
        else
        {
            var senators = JsonUtility.FromJson<Senators>(www.downloadHandler.text);

            //Debug.Log(senators.senatorOne.FullName + ":" + senators.senatorOne.Id);
            firstSenator.GetComponent<TextMesh>().text = senators.senatorOne.FullName;

            if (senators.senatorTwo.Id != null)
            {
                //Debug.Log(senators.senatorTwo.FullName + ":" + senators.senatorTwo.Id);
                secondSenator.GetComponent<TextMesh>().text = senators.senatorTwo.FullName;
            }
            else
            {
                secondSenator.GetComponent<TextMesh>().text = string.Empty;
            }

            firstContributorOne.GetComponent<TextMesh>().text = senators.oneContribOne.Organization + ": " + string.Format("{0:C0}", Convert.ToInt32(senators.oneContribOne.Total));
            firstContributorTwo.GetComponent<TextMesh>().text = senators.oneContribTwo.Organization + ": " + string.Format("{0:C0}", Convert.ToInt32(senators.oneContribTwo.Total));
            firstContributorThree.GetComponent<TextMesh>().text = senators.oneContribThree.Organization + " : " + string.Format("{0:C0}", Convert.ToInt32(senators.oneContribThree.Total));

            if (senators.senatorTwo.Id != null)
            {
                secondContributorOne.GetComponent<TextMesh>().text = senators.twoContribOne.Organization + ": " + string.Format("{0:C0}", Convert.ToInt32(senators.twoContribOne.Total));
                secondContributorTwo.GetComponent<TextMesh>().text = senators.twoContribTwo.Organization + ": " + string.Format("{0:C0}", Convert.ToInt32(senators.twoContribTwo.Total));
                secondContributorThree.GetComponent<TextMesh>().text = senators.twoContribThree.Organization + ": " + string.Format("{0:C0}", Convert.ToInt32(senators.twoContribThree.Total));
            }
            else
            {
                secondContributorOne.GetComponent<TextMesh>().text = string.Empty;
                secondContributorTwo.GetComponent<TextMesh>().text = string.Empty;
                secondContributorThree.GetComponent<TextMesh>().text = string.Empty;
            }

            var SenatorOneImage = firstSenatorProfile.GetComponent<RawImage>();
            var SenatorTwoImage = secondSenatorProfile.GetComponent<RawImage>();
            if (senators.senatorOne.Id != null)
            {
                var firstSenatorTexture = Resources.Load<Texture>(senators.senatorOne.Id);
                var transform1 = firstSenatorProfile.GetComponent<RectTransform>();
                Debug.Log("First senator: w:" + firstSenatorTexture.width + " h:" + firstSenatorTexture.height);
                transform1.sizeDelta = new Vector2(firstSenatorTexture.width / 3.5f, firstSenatorTexture.height / 3.5f);

                SenatorOneImage.enabled = true;
                SenatorOneImage.texture = firstSenatorTexture;
            }
            else
            {
                SenatorOneImage.enabled = false;
            }

            if (senators.senatorTwo.Id != null)
            {
                var secondSenatorTexture = Resources.Load<Texture>(senators.senatorTwo.Id);
                var transform2 = secondSenatorProfile.GetComponent<RectTransform>();
                Debug.Log("Second senator: w:" + secondSenatorTexture.width + " h:" + secondSenatorTexture.height);
                transform2.sizeDelta = new Vector2(secondSenatorTexture.width / 3, secondSenatorTexture.height / 3);

                SenatorTwoImage.enabled = true;
                SenatorTwoImage.texture = secondSenatorTexture;
            }
            else
            {
                SenatorTwoImage.enabled = false;
            }
        }
    }

    //public static bool GetImageSize(Texture asset, out int width, out int height)
    //{
    //    if (asset != null)
    //    {
    //        string assetPath = AssetDatabase.GetAssetPath(asset);
    //        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

    //        if (importer != null)
    //        {
    //            object[] args = new object[2] { 0, 0 };
    //            MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
    //            mi.Invoke(importer, args);

    //            width = (int)args[0] / 3;
    //            height = (int)args[1] / 3;

    //            return true;
    //        }
    //    }

    //    height = width = 0;
    //    return false;
    //}

    [Serializable]
    public class SenatorOne
    {
        public string Id;
        public string FullName;
        public string Lastname;
        public string ImageUrl;
        public string State;
        public string Gender;
        public string Branch;
        public string Party;
        public string Office;
        public string YearElected;
        public string ExitCode;
        public string Comments;
        public string Phone;
        public string Fax;
        public string Website;
        public string Webform;
        public string CongressOffice;
        public string BioguideId;
        public string VotesmartId;
        public string FeccandId;
        public string TwitterId;
        public string YoutubeUrl;
        public string FacebookId;
        public string Birthdate;
        public string PartitionKey;
        public string RowKey;
        public string Timestamp;
        public string ETag;
    }

    [Serializable]
    public class OneContribOne
    {
        public string Id;
        public string CID;
        public string Cycle;
        public string Organization;
        public string Total;
        public string Pacs;
        public string Indivs;
        public string PartitionKey;
        public string RowKey;
        public string Timestamp;
        public string ETag;
    }

    [Serializable]
    public class OneContribTwo
    {
        public string Id;
        public string CID;
        public string Cycle;
        public string Organization;
        public string Total;
        public string Pacs;
        public string Indivs;
        public string PartitionKey;
        public string RowKey;
        public string Timestamp;
        public string ETag;
    }

    [Serializable]
    public class OneContribThree
    {
        public string Id;
        public string CID;
        public string Cycle;
        public string Organization;
        public string Total;
        public string Pacs;
        public string Indivs;
        public string PartitionKey;
        public string RowKey;
        public string Timestamp;
        public string ETag;
    }

    [Serializable]
    public class SenatorTwo
    {
        public string Id;
        public string FullName;
        public string Lastname;
        public string ImageUrl;
        public string State;
        public string Gender;
        public string Branch;
        public string Party;
        public string Office;
        public string YearElected;
        public string ExitCode;
        public string Comments;
        public string Phone;
        public string Fax;
        public string Website;
        public string Webform;
        public string CongressOffice;
        public string BioguideId;
        public string VotesmartId;
        public string FeccandId;
        public string TwitterId;
        public string YoutubeUrl;
        public string FacebookId;
        public string Birthdate;
        public string PartitionKey;
        public string RowKey;
        public string Timestamp;
        public string ETag;
    }

    [Serializable]
    public class TwoContribOne
    {
        public string Id;
        public string CID;
        public string Cycle;
        public string Organization;
        public string Total;
        public string Pacs;
        public string Indivs;
        public string PartitionKey;
        public string RowKey;
        public string Timestamp;
        public string ETag;
    }

    [Serializable]
    public class TwoContribTwo
    {
        public string Id;
        public string CID;
        public string Cycle;
        public string Organization;
        public string Total;
        public string Pacs;
        public string Indivs;
        public string PartitionKey;
        public string RowKey;
        public string Timestamp;
        public string ETag;
    }

    [Serializable]
    public class TwoContribThree
    {
        public string Id;
        public string CID;
        public string Cycle;
        public string Organization;
        public string Total;
        public string Pacs;
        public string Indivs;
        public string PartitionKey;
        public string RowKey;
        public string Timestamp;
        public string ETag;
    }

    [Serializable]
    public class Senators
    {
        public SenatorOne senatorOne;
        public OneContribOne oneContribOne;
        public OneContribTwo oneContribTwo;
        public OneContribThree oneContribThree;
        public SenatorTwo senatorTwo;
        public TwoContribOne twoContribOne;
        public TwoContribTwo twoContribTwo;
        public TwoContribThree twoContribThree;
    }
}
