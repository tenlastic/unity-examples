using UnityEngine;

namespace Tenlastic {
    [CreateAssetMenu(fileName = "Environment", menuName = "Tenlastic/Scriptable Objects/Environment")]
    public class EnvironmentObject : ScriptableObject {

        public string buildApiBaseUrl;
        public string collectionApiBaseUrl;
        public string gameServerApiBaseUrl;
        public string groupApiBaseUrl;
        public string loginApiBaseUrl;
        public string namespaceApiBaseUrl;
        public string namespaceId;
        public string publicKeyApiBaseUrl;
        public string userApiBaseUrl;

    }
}
