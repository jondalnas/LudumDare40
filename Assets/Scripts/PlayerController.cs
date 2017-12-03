using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	Rigidbody2D rb;
	Animator anim;

	private Vector3 startPosition;
	private bool dead;

	public float movementSpeed = 20;
	public float walkingDeadZone = 0.1f;
	public float shootingCooldownTime = 0.1f;
	public float bulletGainTime = 1f;
	public GameObject bullet;
	public Transform barrel;
	public GameObject resetText;
	public GameObject particle;
	public GameObject blood;
	public AudioClip[] sounds;
	
	void Start() {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		startPosition = transform.position;
		resetText = GameObject.Find("Canvas").transform.Find("Reset").gameObject;
	}
	
	float shootingCooldownTimer;
	float bulletGainTimer;
	Vector2 move;
	void Update() {
		if (Game.paused || Game.clearedLevel) return;

		//Reset
		if (Input.GetButtonDown("Reset")) {
			reset();
			return;
		}

		if (particle.GetComponent<ParticleSystem>().particleCount >= 400)
			particle.GetComponent<ParticleSystem>().Pause();

		if (dead) return;

		//Movement
		move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

		//Looking
		Vector2 mousePostion = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		mousePostion -= new Vector2(0.5f, 0.5f);
		mousePostion += mousePostion;
		transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(mousePostion.y, mousePostion.x)-180);

		//Shooting
		shootingCooldownTimer += Time.deltaTime;
		if (Input.GetButtonDown("Shoot") && shootingCooldownTime < shootingCooldownTimer && Game.bullets > 0) {
			shootingCooldownTimer = 0;
			Game.bullets--;

			Vector3 playerRotation = transform.rotation.eulerAngles;
			playerRotation.z += 90;
			GameObject.Instantiate(bullet, barrel.position, Quaternion.Euler(playerRotation));
			anim.SetBool("Shooting Gun", true);

			GetComponent<AudioSource>().clip = sounds[1];
			GetComponent<AudioSource>().Play();
		} else {
			anim.SetBool("Shooting Gun", false);
		}

		//Camera
		Vector3 playerPos = transform.position;
		playerPos.z = Camera.main.transform.position.z;
		Camera.main.transform.position = playerPos;

		//Bullet Gain
		bulletGainTimer += Time.deltaTime;
		if (bulletGainTimer > bulletGainTime) {
			bulletGainTimer = 0;
			Game.bullets++;
		}
	}

	void FixedUpdate() {
		if (dead) return;

		//Movement
		anim.SetBool("Walking", move.sqrMagnitude >= walkingDeadZone);
		rb.velocity = move * movementSpeed / (Game.bullets / 3f + 1);
		move = Vector3.zero;
	}

	public void die(Quaternion bulletDirrection) {
		dead = true;
		rb.simulated = false;
		transform.Find("Outer Player").GetComponent<CircleCollider2D>().gameObject.SetActive(false);
		particle.GetComponent<ParticleSystem>().Play();
		resetText.SetActive(true);
		anim.SetBool("Dead", true);

		Transform sr = transform.Find("Player_Render");
		transform.rotation = bulletDirrection;
		sr.localEulerAngles = new Vector3(0, 0, 0);
		sr.localPosition = Vector3.up*0.7f;

		GameObject bloodgo = GameObject.Instantiate(blood, sr);
		bloodgo.transform.localPosition = new Vector3(0, 0, 0.02f);

		GetComponent<AudioSource>().clip = sounds[0];
		GetComponent<AudioSource>().Play();
	}

	public bool isDead() {
		return dead;
	}

	public void reset() {
		dead = false;
		rb.simulated = true;
		transform.Find("Outer Player").GetComponent<CircleCollider2D>().gameObject.SetActive(true);
		resetText.SetActive(false);
		anim.SetBool("Dead", false);
		particle.GetComponent<ParticleSystem>().Pause();
		particle.GetComponent<ParticleSystem>().Clear();

		if (transform.Find("Player_Render").Find("Blood(Clone)"))
			Destroy(transform.Find("Player_Render").Find("Blood(Clone)").gameObject);

		Transform sr = transform.Find("Player_Render");
		sr.localEulerAngles = new Vector3(0, 0, 90);
		sr.localPosition = Vector3.zero;

		Game.bullets = 0;

		transform.position = startPosition;

		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

		for (int i = 0; i < enemies.Length; i++) {
			GameObject enemy = enemies[i];

			enemy.GetComponent<EnemyAI>().reset();
		}

		GameObject[] bulletList = GameObject.FindGameObjectsWithTag("Bullet");

		for (int i = 0; i < bulletList.Length; i++) {
			GameObject bullet = bulletList[i];
			Destroy(bullet);
		}
	}
}
