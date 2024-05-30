using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {
	
	public void DestroyMyself() {
		Destroy (gameObject);
	}

    public void DeactiveMyself()
    {
        gameObject.SetActive(false);
    }
}
