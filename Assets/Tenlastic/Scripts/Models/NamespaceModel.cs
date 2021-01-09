using System;

namespace Tenlastic {
    [Serializable]
    public class NamespaceModel {

        [Serializable]
        public struct Key {
            public string description;
            public string[] roles;
            public string value;
        }

        [Serializable]
        public struct User {
            public string _id;
            public string[] roles;
        }

        public string _id;
        public DateTime createdAt;
        public Key[] keys;
        public string name;
        public DateTime updatedAt;
        public User[] users;

    }
}
