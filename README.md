# A Valve RCON protocol implementation in Visual Basic .NET
This is a simple implementation of the Valve RCON protocol in Visual Basic .NET 2019 Community Edition (Framework 4.7.2).
Whith this library/class you can connect/authenticate/send commands/receive responses from your gameserver.

**More info:** https://developer.valvesoftware.com/wiki/Source_RCON_Protocol

**TESTED WITH:** 
- Counter-Strike 2
- Counter-Strike: Global Offensive
- Counter-Strike: Source
- (Older versions are not guaranteed)
 
## Installation
- Navigate to https://github.com/fpaezf/Valve-RCON-protocol-in-VB.NET/releases and download the latest version.
- Import the **RconClient** class to your project or add a library reference.
- Add **rcon_password "your-password"** to your **server.cfg** file.

## Usage
```vbnet
Dim serverIP As String = "82.223.49.142" 'String to declare server IP or host
Dim serverPort As Integer = 27015 'Integer to declare server port
Dim serverPassword As String = "your-password" 'String to declare RCON password

Dim RCON As New RconClient 'Create a new RconClient
RCON.Connect(serverIP, serverPort) 'Connect to the server IP/HOST:PORT
RCON.Authorize(serverPassword) 'Send password to authenticate
   If RCON.Authorized = True And RCON.Connected = True Then 'If no errors, you can now send commands
      Dim a As String = RCON.SendCommand("status") 'Store in a string the server response to command "status"
      RichTextBox1.AppendText(a) 'Append server response to RichTextBox.Text
   End If
RCON.Close() 'Close connection to the server
```
## Data visualization
If you put the server output string in a TextBox control the lines will be concatenated because of the return carriage chars. To visualize data correctly please, use RichTextBox control. 

## Building RCON packets
Both requests and responses are sent as TCP packets. Their payload follows the following basic structure:
- Size (Integer)
- ID (Integer)
- Type (Integer)
- Body (null-terminated string encoded in ASCII)
- Empty String (Empty null-terminated string encoded in ASCII)

This is how i build the packet to request authorization:

```vbnet
Dim Packet As Byte() = New Byte(CByte((4 + 4 + 4 + password.Length + 1))) {} 'Size + ID + Type + Command + Null
Packet(0) = password.Length + 9         'Packet Size (Integer)
Packet(4) = 99                          'Request Id (Integer between 1 and 99)
Packet(8) = 3                           'Request packet type: 3 = SERVERDATA_AUTH request
For X As Integer = 0 To password.Length - 1
     Packet(12 + X) = System.Text.Encoding.UTF8.GetBytes(password(X))(0) 'Command
Next
```
