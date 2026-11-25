using UnityEngine;
using UnityEngine.Video;
using TMPro; 

public class VRAppController : MonoBehaviour
{
    [Header("1. Main 3D object groups (from Hierarchy):")]
    public GameObject menu3DGroup;      // Menu3D_Group
    public GameObject player3DGroup;    // Player3D_Group

    [Header("2. Player and video list (.mp4):")]
    public VideoPlayer videoPlayer;     // VideoScreen object
    public VideoClip[] videoClips;      // List of videos 

    [Header("3. Progress bar and PLAY/STOP button text:")]
    public Transform progressBarFill;   // ProgressBar_Fill object
    public float fullBarWidth = 2.4f;   // Full width of the progress bar
    public TMP_Text playButtonText;     // Text (TMP) inside Btn3D_Play

    private bool isPaused = false;

    void Start()
    {
        // If you forgot to drag VideoScreen in the Inspector, the script will find it automatically:
        if (videoPlayer == null && player3DGroup != null)
            videoPlayer = player3DGroup.GetComponentInChildren<VideoPlayer>(true);

        ShowMenu();
    }

    void Update()
    {
        // Real-time update of the red 3D progress bar during playback
        if (player3DGroup != null && player3DGroup.activeSelf && videoPlayer != null && videoPlayer.length > 0 && progressBarFill != null)
        {
            float progress = Mathf.Clamp01((float)(videoPlayer.time / videoPlayer.length));

            Vector3 scale = progressBarFill.localScale;
            scale.x = Mathf.Max(0.001f, fullBarWidth * progress);
            progressBarFill.localScale = scale;

            Vector3 pos = progressBarFill.localPosition;
            pos.x = -(fullBarWidth / 2f) + (scale.x / 2f);
            progressBarFill.localPosition = pos;
        }
    }

    // Selecting a video from the list 
    public void SelectAndPlayVideo(int videoIndex)
    {
        if (menu3DGroup != null) menu3DGroup.SetActive(false);
        if (player3DGroup != null) player3DGroup.SetActive(true);

        if (videoPlayer == null) return;

        // Swap the clip only if a file was actually assigned in Video Clips on the Main Camera
        if (videoClips != null && videoIndex >= 0 && videoIndex < videoClips.Length && videoClips[videoIndex] != null)
        {
            videoPlayer.clip = videoClips[videoIndex];
        }

        videoPlayer.Stop();
        videoPlayer.time = 0;
        videoPlayer.Play();
        isPaused = false;

        // When the video starts after selecting from the menu, the button should display "STOP"
        UpdatePlayButtonLabel();
    }

    // Pause (STOP) / Resume (PLAY) video and update label
    public void TogglePlayPause()
    {
        if (videoPlayer == null) return;

        if (videoPlayer.isPlaying || !isPaused)
        {
            videoPlayer.Pause();
            isPaused = true;
        }
        else
        {
            videoPlayer.Play();
            isPaused = false;
        }

        UpdatePlayButtonLabel();
    }

    private void UpdatePlayButtonLabel()
    {
        if (playButtonText != null)
        {
            playButtonText.text = isPaused ? "PLAY" : "STOP";
        }
    }

    // Return from video to the menu
    public void ShowMenu()
    {
        if (videoPlayer != null) videoPlayer.Stop();

        if (player3DGroup != null) player3DGroup.SetActive(false);
        if (menu3DGroup != null) menu3DGroup.SetActive(true);
    }

    // Seek video (-5s / +5s)
    public void SeekVideo(float secondsDelta)
    {
        if (videoPlayer == null || !videoPlayer.isPrepared) return;

        double newTime = videoPlayer.time + secondsDelta;
        videoPlayer.time = Mathf.Clamp((float)newTime, 0f, (float)videoPlayer.length);
    }

    // Exit application (EXIT button)
    public void QuitApplication()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}