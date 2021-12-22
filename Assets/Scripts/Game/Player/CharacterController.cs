using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Cinemachine;

[RequireComponent(typeof(PlayerInstantiation))]
public class CharacterController : MonoBehaviourPun
{
	[Header("Movement")]
	[SerializeField]
	float BaseSpeed = 1.5f;
	[SerializeField]
	float SpeedMultiplierValue = 0.5f;
	[SerializeField]
	float rayLength = 1.4f;
	[SerializeField]
	float rayOffsetX = 0.5f;
	[SerializeField]
	float rayOffsetY = 0.5f;
	[SerializeField]
	float rayOffsetZ = 0.5f;

	[Header("Bomb")]
	public GameObject BombObj;
	public GameObject SpikeBomb;

	[Header("Debug")]
	public float turnSmoothness = 0.1f;
	public float targetAngle;

	[HideInInspector]
	public Vector3 targetPosition;
	Vector3 startPosition;
	bool moving;
	float turnSmoothVelocity;
	private GameObject BombContainer;

	void Start()
    {
		BombContainer = GameObject.Find("BombContainer");
	}
    void Update()
	{
		if (!photonView.IsMine)
			return;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (GetComponent<PlayerInstantiation>().CanBomb())
				photonView.RPC("SpawnBomb", RpcTarget.AllBuffered);
		}

		if (moving)
		{
			if (Vector3.Distance(startPosition, transform.position) > 1f)
			{
				transform.position = targetPosition;
				moving = false;
				ProcessMovement();
				return;
			}

			transform.position += (targetPosition - startPosition).normalized * (BaseSpeed + GetComponent<PlayerInstantiation>().SpeedMultiplier * SpeedMultiplierValue) * Time.deltaTime;
			return;
		}

		DrawDebugLine();
		ProcessMovement();
	}
	bool CanMove(Vector3 direction)
	{
		if (Vector3.Equals(Vector3.forward, direction) || Vector3.Equals(Vector3.back, direction))
		{
			RaycastHit info;
			if (Physics.Raycast(transform.position + Vector3.up * rayOffsetY + Vector3.right * rayOffsetX, direction, out info, rayLength))
            {
				return info.collider.isTrigger ? true : false;
			}
			if (Physics.Raycast(transform.position + Vector3.up * rayOffsetY - Vector3.right * rayOffsetX, direction, out info, rayLength))
			{
				return info.collider.isTrigger ? true : false;
			}
		}
		else if (Vector3.Equals(Vector3.left, direction) || Vector3.Equals(Vector3.right, direction))
		{
			RaycastHit info;
			if (Physics.Raycast(transform.position + Vector3.up * rayOffsetY + Vector3.forward * rayOffsetZ, direction, out info, rayLength))
			{
				return info.collider.isTrigger ? true : false;
			}
			if (Physics.Raycast(transform.position + Vector3.up * rayOffsetY - Vector3.forward * rayOffsetZ, direction, out info, rayLength))
			{
				return info.collider.isTrigger ? true : false;
			}
		}
		return true;
	}

	void TurnDirection(Vector3 Direction)
    {
		if (Direction.magnitude >= 0.1f)
		{
			Vector3 DirVec = Direction.normalized;
			targetAngle = Mathf.Atan2(DirVec.x, DirVec.z) * Mathf.Rad2Deg;
		}

		float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothness);
		transform.rotation = Quaternion.Euler(0, targetAngle, 0);
	}

	void ProcessMovement()
    {
		object start;
		PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameManager.ROOM_GAME_START, out start);
		if (!(bool)start)
			return;

		if (Input.GetKey(KeyCode.W))
		{
			if (CanMove(Vector3.forward))
			{
				targetPosition = transform.position + Vector3.forward;
				startPosition = transform.position;
				moving = true;
			}

			TurnDirection(Vector3.forward);
		}
		else if (Input.GetKey(KeyCode.S))
		{
			if (CanMove(Vector3.back))
			{
				targetPosition = transform.position + Vector3.back;
				startPosition = transform.position;
				moving = true;
			}

			TurnDirection(Vector3.back);
		}
		else if (Input.GetKey(KeyCode.A))
		{
			if (CanMove(Vector3.left))
			{
				targetPosition = transform.position + Vector3.left;
				startPosition = transform.position;
				moving = true;
			}

			TurnDirection(Vector3.left);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			if (CanMove(Vector3.right))
			{
				targetPosition = transform.position + Vector3.right;
				startPosition = transform.position;
				moving = true;
			}

			TurnDirection(Vector3.right);
		}
		
		
		if(Input.GetKeyDown(KeyCode.LeftShift))
        {
			if(GetComponent<PlayerInstantiation>().CanPunch)
            {
				Vector3 DirectionVector = transform.forward;

				RaycastHit hit;
				Ray ray = new Ray(targetPosition + Vector3.up * rayOffsetY, DirectionVector);
				Debug.Log(ray);
				if (Physics.Raycast(ray, out hit, 2f, LayerMask.GetMask("Bomb")))
				{
					hit.transform.gameObject.GetComponent<BombBehaviour>().Velocity = DirectionVector * 10;
				}
			}
        }
	}

	void DrawDebugLine()
    {
		Debug.DrawLine(transform.position + Vector3.up * rayOffsetY + Vector3.right * rayOffsetX, transform.position + Vector3.up * rayOffsetY + Vector3.right * rayOffsetX + Vector3.forward * rayLength, Color.red, Time.deltaTime);
		Debug.DrawLine(transform.position + Vector3.up * rayOffsetY - Vector3.right * rayOffsetX, transform.position + Vector3.up * rayOffsetY - Vector3.right * rayOffsetX + Vector3.forward * rayLength, Color.red, Time.deltaTime);

		Debug.DrawLine(transform.position + Vector3.up * rayOffsetY + Vector3.right * rayOffsetX, transform.position + Vector3.up * rayOffsetY + Vector3.right * rayOffsetX + Vector3.back * rayLength, Color.red, Time.deltaTime);
		Debug.DrawLine(transform.position + Vector3.up * rayOffsetY - Vector3.right * rayOffsetX, transform.position + Vector3.up * rayOffsetY - Vector3.right * rayOffsetX + Vector3.back * rayLength, Color.red, Time.deltaTime);

		Debug.DrawLine(transform.position + Vector3.up * rayOffsetY + Vector3.forward * rayOffsetZ, transform.position + Vector3.up * rayOffsetY + Vector3.forward * rayOffsetZ + Vector3.left * rayLength, Color.red, Time.deltaTime);
		Debug.DrawLine(transform.position + Vector3.up * rayOffsetY - Vector3.forward * rayOffsetZ, transform.position + Vector3.up * rayOffsetY - Vector3.forward * rayOffsetZ + Vector3.left * rayLength, Color.red, Time.deltaTime);

		Debug.DrawLine(transform.position + Vector3.up * rayOffsetY + Vector3.forward * rayOffsetZ, transform.position + Vector3.up * rayOffsetY + Vector3.forward * rayOffsetZ + Vector3.right * rayLength, Color.red, Time.deltaTime);
		Debug.DrawLine(transform.position + Vector3.up * rayOffsetY - Vector3.forward * rayOffsetZ, transform.position + Vector3.up * rayOffsetY - Vector3.forward * rayOffsetZ + Vector3.right * rayLength, Color.red, Time.deltaTime);

	}

	[PunRPC]
	void SpawnBomb()
	{
		GetComponent<PlayerInstantiation>().CurrentBombUsed++;
		if (!photonView.IsMine)
			return;

		Vector3 position = RoundVector3(transform.position);
		bool available = true;
		foreach (Transform otherObj in BombContainer.GetComponentInChildren<Transform>())
		{
			if (position == otherObj.localPosition)
			{
				available = false;
			}
		}

		if (available)
		{
			GameObject bomb = PhotonNetwork.Instantiate(GetComponent<PlayerInstantiation>().Penetrative ? SpikeBomb.name : BombObj.name, position, Quaternion.identity);
		}
	}

	Vector3 RoundVector3(Vector3 target)
	{
		return new Vector3(Mathf.RoundToInt(target.x), Mathf.RoundToInt(target.y), Mathf.RoundToInt(target.z));
	}
}
