using NPOI.HPSF;
using System.Collections;
using UnityEngine;

public class ParticleAutoReturn : MonoBehaviour
{
    [SerializeField] private string particleName;

    private Transform target;
    private Vector3 offset = new Vector3(0, -0.3f, 0);
    private ParticlePool pool;

    private void Awake()
    {
        pool = FindObjectOfType<ParticlePool>();
    }
    private void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    private void OnDisable()
    {
        if (pool != null)
        {
            pool.ReturnParticle(particleName, gameObject);
        }

    }
    private void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
    }


    public void SetTarget(Transform t)
    {
        target = t;
    }


}