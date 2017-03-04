using UnityEngine;

public class AlgaeController : MonoBehaviour {
	public float lifeStrength = 100;

	private AlgaeRegister registry;	

	void Start(){
		registry = transform.parent.gameObject.GetComponent<AlgaeRegister>();
		registry.register(this);
	}

	void Update () {
		// Remove algae if it is dead
		if(lifeStrength <= 0){
			registry.unregister(this);
			Destroy(this.gameObject);
			return;
		}		
	}
}
