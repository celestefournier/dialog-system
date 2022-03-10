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
    [SerializeField] ArrowVfx arrowIcon;
    [SerializeField] Dialog[] dialogs;

    TMP_TextInfo textInfo;
    int dialogIndex = 0;
    bool dialogFinished;
    float charSpeedAnimation = 0.15f;
    Vector3 charPositionAnimation = Vector3.up * 15;

    void Start()
    {
        textInfo = textComponent.textInfo;
        StartAnimation();
    }

    void Update()
    {
        if (Input.GetButton("Submit") && dialogFinished && dialogIndex < dialogs.Length)
        {
            StartCoroutine("Type");
        }
    }

    void StartAnimation()
    {
        textComponent.gameObject.SetActive(false);

        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScaleY(0, 0.3f).From().SetEase(Ease.OutQuad));
        sequence.PrependInterval(1);
        sequence.AppendCallback(() =>
        {
            textComponent.gameObject.SetActive(true);
            StartCoroutine("Type");
        });
    }

    IEnumerator Type()
    {
        dialogFinished = false;
        arrowIcon.Hide();
        textComponent.text = dialogs[dialogIndex].sentence;
        avatar.sprite = dialogs[dialogIndex].avatar;
        DOTween.Kill(gameObject);

        textComponent.ForceMeshUpdate();

        // Save original color of each vertice and set to transparent

        var originalMeshInfo = new List<List<Color32>>();

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            originalMeshInfo.Add(new List<Color32>());

            for (int j = 0; j < textInfo.meshInfo[i].colors32.Length; j++)
            {
                originalMeshInfo[i].Add(textInfo.meshInfo[i].colors32[j]);
                textInfo.meshInfo[i].colors32[j] = new Color32(
                    textInfo.meshInfo[i].colors32[j].r,
                    textInfo.meshInfo[i].colors32[j].g,
                    textInfo.meshInfo[i].colors32[j].b,
                    0
                );
            }
        }

        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        // "<link=wave>" animation

        for (int i = 0; i < textInfo.linkCount; i++)
        {
            var link = textInfo.linkInfo[i];

            if (link.GetLinkID() != "wave")
                continue;

            var firstIndex = link.linkTextfirstCharacterIndex;
            var lastIndex = link.linkTextfirstCharacterIndex + link.linkTextLength;

            for (int j = firstIndex; j < lastIndex; j++)
            {
                var charInfo = textComponent.textInfo.characterInfo[j];

                if (!charInfo.isVisible)
                    continue;

                var vertexPosition = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                for (int k = 0; k < 4; k++)
                {
                    var vertexIndex = charInfo.vertexIndex + k;
                    var delay = (lastIndex - j) * 0.06f;

                    DOTween.To(
                        () => vertexPosition[vertexIndex] - charPositionAnimation / 2,
                        x =>
                        {
                            vertexPosition[vertexIndex] = x;
                            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                        },
                        vertexPosition[vertexIndex] + charPositionAnimation / 2,
                        charSpeedAnimation * 3
                    ).SetDelay(delay).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetId(gameObject);
                }
            }
        }

        // Restore original color of each vertice

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

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
                ).SetEase(Ease.OutSine);
            }

            if (charInfo.character == ',')
            {
                yield return new WaitForSeconds(0.2f);
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
        dialogIndex++;

        if (dialogIndex < dialogs.Length)
            arrowIcon.Show();
    }
}
