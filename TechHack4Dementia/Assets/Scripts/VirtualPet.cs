using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VirtualPet : MonoBehaviour
{
    public TextMeshProUGUI hungerText;
    public TextMeshProUGUI boredomText;
    public TextMeshProUGUI affectionText;

    //attributes
    [Header("Attributes")]
    public int hunger = 0;
    public int boredom = 0;
    public int affection = 10;

    //speechbubbles
    [Header("Speech Bubbles")]
    public Image FeedBubble;
    public Image PlayBubble;
    public Image PetBubble;

    [Header("Sliders")]
    public Slider hungerSlider;
    public Slider boredomSlider;
    public Slider affectionSlider;

    [Header("Canvas")]
    public Canvas canvas;

    [Header("Sprites")]
    public Sprite petSit;
    public Sprite petRun1;
    public Sprite petRun2;
    public Sprite petSleep;

    [Header("Pet Image")]
    public Image petImage;

    //Custom colours for sliders
    Color pinkColour = new Color(0.7452f, 0.3550f, 0.5928f);
    Color greenColour = new Color(0.4133f, 0.7075f, 0.4105f);
    Color redColour = new Color(0.6509f, 0.2487f, 0.2528f);

    private float screenWidth;

    // Start is called before the first frame update
    void Start()
    {
        screenWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        petImage.sprite = petSit;
        StartCoroutine(ChooseAction(Random.Range(1, 5)));
    }

    // Update is called once per frame
    void Update()
    {
        hungerSlider.value = hunger;
        boredomSlider.value = boredom;
        affectionSlider.value = affection;

        //Change slider colours when attributes need fixing
        UpdateSliderColor(hungerSlider, hunger, greenColour, redColour);
        UpdateSliderColor(boredomSlider, boredom, greenColour, redColour);
        UpdateSliderColor(affectionSlider, affection, Color.grey, pinkColour); 
    }

    void UpdateSliderColor(Slider slider, int value, Color lowColor, Color highColor)
    {
        if (value <= 5)
        {
            slider.fillRect.GetComponent<Image>().color = lowColor;
        }
        else
        {
            slider.fillRect.GetComponent<Image>().color = highColor;
        }
    }

    //Move the pet around
    void PetWalk(int direction)
    {
        //how far it'll walk
        float minStep = 100f;
        float maxStep = 150f;
        float walkDistance = Random.Range(minStep, maxStep);

        float currentX = petImage.rectTransform.localPosition.x;

        RectTransform canvasRect = petImage.canvas.GetComponent<RectTransform>();
        float canvasHalfWidth = canvasRect.rect.width / 2f;

        float petHalfWidth = petImage.rectTransform.rect.width / 2f;

        float leftLimit = -canvasHalfWidth + petHalfWidth;
        float rightLimit = canvasHalfWidth - petHalfWidth;

        float targetX = currentX + direction * walkDistance;

        // Flip direction if goes off screen
        if (targetX < leftLimit)
        {
            direction = 1;
            targetX = currentX + direction * walkDistance;
        }
        else if (targetX > rightLimit)
        {
            direction = -1;
            targetX = currentX + direction * walkDistance;
        }

        targetX = Mathf.Clamp(targetX, leftLimit, rightLimit);
        petImage.sprite = petSit;

        StartCoroutine(MovePet(targetX));
    }

    IEnumerator MovePet(float targetX)
    {
        float startX = petImage.rectTransform.localPosition.x;
        float elapsedTime = 0f;
        float duration = 1f; 

        while (elapsedTime < duration)
        {
            float newX = Mathf.Lerp(startX, targetX, elapsedTime / duration);
            petImage.rectTransform.localPosition = new Vector3(newX, petImage.rectTransform.localPosition.y, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        petImage.rectTransform.localPosition = new Vector3(targetX, petImage.rectTransform.localPosition.y, 0);
        petImage.sprite = petSit;
    }

    IEnumerator JumpPet()
    {
        petImage.sprite = petSleep;

        float jumpHeight = 100f;
        float duration = 0.6f;
        Vector3 startPos = petImage.transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Parabola curve - whatever that means O_o
            float yOffset = 4 * jumpHeight * t * (1 - t);

            petImage.transform.localPosition = new Vector3(
                startPos.x,
                startPos.y + yOffset,
                startPos.z
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        petImage.transform.localPosition = startPos;
    }

    IEnumerator ChooseAction(int seconds)
    {
        while (true)
        {
            int waitTime = Random.Range(3, 6); // Wait for a random time between 3 and 6 seconds
            yield return new WaitForSeconds(waitTime);
            petImage.sprite = petSit;

            int action = Random.Range(1, 4); // biased towards walking cause otherwise it keeps jumping

            if (boredom < 10)
            {
                boredom += 1;
            }
            else
            {
                //there's probably a better way to do this
                Image playBubbleInstance = Instantiate(PlayBubble);
                playBubbleInstance.transform.SetParent(canvas.transform, false);
                RectTransform petRect = petImage.GetComponent<RectTransform>();
                playBubbleInstance.transform.position = petRect.position;
                StartCoroutine(SpeechBubbleAnim(playBubbleInstance));
            }

            if (action == 1)
            {
                Debug.Log("Jump");
                StartCoroutine(JumpPet());
                if(affection > 0)
                {
                    affection -= 1; // Decrease affection
                }
                else
                {
                    Image petBubbleInstance = Instantiate(PetBubble);
                    petBubbleInstance.transform.SetParent(canvas.transform, false);
                    RectTransform petRect = petImage.GetComponent<RectTransform>();
                    petBubbleInstance.transform.position = petRect.position;
                    StartCoroutine(SpeechBubbleAnim(petBubbleInstance));
                }
            }
            else
            {
                Debug.Log("Walk");
                if (hunger < 10)
                {
                    hunger += 1; // Increase hunger
                }
                else
                {
                    Image feedBubbleInstance = Instantiate(FeedBubble);
                    feedBubbleInstance.transform.SetParent(canvas.transform, false);
                    RectTransform petRect = petImage.GetComponent<RectTransform>();
                    feedBubbleInstance.transform.position = petRect.position;
                    StartCoroutine(SpeechBubbleAnim(feedBubbleInstance));
                }

                int dir = Random.Range(1, 3);
                if (dir == 1)
                {
                    PetWalk(1); // Walk right
                    petImage.sprite = petRun1;
                }
                else
                {
                    PetWalk(-1); // Walk left
                    petImage.sprite = petRun2; 
                }
            }
        }
    }

    //animate the speech bubble woaaahhhh
    IEnumerator SpeechBubbleAnim(Image prefab)
    {
        float duration = 1.5f;
        float elapsedTime = 0f;

        Vector3 startPos = prefab.transform.position;
        Vector3 targetPos = startPos + new Vector3(0f, 50f, 0f);

        Color startColor = prefab.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            prefab.transform.position = Vector3.Lerp(startPos, targetPos, t);
            prefab.color = Color.Lerp(startColor, targetColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        prefab.transform.position = targetPos;
        prefab.color = targetColor;

        Destroy(prefab.gameObject);
    }

}
