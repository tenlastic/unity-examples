using Mirror;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Tenlastic;
using UnityEngine;

public class SecureAuthenticator : NetworkAuthenticator {

    public GroupService groupService;
    public LoginService loginService;

    public struct AuthRequestMessage : NetworkMessage {
        public string accessToken;
        public string groupId;
    }

    public struct AuthResponseMessage : NetworkMessage {
        public int code;
        public string message;
    }

    public override void OnStartServer() {
        NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
    }

    public override void OnStartClient() {
        NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
    }

    public override void OnServerAuthenticate(NetworkConnection conn) { }

    public override void OnClientAuthenticate(NetworkConnection conn) {
        string accessToken = TokenManager.singleton.accessToken;
        string groupId = EnvironmentVariables.singleton.groupId;

        AuthRequestMessage message = new AuthRequestMessage {
            accessToken = accessToken,
            groupId = groupId,
        };

        conn.Send(message);
    }

    public async void OnAuthRequestMessage(NetworkConnection conn, AuthRequestMessage msg) {
        bool isValid = await loginService.Validate(msg.accessToken);
        if (!isValid) {
            StartCoroutine(DelayedDisconnect(conn, "Invalid access token."));
            return;
        }

        Jwt jwt = new Jwt(msg.accessToken);

        string[] allowedUserIds = GameState.singleton.gameServerModel.allowedUserIds;
        if (allowedUserIds.Length > 0 && !allowedUserIds.Contains(jwt.payload.user._id)) {
            StartCoroutine(DelayedDisconnect(conn, "User not allowed."));
            return;
        }
        Debug.Log("User is allowed.");

        // Set authenticationData for other scripts to use.
        GroupModel groupModel = await GetGroupModel(msg.groupId, jwt.payload.user._id);
        conn.authenticationData = new AuthenticationData {
            groupModel = groupModel,
            userModel = jwt.payload.user
        };

        // Create and send msg to client so it knows to proceed.
        AuthResponseMessage authResponseMessage = new AuthResponseMessage {
            code = 200,
            message = "Success"
        };
        conn.Send(authResponseMessage);

        OnServerAuthenticated.Invoke(conn);
    }

    public IEnumerator DelayedDisconnect(NetworkConnection conn, string message) {
        // create and send msg to client so it knows to disconnect
        AuthResponseMessage authResponseMessage = new AuthResponseMessage {
            code = 401,
            message = message
        };

        conn.Send(authResponseMessage);
        conn.isAuthenticated = false;

        yield return new WaitForSeconds(1f);

        conn.Disconnect();
    }

    public void OnAuthResponseMessage(NetworkConnection conn, AuthResponseMessage msg) {
        if (msg.code == 200) {
            OnClientAuthenticated.Invoke(conn);
        } else {
            conn.isAuthenticated = false;
            conn.Disconnect();
        }
    }

    private async Task<GroupModel> GetGroupModel(string groupId, string userId) {
        if (string.IsNullOrEmpty(groupId)) {
            return null;
        }

        GroupModel groupModel = await groupService.FindRecordById(groupId);
        return groupModel.userIds.Contains(userId) ? groupModel : null;
    }
}
