Imports System.Net.Sockets
Imports System.Text

Public Class RCONClient
    Private _client As TcpClient
    Private _stream As NetworkStream
    Private _password As String

    Public Sub New(ip As String, port As Integer, password As String)
        _client = New TcpClient(ip, port)
        _stream = _client.GetStream()
        _password = password
    End Sub

    Public Function SendCommand(command As String) As String
        Dim commandBytes As Byte() = Encoding.ASCII.GetBytes(command)
        Dim packetLength As Integer = 4 + 4 + commandBytes.Length + 2
        Dim buffer(packetLength - 1) As Byte

        ' Packet length
        BitConverter.GetBytes(packetLength).CopyTo(buffer, 0)

        ' Request ID (set to 1 for simplicity)
        BitConverter.GetBytes(1).CopyTo(buffer, 4)

        ' Type (0 for server)
        BitConverter.GetBytes(0).CopyTo(buffer, 8)

        ' RCON command
        commandBytes.CopyTo(buffer, 12)

        ' Null-terminated string
        buffer(packetLength - 3) = 0
        buffer(packetLength - 2) = 0

        ' Send the packet to the server
        _stream.Write(buffer, 0, packetLength)

        ' Receive and parse the response
        Dim responseBuffer(4096) As Byte
        Dim bytesRead As Integer = _stream.Read(responseBuffer, 0, responseBuffer.Length)
        Dim response As String = Encoding.ASCII.GetString(responseBuffer, 12, bytesRead - 12)

        _stream.Close()
        _client.Close()

        Return response
    End Function

    'Public Sub Close()
    '_stream.Close()
    '_client.Close()
    'End Sub

End Class
