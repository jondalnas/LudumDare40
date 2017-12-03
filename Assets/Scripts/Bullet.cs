using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	Rigidbody2D rb;

	private float instTime;

	public float bulletSpeed = 50f;
	public int bounces = 3;
	public GameObject particles;
	public 
	
	void Start() {
		rb = GetComponent<Rigidbody2D>();
		instTime = Time.timeSinceLevelLoad;
	}
	
	void FixedUpdate() {
		if (Game.paused || Game.clearedLevel) return;

		if (Time.timeSinceLevelLoad - instTime > 0.5f && rb.velocity.sqrMagnitude < (bulletSpeed * 0.9f) * (bulletSpeed * 0.9f)) Destroy(gameObject);
		
		rb.velocity = transform.up*bulletSpeed;
	}

	int bounced;
	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.CompareTag("Wall")) {
			ContactPoint2D contact = col.contacts[0];

			GameObject particle = Instantiate(particles);
			particle.transform.position = transform.position;
			particle.transform.eulerAngles = new Vector3(Mathf.Atan2(contact.normal.y, contact.normal.x) * Mathf.Rad2Deg - 70f, -90f, 90f);

			if (bounced > bounces) {
				Destroy(gameObject);
				return;
			}

			bounced++;
			Vector2 bulletDir = transform.up;
			Vector2 newDir = Vector2.Reflect(bulletDir, contact.normal);
			transform.up = newDir;
		} else if (col.gameObject.CompareTag("Enemy")) {
			col.gameObject.GetComponent<EnemyAI>().die(transform.rotation);
			Destroy(gameObject);
		} else if (col.gameObject.CompareTag("Bullet")) {
			Destroy(col.gameObject);
			Destroy(gameObject);
		} else if (col.gameObject.CompareTag("Player")) {
			col.transform.GetComponent<PlayerController>().die(transform.rotation);
			Destroy(gameObject);
		}
	}
}
