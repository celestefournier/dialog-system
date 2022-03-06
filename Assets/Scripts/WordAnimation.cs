using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordAnimation : MonoBehaviour
{
    [SerializeField] string word;
    
    TMP_Text textUI;
    TMP_TextInfo textInfo;

    void Start()
    {
        textUI = GetComponent<TMP_Text>();
        textInfo = textUI.textInfo;
    }

    void Update()
    {
        textUI.ForceMeshUpdate();

        if (string.IsNullOrEmpty(word))
            return;

        var indexList = new List<int>();

        for (int i = 0; i < textUI.text.Length; i++)
        {
            if (textUI.text[i] == word[indexList.Count])
                indexList.Add(i);
            else
                indexList.Clear();

            if (indexList.Count == word.Length)
            {
                Animate(indexList);
                indexList.Clear();
            }
        }

        textUI.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    void Animate(List<int> indexList)
    {
        foreach (var index in indexList)
        {
            var charInfo = textInfo.characterInfo[index];

            if (!charInfo.isVisible)
                continue;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig + Vector3.up * (Mathf.Sin(Time.time * 2f + orig.x * 0.01f) * 10f);
            }
        }
    }
}
