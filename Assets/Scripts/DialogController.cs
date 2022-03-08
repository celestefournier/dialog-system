using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    [SerializeField] float dialogSpeed;
    [SerializeField] Image avatar;
    [SerializeField] TextMeshProUGUI textComponent;
    [SerializeField] GameObject nextIcon;
    [SerializeField] Dialog[] dialogs;

    TMP_TextInfo textInfo;
    int dialogIndex = 0;
    bool dialogFinished;
    float charSpeedAnimation = 0.2f;
    Vector3 charPositionAnimation = Vector3.up * 20;

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

        textComponent.ForceMeshUpdate();

        // Save original color of each vertice and set to transparent

        var originalMeshInfo = new List<List<Color32>>();

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            originalMeshInfo.Add(new List<Color32>());

            for (int j = 0; j < textInfo.meshInfo[i].colors32.Length; j++)
            {
                originalMeshInfo[i].Add(textInfo.meshInfo[i].colors32[j]);
                textInfo.meshInfo[i].colors32[j] = new Color(
                    textInfo.meshInfo[i].colors32[j].r,
                    textInfo.meshInfo[i].colors32[j].g,
                    textInfo.meshInfo[i].colors32[j].b,
                    0
                );
            }
        }

        // Set to original colors in each time

        foreach (var charInfo in textInfo.characterInfo)
        {
            if (!charInfo.isVisible)
                continue;

            var vertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
            var vertexPosition = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                var vertexIndex = charInfo.vertexIndex + j;

                DOTween.To(
                    () => vertexColors[vertexIndex],
                    x =>
                    {
                        vertexColors[vertexIndex] = x;
                        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                    },
                    originalMeshInfo[charInfo.materialReferenceIndex][vertexIndex],
                    charSpeedAnimation
                ).SetEase(Ease.Linear);

                DOTween.To(
                    () => vertexPosition[vertexIndex] + charPositionAnimation,
                    x =>
                    {
                        vertexPosition[vertexIndex] = x;
                        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                    },
                    vertexPosition[vertexIndex],
                    charSpeedAnimation
                ).SetEase(Ease.OutQuad);
            }

            if (charInfo.character == ',')
            {
                yield return new WaitForSeconds(0.18f);
                continue;
            }

            if (Regex.IsMatch(charInfo.character.ToString(), @"[\!?]"))
            {
                yield return new WaitForSeconds(0.3f);
                continue;
            }

            yield return new WaitForSeconds(dialogSpeed);
        }

        yield return new WaitForSeconds(0.25f);

        dialogFinished = true;
        nextIcon.SetActive(true);
        dialogIndex++;
    }
}
