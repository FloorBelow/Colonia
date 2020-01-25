using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectFadeScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject spriteObject;
    MeshRenderer mesh;
    SpriteRenderer sprite;
    public Material mat;
    public Material fadeMat;
    public float t;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        sprite = spriteObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.K)) StartCoroutine(FadeTest());
    }

    IEnumerator FadeTest() {
        yield return StartCoroutine(FadeOut(t));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeIn(t));

    }



    IEnumerator FadeOut(float time) {
        mesh.material = fadeMat;
        for(float t = time; t >= 0; t -= Time.deltaTime) {
            Color c = new Color(1, 1, 1, t / time);
            mesh.material.color = c;
            sprite.color = c;
            yield return null;
        }
        mesh.material.color = Color.clear;
        sprite.color = Color.clear;
    }

    IEnumerator FadeIn(float time) {
        mesh.material = fadeMat;
        for (float t = 0; t <= time; t += Time.deltaTime) {
            Color c = new Color(1, 1, 1, t / time);
            mesh.material.color = c;
            sprite.color = c;
            yield return null;
        }
        mesh.material.color = Color.white;
        sprite.color = Color.white;
        mesh.material = mat;
    }
}
