using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolygonArsenal
{

	public class PolygonBeamStatic : MonoBehaviour
	{

		[Header("Prefabs")]
		public GameObject beamLineRendererPrefab; //Put a prefab with a line renderer onto here.
		public GameObject beamStartPrefab; //This is a prefab that is put at the start of the beam.
		public GameObject beamEndPrefab; //Prefab put at end of beam.

		private GameObject beamStart;
		private GameObject beamEnd;
		private GameObject beam;
		private LineRenderer line;

		[Header("Beam Options")]
		public Transform target;
		public bool beamCollides = true; //Beam stops at colliders
		public float beamLength = 100; //Ingame beam length
		public float beamEndOffset = 0f; //How far from the raycast hit point the end effect is positioned
		public float textureScrollSpeed = 0f; //How fast the texture scrolls along the beam, can be negative or positive.
		public float textureLengthScale = 1f;   //Set this to the horizontal length of your texture relative to the vertical. Example: if texture is 200 pixels in height and 600 in length, set this to 3

		[Header("Width Pulse Options")]
		public float widthMultiplier = 1.5f;
		private float customWidth;
		private float originalWidth;
		private float lerpValue = 0.0f;
		public float pulseSpeed = 1.0f;
		private bool pulseExpanding = true;

		void Start()
		{
			SpawnBeam();
			originalWidth = line.startWidth;
			customWidth = originalWidth * widthMultiplier;
		}

		void FixedUpdate()
		{
			if (beam)
			{
				line.SetPosition(0, transform.position);

				Vector3 end;

				// Có target => beam luôn nối tới target
				if (target != null)
				{
					Collider col = target.GetComponent<Collider>();

					if (col != null)
					{
						end = col.bounds.center;
					}
					else
					{
						end = target.position;
					}
				}
				else
				{
					end = transform.position + transform.forward * beamLength;

					if (beamCollides)
					{
						RaycastHit hit;

						if (Physics.Raycast(
							transform.position,
							transform.forward,
							out hit,
							beamLength))
						{
							end = hit.point - (transform.forward * beamEndOffset);
						}
					}
				}

				line.SetPosition(1, end);

				if (beamStart != null)
				{
					beamStart.transform.position = transform.position;
					beamStart.transform.LookAt(end);
				}

				if (beamEnd != null)
				{
					beamEnd.transform.position = end;
					beamEnd.transform.LookAt(transform.position);
				}

				float distance = Vector3.Distance(transform.position, end);

				line.material.mainTextureScale =
					new Vector2(distance / textureLengthScale, 1);

				line.material.mainTextureOffset -=
					new Vector2(Time.deltaTime * textureScrollSpeed, 0);
			}

			// Pulse width
			if (pulseExpanding)
			{
				lerpValue += Time.deltaTime * pulseSpeed;
			}
			else
			{
				lerpValue -= Time.deltaTime * pulseSpeed;
			}

			if (lerpValue >= 1f)
			{
				pulseExpanding = false;
				lerpValue = 1f;
			}
			else if (lerpValue <= 0f)
			{
				pulseExpanding = true;
				lerpValue = 0f;
			}

			float currentWidth = Mathf.Lerp(
				originalWidth,
				customWidth,
				Mathf.Sin(lerpValue * Mathf.PI));

			line.startWidth = currentWidth;
			line.endWidth = currentWidth;
		}

		public void SpawnBeam()
		{
			if (beamLineRendererPrefab)
			{
				beam = Instantiate(beamLineRendererPrefab);
				beam.transform.position = transform.position;
				beam.transform.parent = transform;
				beam.transform.rotation = transform.rotation;

				line = beam.GetComponent<LineRenderer>();
				line.useWorldSpace = true;

#if UNITY_5_5_OR_NEWER
				line.positionCount = 2;
#else
			line.SetVertexCount(2); 
#endif

				beamStart = beamStartPrefab ? Instantiate(beamStartPrefab, beam.transform) : null;
				beamEnd = beamEndPrefab ? Instantiate(beamEndPrefab, beam.transform) : null;
			}
			else
			{
				Debug.LogError("A prefab with a line renderer must be assigned to the `beamLineRendererPrefab` field in the PolygonArsenalBeamStatic script on " + gameObject.name);
			}
		}

	}
}