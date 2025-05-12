using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PetCare : MonoBehaviour
{
    //Script for interaction buttons

    public VirtualPet virtualPet;
    public Image pet;
    public Image heartPrefab;
    public Canvas canvas;

    public void Pet()
    {
        if (virtualPet.affection < 10)
        {
            virtualPet.affection += 1;
        }

        Image heartInstance = Instantiate(heartPrefab);

        heartInstance.transform.SetParent(canvas.transform, false);

        RectTransform petRect = pet.GetComponent<RectTransform>();
        heartInstance.transform.position = petRect.position;

        StartCoroutine(AnimateHeart(heartInstance));
    }

    public void Feed()
    {
        if (virtualPet.hunger > 0)
        {
            virtualPet.hunger -= 1;
        }
    }

    public void Play()
    {
        if (virtualPet.boredom > 0)
        {
            virtualPet.boredom -= 1;
        }
    }

    // mpve heart up and fade it out
    IEnumerator AnimateHeart(Image heart)
    {
        float duration = 1.5f; 
        float elapsedTime = 0f;

        Vector3 startPos = heart.transform.position;
        Vector3 targetPos = startPos + new Vector3(0f, 50f, 0f); 

        Color startColor = heart.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            heart.transform.position = Vector3.Lerp(startPos, targetPos, t);
            heart.color = Color.Lerp(startColor, targetColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        heart.transform.position = targetPos;
        heart.color = targetColor;

        Destroy(heart.gameObject);
    }
}
