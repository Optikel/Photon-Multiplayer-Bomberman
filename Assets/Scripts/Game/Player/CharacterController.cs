using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Cinemachine;
public class CharacterController : MonoBehaviourPun
{
	[Header("Movement")]
	[SerializeField]
	float moveSpeed = 0.25f;
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
	[Range(0, 10)]
	public int MaxBombs;

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

		if (moving)
		{
			if (Vector3.Distance(startPosition, transform.position) > 1f)
			{
				transform.position = targetPosition;
				moving = false;
				return;
			}

			transform.position += (targetPosition - startPosition) * moveSpeed * Time.deltaTime;
			return;
		}

		DrawDebugLine();
		ProcessKeyboard();
		TurnDirection(targetPosition - startPosition);

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

	void ProcessKeyboard()
    {
		if (Input.GetKey(KeyCode.W))
		{
			if (CanMove(Vector3.forward))
			{
				targetPosition = transform.position + Vector3.forward;
				startPosition = transform.position;
				moving = true;
			}
		}
		else if (Input.GetKey(KeyCode.S))
		{
			if (CanMove(Vector3.back))
			{
				targetPosition = transform.position + Vector3.back;
				startPosition = transform.position;
				moving = true;
			}
		}
		else if (Input.GetKey(KeyCode.A))
		{
			if (CanMove(Vector3.left))
			{
				targetPosition = transform.position + Vector3.left;
				startPosition = transform.position;
				moving = true;
			}
		}
		else if (Input.GetKey(KeyCode.D))
		{
			if (CanMove(Vector3.right))
			{
				targetPosition = transform.position + Vector3.right;
				startPosition = transform.position;
				moving = true;
			}
		}
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			photonView.RPC("SpawnBomb", RpcTarget.All);
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
		if (!photonView.IsMine)
			return;

		float heightCompensation = GetComponent<BoxCollider>().size.y * 0.5f - BombObj.GetComponentInChildren<BoxCollider>().size.y * 0.5f;
		Vector3 v3_heightOffset = new Vector3(0, heightCompensation, 0);

		Vector3 position = targetPosition - v3_heightOffset;
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
			GameObject bomb = PhotonNetwork.Instantiate(BombObj.name, position, Quaternion.identity);
			bomb.GetPhotonView().RPC("AttachToContainer", RpcTarget.All);
			//bomb.transform.parent = GameObject.Find("BombContainer").transform;
		}
		//bomb.GetComponent<Collider>().enabled = false;
	}
}
