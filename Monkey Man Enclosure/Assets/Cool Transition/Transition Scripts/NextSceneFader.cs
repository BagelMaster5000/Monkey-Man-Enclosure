using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextSceneFader : MonoBehaviour
{
    public static NextSceneFader instance;
    string nextScene;

    bool fading;

    Color[,] colorCombos = {
        { GlobalColors.melon, GlobalColors.wedgewood },
        { GlobalColors.geraldine, GlobalColors.oxfordBlue },
        { GlobalColors.wedgewood, GlobalColors.balticSea },
        { GlobalColors.oxfordBlue, GlobalColors.melon },
        { GlobalColors.balticSea, GlobalColors.geraldine },
    };
    [SerializeField] SpriteRenderer bg;
    [SerializeField] SpriteRenderer bananaGraphic;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void LateUpdate()
    {
        if (fading)
            SetParent();
    }

    /* Starts fade to next scene
     *@param nextScene name of next scene to load
     *@param stopMusic whether or not background music should fade out
     */
    public void FadeToNextScene(string nextScene, bool stopMusic)
    {
        transform.parent = null;
        DontDestroyOnLoad(this.gameObject);

        int randomColorCombo = Random.Range(0, 5);
        bg.color = colorCombos[randomColorCombo, 0];
        bananaGraphic.color = colorCombos[randomColorCombo, 1];

        if (stopMusic && MusicPlayer.instance != null)
            MusicPlayer.instance.FadeOut();

        fading = true;

        // Starts transition animation
        this.nextScene = nextScene;
        GetComponent<Animator>().SetBool("Dark", true);
    }

    /* Starts fade to next scene in scene hierarchy
     *@param stopMusic whether or not background music should fade out
     */
    public void FadeToNextScene(bool stopMusic)
    {
        transform.parent = null;
        DontDestroyOnLoad(this.gameObject);

        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            FadeToNextScene(SceneManager.GetActiveScene().name, false);
            return;
        }

        int randomColorCombo = Random.Range(0, 5);
        bg.color = colorCombos[randomColorCombo, 0];
        bananaGraphic.color = colorCombos[randomColorCombo, 1];

        if (stopMusic && MusicPlayer.instance != null)
            MusicPlayer.instance.FadeOut();

        fading = true;

        // Below code is stolen from Giora-Guttsait. Thank you and incredibly dumb that this is how you get the scene name of the next scene.
        string path = SceneUtility.GetScenePathByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        this.nextScene = name.Substring(0, dot);

        GetComponent<Animator>().SetBool("Dark", true);
    }

    // Restarts scene
    public void Restart()
    {
        transform.parent = null;
        DontDestroyOnLoad(this.gameObject);

        this.nextScene = SceneManager.GetActiveScene().name;
        GetComponent<Animator>().SetBool("Dark", true);
    }

    // Executes scene load
    public void Load()
    {
        //transform.position = Vector3.zero;

        GetComponent<Animator>().SetBool("Dark", false);
        SceneManager.LoadScene(nextScene);

        StartCoroutine(EndFadingAfterSeconds(1));
    }

    void SetParent()
    {
        Transform camTransform = Camera.main.transform;
        transform.position = new Vector3(0, 16.91447f, -26.9171f);
        transform.rotation = camTransform.rotation;
        //StartCoroutine(WaitToParent());
    }

    IEnumerator EndFadingAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        fading = false;
    }
}
