using System;

namespace Tenlastic {
    [Serializable]
    public class GameServerModel {

        [Serializable]
        public struct Metadata {
            public string scene;
        }

        public string _id;
        public string[] allowedUserIds;
        public string buildId;
        public float cpu;
        public DateTime createdAt;
        public string[] currentUserIds;
        public string description;
        public bool isPersistent;
        public bool isPreemptible;
        public float memory;
        public Metadata metadata;
        public string name;
        public string namespaceId;
        public int port;
        public string queueId;
        public string status;
        public DateTime updatedAt;

    }
}
