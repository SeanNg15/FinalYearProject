using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using UnityEngine.Networking;

public class GenerateToy : MonoBehaviour
{
    public RawImage rawImage;
    public GameObject[] toyTypes;

    public Transform generatePosition;

    public float rotSpeed = 2;

    private GameObject generatedToy;
    private Transform display;

    private bool isRotating = false;

    private Dictionary<string, int> toyDict = new Dictionary<string, int>()
    {   //0 is the default pill object
        {"ball", 1},
        {"bear", 2},
        {"teddy", 2},
        {"train", 3},
        {"car", 4},
        {"vehicle", 4},
        {"aircraft", 5},
        {"airplane", 5},
        {"plane", 5},
        {"stuffed", 6},
        {"figurine", 7},
        {"doll", 8}
    };

    void Start()
    {
        display = generatePosition;
    }

    public void ChooseImage()
    {
        //clean up
        isRotating = false;
        Destroy(generatedToy);
        if (PersistentObjects.Instance.customToy != null)
        {
            Destroy(PersistentObjects.Instance.customToy);
        }
        

        var extensions = new [] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" )
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Get Image", "", extensions, false);
        if (paths.Length > 0) {
            StartCoroutine(GetImage(new System.Uri(paths[0]).AbsoluteUri));
        }
    }

    private IEnumerator GetImage(string path)
    {
        using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path);
        yield return uwr.SendWebRequest();
        var tex = DownloadHandlerTexture.GetContent(uwr);
        rawImage.texture = tex;
        Texture2D rawImageTexture = (Texture2D)rawImage.texture;
        byte[] pngData = rawImageTexture.EncodeToPNG();
        yield return UploadToAPI(pngData);
    }

    private IEnumerator UploadToAPI(byte[] imageBytes)
    {
        string apiKey = "{YOUR API KEY HERE}";
        string url = $"https://vision.googleapis.com/v1/images:annotate?key={apiKey}";

        AnnotateImageRequests jsonRequest = new()
        {
            requests = new List<AnnotateImageRequest>()
        };

        AnnotateImageRequest req = new()
        {
            image = new GenerateToy.Image(),
            features = new List<Feature>()
        };
        string base64 = System.Convert.ToBase64String(imageBytes);
        req.image.content = base64;

        Feature f1 = new()
        {
            type = "LABEL_DETECTION",
            maxResults = 3
        };

        Feature f2 = new()
        {
            type = "IMAGE_PROPERTIES",
            maxResults = 3
        };

        Feature f3 = new()
        {
            type = "OBJECT_LOCALIZATION",
            maxResults = 3
        };

        req.features.Add(f1);
        req.features.Add(f2); 
        req.features.Add(f3);
        jsonRequest.requests.Add(req);

        string jsonData = JsonUtility.ToJson(jsonRequest, false);
        Debug.Log(jsonRequest);
        print(jsonData);
        byte[] postData = System.Text.Encoding.Default.GetBytes(jsonData);
        UnityWebRequest request = new UnityWebRequest(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(postData),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            ProcessResponse(request.downloadHandler.text);
        }
    }

    void ProcessResponse(string jsonResponse)
    {
        var response = JsonUtility.FromJson<GoogleVisionResponse>(jsonResponse);
        
        //add all possible labels to a set
        HashSet<string> labels = new();
        foreach (var obj in response.responses[0].labelAnnotations) 
        {
            var label = obj.description;
            Debug.Log("Label: " + label);
            string[] eachLabel = label.Split(" ", System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var sep in eachLabel) 
            {
                labels.Add(sep.ToLower());
            }
        }

        //add all possible objects to a set
        HashSet<string> objectDetected = new();
        if (response.responses[0].localizedObjectAnnotations != null)
        {
            foreach (var objd in response.responses[0].localizedObjectAnnotations) 
            {
                var name = objd.name;
                Debug.Log("Object detected: " + name);
                string[] eachName = name.Split(" ", System.StringSplitOptions.RemoveEmptyEntries);
                foreach (var sepN in eachName) 
                {
                    objectDetected.Add(sepN.ToLower());
                }
            }
        }
        
        int generatedObj = 0;
        //Map the detected label to the gameobject
        foreach(var possibleLable in labels)
        {
            if (toyDict.ContainsKey(possibleLable))
            {
                Debug.Log("Generate Object with id: " + toyDict[possibleLable]);
                generatedObj = toyDict[possibleLable];
                break;
            }
        }

        //Check through objects detected if labels were not detected well
        if (generatedObj == 0)
        {
            foreach(var possibleObject in objectDetected)
            {
                if (toyDict.ContainsKey(possibleObject))
                {
                    Debug.Log("Generate Object with id: " + toyDict[possibleObject]);
                    generatedObj = toyDict[possibleObject];
                    break;
                }
            }
        }

        //if not found, generate default pill
        if (generatedObj == 0)
        {
            Debug.Log("Object not found, generating default pill with id 0");
        }
        
        //get object type
        var toyType = toyTypes[generatedObj];

        //get average dominant colour
        float red = 0;
        float green = 0;
        float blue = 0;
        foreach (var c in response.responses[0].imagePropertiesAnnotation.dominantColors.colors)
        {
            red += c.color.red;
            green += c.color.green;
            blue += c.color.blue;
        }

        red /= 3;
        green /= 3;
        blue /= 3;
        Debug.Log($"Average Color: RGB({red}, {green}, {blue})");
 
        GameObject toy = Instantiate(toyType, generatePosition.position, toyType.transform.rotation);
        var renderer = toy.transform.GetChild(0).GetComponent<Renderer>();
        
        Debug.Log(new UnityEngine.Color(red, green, blue, 1f));
        renderer.material.SetColor("_BaseColor", new UnityEngine.Color(red / 255, green / 255, blue / 255, 1f));
        generatedToy = toy;
        generatedToy.transform.parent = display;

        //create new copy to be stored in persistent objects, since dont destroy on load does NOT work on non-root
        PersistentObjects.Instance.customToy = Instantiate(toy, new Vector3(0, -100, 0), toy.transform.rotation);
        DontDestroyOnLoad(PersistentObjects.Instance.customToy);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && generatedToy != null)
        {
            isRotating = true;
        } else if (Input.GetMouseButtonUp(0) && generatedToy != null)
        {
            isRotating = false;
        }
        //allows rotation
        if (isRotating && generatedToy != null)
        {
            float rotx = Input.GetAxis("Mouse X") * rotSpeed;

            display.Rotate(Vector3.down, rotx);
        }
    }

    //-------------------------------------------REQUEST CLASS--------------------------------------------------

    [System.Serializable]
	public class AnnotateImageRequests {
		public List<AnnotateImageRequest> requests;
	}

	[System.Serializable]
	public class AnnotateImageRequest {
		public Image image;
		public List<Feature> features;
	}

	[System.Serializable]
	public class Image {
		public string content;
	}

	[System.Serializable]
	public class Feature {
		public string type;
		public int maxResults;
	}

    //-------------------------------------------RESPONSE CLASS--------------------------------------------------

    [System.Serializable]
    public class GoogleVisionResponse
    {
        public Response[] responses;
    }

    [System.Serializable]
    public class Response
    {
        public LabelAnnotation[] labelAnnotations;
        public ImagePropertiesAnnotation imagePropertiesAnnotation;

        public LocalizedObjectAnnotations[] localizedObjectAnnotations;
    }

    [System.Serializable]
    public class LabelAnnotation
    {
        public string description;
    }

    [System.Serializable]
    public class ImagePropertiesAnnotation
    {
        public DominantColors dominantColors;
    }

    [System.Serializable]
    public class DominantColors
    {
        public ColorInfo[] colors;
    }

    [System.Serializable]
    public class ColorInfo
    {
        public Color color;
    }

    [System.Serializable]
    public class Color
    {
        public float red;
        public float green;
        public float blue;
    }
    
    [System.Serializable]
    public class LocalizedObjectAnnotations
    {
        public string name;
    }

}

