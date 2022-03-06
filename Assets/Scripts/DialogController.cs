using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    [SerializeField] float dialogSpeed = 0.05f;
    [SerializeField] Image avatar;
    [SerializeField] TextMeshProUGUI textComponent;
    [SerializeField] GameObject nextIcon;
    [SerializeField] Dialog[] dialogs;

    TMP_TextInfo textInfo;
    int dialogIndex = 0;
    bool dialogFinished;

    void Start()
    {
        textInfo = textComponent.textInfo;
        StartCoroutine("Type");
    }

    void Update()
    {
        if (Input.GetButton("Submit") && dialogFinished && dialogIndex < dialogs.Length)
        {
            StartCoroutine("Type");
        }
    }

    IEnumerator Type()
    {
        dialogFinished = false;
        nextIcon.SetActive(false);
        textComponent.text = dialogs[dialogIndex].sentence;
        avatar.sprite = dialogs[dialogIndex].avatar;

        // Save original color of each vertice and set to transparent

        textComponent.ForceMeshUpdate();
        var originalMeshInfo = new List<List<Color32>>();

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            originalMeshInfo.Add(new List<Color32>());

            for (int j = 0; j < textInfo.meshInfo[i].colors32.Length; j++)
            {
                originalMeshInfo[i].Add(textInfo.meshInfo[i].colors32[j]);
                textInfo.meshInfo[i].colors32[j] = Color.clear;
            }
        }

        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        // Set to original colors in each time

        foreach (var charInfo in textInfo.characterInfo)
        {
            if (!charInfo.isVisible)
                continue;
            
            var vertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;

            for (int j = 0; j < 4; j++)
            {
                vertexColors[charInfo.vertexIndex + j] =
                    originalMeshInfo[charInfo.materialReferenceIndex][charInfo.vertexIndex + j];
            }

            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            yield return new WaitForSeconds(dialogSpeed);
        }

        yield return new WaitForSeconds(0.25f);

        dialogFinished = true;
        nextIcon.SetActive(true);
        dialogIndex++;
    }
}
