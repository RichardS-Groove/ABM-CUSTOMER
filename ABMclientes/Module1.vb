Imports System.IO
Imports System.Data.SqlClient
Imports System.Net.Mail

Module Module1
    Function leerarchivo(ByVal archivo As String) As String
        If File.Exists("c:\ABM\ip.txt") = True Then
            Dim SR As StreamReader = File.OpenText("c:\ABM\ip.txt")
            Dim Line As String = SR.ReadLine()
            SR.Close()
            Return Line

        End If
    End Function
    Function YaExisteSQL(ByVal sql As String) As Boolean
        Dim ar As String
        Dim con As New SqlConnection("data source=" & CStr(leerarchivo(ar)) & "; initial catalog=Northwind; integrated security=true")
        Dim da1 As New SqlDataAdapter(sql, con)
        Dim ds1 As New DataSet
        da1.Fill(ds1, "afidesc")
        If ds1.Tables("afidesc").Rows.Count < 1 Then
            Return False
        Else
            Return True
        End If
    End Function
    Function VNum(ByVal NTexto As String) As Decimal

        Return CDec(Val(NTexto.Trim.Replace(",", ".")))

    End Function
    Function SQL_Accion(ByVal Sql_de_accion As String) As Boolean
        Dim ar As String
        Dim conn As New SqlConnection("data source=" & CStr(leerarchivo(ar)) & "; initial catalog=Northwind; integrated security=true")

        Dim adapter As New SqlDataAdapter, salida As Boolean = True

        Try
            conn.Open()
            If Sql_de_accion.ToUpper.IndexOf("INSERT") Then
                adapter.InsertCommand = New SqlCommand(Sql_de_accion, conn)
                adapter.InsertCommand.ExecuteNonQuery()
            Else
                If Sql_de_accion.ToUpper.IndexOf("UPDATE") Then
                    adapter.UpdateCommand = New SqlCommand(Sql_de_accion, conn)
                    adapter.UpdateCommand.ExecuteNonQuery()
                Else
                    If Sql_de_accion.ToUpper.IndexOf("DELETE") Then
                        adapter.DeleteCommand = New SqlCommand(Sql_de_accion, conn)
                        adapter.DeleteCommand.ExecuteNonQuery()
                    Else
                        salida = False
                    End If
                End If
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
            salida = False
        End Try
        conn.Close()
        Return salida
    End Function
    Function NumSQL(ByVal numero As String) As String
        Return CStr(VNum(numero)).Trim.Replace(",", ".")
    End Function
    Function RellenaNum(ByVal numero As Integer, ByVal cantidad As Integer) As String
        Dim snum As String = CStr(numero).Trim
        Return "00000000000000000000".Substring(0, cantidad - snum.Length) & snum
    End Function
    Function FechaSQL(ByVal fecha As Date) As String
        Return "'" & RellenaNum(Year(fecha), 4) & RellenaNum(Month(fecha), 2) & RellenaNum(fecha.Day, 2) & "'"

    End Function

End Module

