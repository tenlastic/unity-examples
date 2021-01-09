using System;

namespace Tenlastic {
    [Serializable]
    public class GroupModel {

        public string _id;
        public DateTime createdAt;
        public DateTime updatedAt;
        public string[] userIds;

    }
}
