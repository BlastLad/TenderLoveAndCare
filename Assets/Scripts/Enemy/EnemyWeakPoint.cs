using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeakPoint : MonoBehaviour
{
    // Start is called before the first frame update
    string weakPointTag = "Bullet";
    EnemyBase attachedEnemy;

    private void Awake()
    {
        attachedEnemy = GetComponentInParent<EnemyBase>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == weakPointTag)
        {
            DamageController(other.gameObject.GetComponent<NormalBullet>());
        }
    }


    void DamageController(NormalBullet bullet)
    {
        attachedEnemy.TakeDamage(bullet.damage);
        Destroy(bullet.gameObject);
    }
}
