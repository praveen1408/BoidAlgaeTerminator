using UnityEngine;
using System.Collections;

public class ChlorineController : MonoBehaviour {
	private float killingPower;
	private ChlorineRegister registry;

	void Start () {
		registry = transform.parent.gameObject.GetComponent<ChlorineRegister>();
		registry.register(this);
		killingPower = registry.baseKillingPower;
		StartCoroutine("decay");		
	}

	IEnumerator decay(){
		int limit = (int) (registry.lifeTime * (1/registry.updateDelay));
		for(int i = 0; i < limit; i++){
			float t = (float)i/limit;

			// Decrease killingPower due to dilution
			killingPower = Mathf.Lerp(registry.baseKillingPower,0,t);
			
			// Increase size
			float scale = Mathf.Lerp(registry.startScale,registry.endScale,t);
			transform.localScale = new Vector3(scale,scale,scale);
			
			yield return new WaitForSeconds(registry.updateDelay);
		}

		registry.unregister(this);
		Destroy(gameObject);
	}

	void OnTriggerStay(Collider other){
		//Debug.Log(other.gameObject.name);
		if(other.CompareTag("TAG_ALGAE")){
			AlgaeController algae = other.gameObject.GetComponent<AlgaeController>();
			algae.lifeStrength -= killingPower;
		}
	}

}
