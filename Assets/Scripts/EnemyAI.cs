using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
	SpriteRenderer sr;
	BoxCollider2D bc;

	private bool dead;
	private bool hasSeenPlayer;
	private Quaternion startDirrection;
	private Sprite idelSprite;

	public float cooldown = 0.5f;
	public GameObject player;
	public Sprite deadSprite;
	public GameObject blood;
	public GameObject bullet;
	public Transform barrel;
	public GameObject pistol;
	public GameObject particle;
	public float FOV = 90f;
	public AudioClip[] sounds;

	void Start() {
		sr = GetComponent<SpriteRenderer>();
		bc = GetComponent<BoxCollider2D>();
		startDirrection = transform.rotation;
		idelSprite = sr.sprite;
		cooldownTimer = cooldown / 2f;
		Game.enemies++;
	}

	float cooldownTimer;
	void Update() {
		if (particle.GetComponent<ParticleSystem>().particleCount >= 400) {
			particle.GetComponent<ParticleSystem>().Pause();
		}

		if (Game.paused || Game.clearedLevel) return;

		if (dead || !hasSeenPlayer || player.GetComponent<PlayerController>().isDead()) return;

		Vector3 diff = player.transform.position-transform.position;
		diff.Normalize();
		transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(diff.y, diff.x)*Mathf.Rad2Deg-90);

		cooldownTimer += Time.deltaTime;
		if (cooldown < cooldownTimer) {
			cooldownTimer = 0;
			Instantiate(bullet, barrel.position, transform.rotation);

			GetComponent<AudioSource>().clip = sounds[1];
			GetComponent<AudioSource>().Play();
		}
	}

	public void die(Quaternion bulletDirrection) {
		if (dead) return;
		dead = true;

		Game.enemies--;

		particle.GetComponent<ParticleSystem>().Play();

		transform.rotation = bulletDirrection;

		sr.sprite = deadSprite;

		bc.enabled = false;

		GameObject bloodgo = GameObject.Instantiate(blood, transform);
		bloodgo.transform.localPosition = new Vector3(0, 0, 0.02f);
		//Destroy(gameObject);

		GetComponent<AudioSource>().clip = sounds[0];
		GetComponent<AudioSource>().Play();
	}

	public bool isDead() {
		return dead;
	}

	public void reset() {
		hasSeenPlayer = false;

		transform.rotation = startDirrection;

		if (!dead) return;
		dead = false;

		Game.enemies++;

		particle.GetComponent<ParticleSystem>().Pause();
		particle.GetComponent<ParticleSystem>().Clear();

		sr.sprite = idelSprite;

		bc.enabled = true;

		Destroy(transform.Find("Blood(Clone)").gameObject);
	}

	void OnTriggerStay2D(Collider2D col) {
		if (dead) return;
		if (col.tag.Equals("Player") || (col.tag.Equals("Enemy") && col.gameObject.GetComponent<EnemyAI>().isDead())) {
			Vector3 dir = col.transform.position - transform.position;
			float dot = Vector2.Dot(new Vector2(transform.up.x, transform.up.y).normalized, new Vector2(dir.x, dir.y).normalized);
			if (dot < 0) return;

			dot = 1 - dot;
			dot *= 180;

			if (dot < FOV) {
				RaycastHit2D hit = Physics2D.Linecast(transform.position, player.transform.position);
				if (hit.transform == player.transform && hit.distance < 6) {
					hasSeenPlayer = true;
				}
			}
		}
	}
}
