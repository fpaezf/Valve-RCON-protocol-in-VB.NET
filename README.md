# A Valve RCON protocol implementation in Visual Basic .NET
This is a simple implementation of the Valve RCON protocol in Visual Basic .NET 2019 Community Edition (Framework 4.7.2).
Whith this library/class you can connect/authenticate/send commands/receive responses from your gameserver.

**WORKS WITH:** 
- Counter-Strike 2
- Counter-Strike: Global Offensive
- Counter-Strike: Source
- (Older versions are not guaranteed)
 
## Installation
- Navigate to https://github.com/fpaezf/Valve-RCON-protocol-in-VB.NET/releases and download the latest version.
- Import the RconClient class or library to your project.

## Usage

```vbnet
Dim serverIP As String = "82.223.49.142"
Dim serverPort As Integer = 27015
Dim serverPassword As String = "Banana1983"

Dim RCON As New RconClient
RCON.Connect(serverIP, serverPort)
RCON.Authorize(serverPassword)
If RCON.Authorized = True And RCON.Connected = True Then
Dim a As String = RCON.SendCommand(TextBox1.Text)
RichTextBox1.AppendText(a)
End If
RCON.Close()
```
