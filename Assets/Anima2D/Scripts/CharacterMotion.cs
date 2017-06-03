using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Anima2D 
{
public class CharacterMotion : MonoBehaviour
{
	Animator animator;
	private List<Transform> toReverse = new List<Transform>();

	void Start()
	{
		animator = GetComponent<Animator>();
		Bone2D[] bone2D = GetComponentsInChildren<Bone2D>();	
		for (int i=bone2D.Length - 1; i >= 0; i--){
			Transform t = bone2D[i].transform;
			if (t.localPosition.z != 0){
				toReverse.Add(t);				
			}
		}
	}

	void Update ()
	{
		float xAxis = Input.GetAxis("Horizontal");
		float yAxis = Input.GetAxis("Vertical");

		Vector3 eulerAngles = transform.localEulerAngles;

		bool rotate = false;
		if(xAxis > 0f && eulerAngles.y != 180f){
			eulerAngles.y = 180f;
			rotate = true;
		} else if(xAxis < 0f && eulerAngles.y != 0f) {
			eulerAngles.y = 0f;
			rotate = true;
		}
		animator.SetFloat("Down", Mathf.Abs(yAxis));
		animator.SetFloat("Forward", Mathf.Abs(xAxis));
		
		if (rotate){
			transform.localRotation = Quaternion.Euler(eulerAngles);
			foreach(Transform t in toReverse){
				Vector3 lp = t.localPosition;
				t.localPosition = new Vector3(lp.x, lp.y, -lp.z);				
			}
		}

	}
}
}