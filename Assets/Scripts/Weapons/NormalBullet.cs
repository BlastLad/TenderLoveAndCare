using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBullet : PortalTraveller
{

    public int damage = 1;

    private void Awake()
    {
        StartCoroutine(destroyBullet());
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject)
        {
            Destroy(this.gameObject);
        }
    }

    private IEnumerator destroyBullet()
    {
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
    }
}
