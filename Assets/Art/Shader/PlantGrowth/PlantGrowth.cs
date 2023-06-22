using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlantGrowth : MonoBehaviour
{
    // Start is called before the first frame update

    [Serializable]
    class GrowMaterialProperties
    {
        public GameObject growVinesMesh;
        [Min(1)] public int id;
        public int waitForId;
        public float delay;
        public UnityEvent onDone;
        [HideInInspector] public Renderer renderer;
        public bool grown { get; set; }
    }

    [SerializeField] private List<GrowMaterialProperties> _growMaterialProperties = new();
    [SerializeField] private List<Transform> fruitTransforms = new();
    [SerializeField] private GameObject fruitPrefab;
    [SerializeField] private int startAmount;

    [SerializeField] private float timeToGrow = 5;
    [Range(0, 1)] [SerializeField] private float minGrow = 0.2f;
    [Range(0, 2)] [SerializeField] private float maxGrow = 0.97f;

    [SerializeField] private UnityEvent onFullyGrown;
    
    private bool _fullyGrown;
    private static readonly int Grow = Shader.PropertyToID("_Grow");


    void Start()
    {
        //Collect materials from the gameobjects
        foreach (var property in _growMaterialProperties)
        {
            property.renderer = property.growVinesMesh.GetComponent<Renderer>();
            foreach (var material in property.renderer.materials)
            {
                material.SetFloat(Grow, minGrow);
            }
        }

        SubscribeWaitingObjects();


        for (int i = 0; i < startAmount; i++)
        {
            AddFruit();
        }
    }

    private void SubscribeWaitingObjects()
    {
        // Loop through the list
        foreach (var growMaterial in _growMaterialProperties)
        {
            // Check if the object is waiting for another one
            if (growMaterial.waitForId != 0)
            {
                // Find the object it's waiting for
                GrowMaterialProperties waitForObject =
                    _growMaterialProperties.Find(g => g.id == growMaterial.waitForId);

                // If the object exists, subscribe a method to its _onDone event
                waitForObject?.onDone.AddListener(() => OnWaitForObjectDone(growMaterial));
            }
        }
    }

    // Method to be called when the _onDone event of a waiting object is invoked
    private void OnWaitForObjectDone(GrowMaterialProperties growMaterial)
    {
        // Logic to handle when the object it's waiting for is done
        StartCoroutine(GrowVines(growMaterial));
    }


    IEnumerator GrowVines(GrowMaterialProperties growMaterialProperties)
    {
        float growValue = growMaterialProperties.renderer.materials[0].GetFloat(Grow);

        yield return new WaitForSeconds(growMaterialProperties.delay);

        while (growValue < maxGrow)
        {
            growValue += 1 / (timeToGrow / Time.deltaTime);

            foreach (var material in growMaterialProperties.renderer.materials)
            {
                material.SetFloat(Grow, growValue);
            }
            //growValue = Mathf.Clamp(growValue, minGrow, magic.CompletionLevel.Value);

            yield return null;
        }

        growMaterialProperties.grown = true;
        growMaterialProperties.onDone?.Invoke();

        _fullyGrown = _growMaterialProperties.All(p => p.grown);
        
        if (_fullyGrown)
        {
            onFullyGrown?.Invoke();
        }
    }

    /*IEnumerator GrowVines(int i)
    {
        float growValue = growMaterials[i].GetFloat(Grow);

        if (_fullyGrown) yield break;

        if (_growMaterialProperties[i].onDone)
        {
            while (_growMaterialProperties.Any(m => !m.onDone && !m.grown))
            {
                yield return null;
            }
        }

        if (_growMaterialProperties[i].delay > 0.001f)
        {
            yield return new WaitForSeconds(_growMaterialProperties[i].delay);
        }

        while (growValue < maxGrow)
        {
            growValue += 1 / (timeToGrow / Time.deltaTime);

            //growValue = Mathf.Clamp(growValue, minGrow, magic.CompletionLevel.Value);
            growMaterials[i].SetFloat(Grow, growValue);

            yield return null;
        }

        _growMaterialProperties[i].grown = true;
        _fullyGrown = growValue >= maxGrow;
    }
    */

    IEnumerator ShrinkVines(Material mat, float delay)
    {
        float growValue = mat.GetFloat(Grow);

        if (growValue <= minGrow) yield break;

        float waitTime = 0f;

        while (waitTime < delay)
        {
            waitTime += Time.deltaTime;
        }

        while (growValue > minGrow)
        {
            growValue -= 1 / (timeToGrow / Time.deltaTime);

            mat.SetFloat(Grow, growValue);

            yield return null;
        }
    }

    public void StartGrow()
    {
        _growMaterialProperties.Where(p => p.id == 1).ToList().ForEach(p => StartCoroutine(GrowVines(p)));
    }

    /*public void StartGrowOnDone()
    {
        for (int i = 0; i < growMaterials.Count; i++)
        {
            if (_growMaterialProperties[i].onDone)
            {
                StartCoroutine(GrowVines(i));
            }
        }
    }*/

    /*
    public void StopGrow()
    {
        StopAllCoroutines();
        for (int i = 0; i < growMaterials.Count; i++)
        {
            StartCoroutine(ShrinkVines(growMaterials[i],
                _growMaterialProperties.Max(v => v.delay) - _growMaterialProperties[i].delay));
        }
    }
    */

    public void AddFruit()
    {
        Transform fruitTrans = fruitTransforms.First(f => f.childCount == 0);

        if (!fruitTrans)
            return;

        GameObject fruit = Instantiate(fruitPrefab, fruitTrans, true);

        fruit.tag = "Fruit";
        fruit.transform.position = fruitTrans.position;
        fruit.GetComponent<Rigidbody>().isKinematic = true;
    }
}