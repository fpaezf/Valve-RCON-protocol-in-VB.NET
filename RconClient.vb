Imports System.Net.Sockets
Imports System.Text

Public Class RCONClient

    Private Client As TcpClient
    Private stream As NetworkStream
    Public Connected As Boolean = False
    Public Authorized As Boolean = False

    Public Sub Connect(ByVal server As String, ByVal port As Integer)
        Try
            Client = New TcpClient(server, port)
            stream = Client.GetStream()
            Connected = True
        Catch ex As Exception
            Close()
            MsgBox("Could not connect to the server. Check ip/host and port.", vbCritical, "RCON.ErrConn")
        End Try
    End Sub

    Public Sub Authorize(ByVal password As String)
        If Connected = True Then
            Dim a As Boolean = SendAuthPacket(password)
            If a = True Then
                Authorized = True
            ElseIf a = False Then
                Authorized = False
                Close()
                MsgBox("Invalid password or rcon_password cvar is not set.")
            End If
        ElseIf Connected = False Then
            Authorized = False
            Close()
        End If
    End Sub

    Private Function SendAuthPacket(ByVal password As String) As Boolean
        If Connected = True Then
            Try
                Dim Packet As Byte() = New Byte(CByte((4 + 4 + 4 + password.Length + 1))) {}
                Packet(0) = password.Length + 9         'Packet Size (Integer)
                Packet(4) = 99                          'Request Id (Integer)
                Packet(8) = 3                           ' 3 = SERVERDATA_AUTH
                For X As Integer = 0 To password.Length - 1
                    Packet(12 + X) = System.Text.Encoding.UTF8.GetBytes(password(X))(0)
                Next
                stream.Write(Packet, 0, Packet.Length)
                Dim data As Byte() = New Byte(4096) {}
                Dim bytes As Integer
                Do
                    bytes = stream.Read(data, 0, data.Length)
                Loop While bytes = 0
                Dim result As Byte() = New Byte(bytes - 1) {}
                Array.Copy(data, 0, result, 0, bytes)
                Dim ID As Integer = BitConverter.ToInt32(data, 4)
                Dim Type As Integer = BitConverter.ToInt32(data, 8)
                If ID = 99 And Type = 2 Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
                Close()
            End Try
        Else
            Return False
            Close()
        End If
    End Function

    Public Function SendCommand(ByVal Command As String) As String
        If Connected = True And Authorized = True Then
            Try
                Dim Packet As Byte() = New Byte(CByte((4 + 4 + 4 + Command.Length + 1))) {}
                Packet(0) = Command.Length + 9          'Packet Size (Integer)
                Packet(4) = 99                          'Request Id (Integer)
                Packet(8) = 2                           '2 = SERVERDATA_EXECCOMMAND
                For X As Integer = 0 To Command.Length - 1
                    Packet(12 + X) = System.Text.Encoding.UTF8.GetBytes(Command(X))(0)
                Next
                stream.Write(Packet, 0, Packet.Length)
                Dim data As Byte() = New Byte(4096) {}
                Dim bytes As Integer
                Do
                    bytes = stream.Read(data, 0, data.Length)
                Loop While bytes = 0
                Dim result As Byte() = New Byte(bytes - 1) {}
                Array.Copy(data, 0, result, 0, bytes)
                Dim size As Integer = BitConverter.ToInt32(data, 0)
                Dim ID As Integer = BitConverter.ToInt32(data, 4)
                Dim Typex As Integer = BitConverter.ToInt32(data, 8)
                Dim Payload As String = Encoding.UTF8.GetString(data, 12, size - 10)
                'Dim returndata As String = "Packet size: " & size.ToString & vbCrLf & "Packet ID: " & ID.ToString & vbCrLf & "Packet type: " & Typex.ToString & vbCrLf & "Packet body: " & Payload
                'MsgBox(returndata)
                If ID = 99 And Typex = 0 Then
                    Return Payload
                Else
                    Return "Bad ID or Type" & vbCrLf & vbCrLf & "ID expected: 99, received: " & ID.ToString & vbCrLf & "Type expected: 0, received: " & Typex.ToString
                    Close()
                End If
            Catch ex As Exception
                Return ""
                Close()
            End Try
        Else
            Close()
            Return ""
        End If
    End Function

    Public Sub Close()
        Try
            If Client.Connected = True Then
                Client.Close()
                Client = Nothing
                stream.Close()
            End If
        Catch ex As Exception
        End Try
        Connected = False
        Authorized = False
    End Sub

End Class
