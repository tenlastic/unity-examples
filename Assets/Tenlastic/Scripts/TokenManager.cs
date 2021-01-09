using UnityEngine;

namespace Tenlastic {
    public class TokenManager {

        public static readonly TokenManager singleton = new TokenManager();

        public string accessToken {
            get {
                return PlayerPrefs.HasKey("TokenManager.accessToken") ? 
                    PlayerPrefs.GetString("TokenManager.accessToken") : 
                    null;
            }
            set {
                if (string.IsNullOrEmpty(value)) {
                    PlayerPrefs.DeleteKey("TokenManager.accessToken");
                } else {
                    PlayerPrefs.SetString("TokenManager.accessToken", value);
                }
            }
        }
        public string refreshToken {
            get {
                return PlayerPrefs.HasKey("TokenManager.refreshToken") ? 
                    PlayerPrefs.GetString("TokenManager.refreshToken") : 
                    null;
            }
            set {
                if (string.IsNullOrEmpty(value)) {
                    PlayerPrefs.DeleteKey("TokenManager.refreshToken");
                } else {
                    PlayerPrefs.SetString("TokenManager.refreshToken", value);
                }
            }
        }

        public void Clear() {
            accessToken = null;
            refreshToken = null;
        }

    }
}
