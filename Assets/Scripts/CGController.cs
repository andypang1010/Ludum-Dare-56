using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CGController : MonoBehaviour
{
    public string videoFileName;
    private VideoPlayer videoPlayer;
    void Start()
    {
        PlayVideo();
    }

    private void PlayVideo()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += Skip;

        if (videoPlayer != null)
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            print(videoPath);
            videoPlayer.url = videoPath;
            videoPlayer.Play();
        }
    }

    private void Skip(VideoPlayer source)
    {
        SceneManager.LoadScene("Playground");
    }

    void Update()
    {
        if (Input.anyKeyDown) {
            SceneManager.LoadScene("Playground");
        }
    }
}
