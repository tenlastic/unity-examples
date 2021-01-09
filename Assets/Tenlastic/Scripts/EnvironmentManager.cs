using UnityEngine;

namespace Tenlastic {
    public class EnvironmentManager : MonoBehaviour {

        public static EnvironmentManager singleton;

        public EnvironmentObject environmentObject;

        private void Awake() {
            if (singleton == null) {
                singleton = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(this);
            }
        }

        public void SetEnvironmentObject(EnvironmentObject environmentObject) {
            this.environmentObject = environmentObject;
        }

    }
}
