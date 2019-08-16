using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour {

    public GameObject BulletPrefab;
    public GameObject TargetObject;
    public Transform SpawnPosition;
    public float BulletSpeed = 60;

    public void ShootBullet(GameObject target = null)
    {
        GameObject gO = Utils.InstantiateSafe(BulletPrefab, SpawnPosition.position);
        Bullet b = gO.GetComponent<Bullet>();

        //parent, can be optimized
        string name = this.gameObject.name + " bullets";
        GameObject groupObject = GameObject.Find(name);

        if (groupObject == null)
            groupObject = new GameObject(name);

        gO.transform.parent = groupObject.transform;

        if (target == null)
            target = TargetObject; 

        if(target == null)
        {
            Debug.LogError("No bullet target assigned for PlayerShoot on object " + gameObject.name);
            return;
        }

        var pointing = target.transform.position - this.transform.position;
        var distance = pointing.magnitude;
        var direction = pointing / distance;

        b.Move(direction, BulletSpeed);

        //shoot sound
        SoundManager.Instance.PlayShootSound();
    }

    /*
     private IEnumerator BulletShooter()
     {
         for(; ; )
         {
             ShootBullet();
             yield return new WaitForSeconds(1f);
         }
     }

     void Update () {

         coolDownTimer -= Time.deltaTime;

         float rT = Input.GetAxis("RightTrigger");

         if (Input.GetMouseButton(1))
             rT = 1;

         if (rT > 0.95f && coolDownTimer <= 0)
         {
             GameObject gO = Utils.InstantiateSafe(BulletPrefab, SpawnPosition.position);
             Bullet b = gO.GetComponent<Bullet>();

             //parent, can be optimized
             string name = this.gameObject.name + " bullets";
             GameObject groupObject = GameObject.Find(name);

             if (groupObject == null)
                 groupObject = new GameObject(name);

             gO.transform.parent = groupObject.transform;

             b.Move(this.transform.forward);

             //move forward
             coolDownTimer = FireCooldown;

             //shoot sound
             SoundManager.Instance.PlayShootSound();
         }

     }
         */
}
