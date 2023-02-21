using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHammer : MonoBehaviour
{
    public float damage = 200.0f;
    public float range;

    int direction = -1;
    bool returning = false;
    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 destination;
    GameObject owner;

    public void SetDirection(bool flip, GameObject player) {
        direction = (flip == true) ? -1 : 1;
        Transform playerTransform = player.GetComponent<Transform>();
        startPoint = new Vector2(playerTransform.position.x, playerTransform.position.y);
        owner = player;

        startPoint = new Vector2(transform.position.x, transform.position.y);
        endPoint = new Vector2(startPoint.x + direction * range, startPoint.y);
        destination = endPoint;
        StartCoroutine(Destroyer());
    }

    void Update()
    {
        if(startPoint != endPoint) {
            if(direction > 0) {
                if(startPoint.x + direction * 0.2f > transform.position.x) {
                    destination = endPoint;
                } else if (endPoint.x - direction * 0.2f < transform.position.x) {
                    destination = startPoint;
                    returning = true;
                }
                
                transform.position = Vector2.Lerp(new Vector2(transform.position.x, transform.position.y), destination, Time.deltaTime*2);
            } else {
                if(startPoint.x + direction * 0.2f < transform.position.x) {
                    destination = endPoint;
                } else if (endPoint.x - direction * 0.2f > transform.position.x) {
                    destination = startPoint;
                    returning = true;
                }
                
                transform.position = Vector2.Lerp(new Vector2(transform.position.x, transform.position.y), destination, Time.deltaTime*2);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Hitbox") {
            collision.gameObject.GetComponent<HitboxChecker>().cangetDamaged(true);
            collision.gameObject.GetComponent<HitboxChecker>().getDamaged(damage);
        }
        
        if(collision.tag == "Boss") {
            BossPatterns boss = collision.gameObject.GetComponent<BossPatterns>();
            if(boss != null) {
                boss.bossHealth -= damage;
            } else {
                collision.gameObject.GetComponent<SingleBossPattern>().bossHealth -= damage;
            }
        }
        
        if(collision.gameObject == owner) {
            if(returning) {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator Destroyer() {
        yield return new WaitForSeconds(range / 2);
        Destroy(gameObject);
    }
}
