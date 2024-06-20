using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using UnityEngine.Networking;

public class UploadPerp : MonoBehaviour
{
    public RawImage perp;
    public GameObject tvOutput;
    
    public void ChooseImage()
    {
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
        perp.texture = tex;
        var renderer = tvOutput.GetComponent<Renderer>();
        renderer.material.mainTexture = tex;
        var tempcolor = renderer.material.color;
        tempcolor.a = 1f;
        renderer.material.color = tempcolor;
    }
}
