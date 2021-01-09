using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Tenlastic {
    public class EnvironmentVariables : MonoBehaviour {

        public static EnvironmentVariables singleton;

        public string accessToken {
            get {
                string key = "ACCESS_TOKEN";
                _accessToken = HasEnvironmentVariable(key) ? GetEnvironmentVariable(key) : _accessToken;
                return _accessToken;
            }
        }
        public GameServerModel gameServerModel {
            get {
                string key = "GAME_SERVER_JSON";
                string value = GetEnvironmentVariable(key);

                _gameServerModel = HasEnvironmentVariable(key) ? 
                    JsonConvert.DeserializeObject<GameServerModel>(value) :
                    _gameServerModel;

                return _gameServerModel;
            }
        }
        public string groupId {
            get {
                string key = "GROUP_ID";
                _groupId = HasEnvironmentVariable(key) ? GetEnvironmentVariable(key) : _groupId;
                return _groupId;
            }
        }
        public bool isServer {
            get {
                return string.IsNullOrEmpty(accessToken) && string.IsNullOrEmpty(refreshToken);
            }
        }
        public string refreshToken {
            get {
                string key = "REFRESH_TOKEN";
                _refreshToken = HasEnvironmentVariable(key) ? GetEnvironmentVariable(key) : _refreshToken;
                return _refreshToken;
            }
        }

        [SerializeField] private string _accessToken;
        [SerializeField] private GameServerModel _gameServerModel;
        [SerializeField] private string _groupId;
        [SerializeField] private string _refreshToken;

        private void Awake() {
            if (singleton == null) {
                singleton = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(this);
            }
        }

        protected string GetEnvironmentVariable(string key) {
            return Environment.GetEnvironmentVariable(key);
        }

        protected bool HasEnvironmentVariable(string key) {
            string value = GetEnvironmentVariable(key);
            return !string.IsNullOrEmpty(value);
        }

    }
}
