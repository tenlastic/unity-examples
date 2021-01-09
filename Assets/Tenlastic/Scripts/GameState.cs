using UnityEngine;

namespace Tenlastic {
	public class GameState : MonoBehaviour {

		public static GameState singleton;

		public GameServerModel gameServerModel;
		public UserModel userModel;

		void Awake() {
			if (singleton == null) {
				singleton = this;
				DontDestroyOnLoad(gameObject);
			} else {
				Destroy(gameObject);
			}
		}

	}
}