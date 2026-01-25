using Protocol;

var newMsg = new S_Login();

newMsg.PlayerId = (int)PacketID.PktSLogin;
newMsg.Success = true;