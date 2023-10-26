using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthText : MonoBehaviour
{
    public float timeToLive = 0.5f;
    public float floatSpeed = 50f;
    public Vector3 floatDirection = new Vector3(0,1,0);
    public TextMeshProUGUI textMesh;
    float timeElapsed = 0.0f;

    RectTransform rectTransform;
    Color startingColor;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startingColor = textMesh.color;
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        rectTransform.position += floatDirection * floatSpeed * Time.deltaTime;

        textMesh.color = new Color(startingColor.r, startingColor.g, startingColor.b, 1 - timeElapsed / timeToLive);

        if (timeElapsed > timeToLive)
        {
            Destroy(gameObject);
        }
    }
}
