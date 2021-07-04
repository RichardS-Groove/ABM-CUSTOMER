Imports System.IO
Imports System.Data.SqlClient
Public Class Form1
    Dim ar As String
    Dim con As New SqlConnection("data source=" & CStr(leerarchivo(ar)) & "; initial catalog=Northwind; integrated security=true")

    Function leerarchivo(ByVal archivo As String) As String
        If File.Exists("c:\ABM\ip.txt") = True Then
            Dim SR As StreamReader = File.OpenText("c:\ABM\ip.txt")
            Dim Line As String = SR.ReadLine()
            SR.Close()
            Return Line
        Else
            MsgBox("Verifique falta archivo de configuracion")
            Me.Close()
        End If
    End Function

    Private Sub ABM_Clientes_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        buscar(" apeynom like '" & tApellido.Text & "%' ")
    End Sub
    Sub buscar(ByVal condicion As String)

        Dim da As New SqlDataAdapter("SELECT TOP (100) PERCENT id,apeynom from Customers_búsqueda where " & condicion & " order by apeynom", con)
        Dim ds As New DataSet
        da.Fill(ds, "Customers")
        If ds.Tables("Customers").Rows.Count = 0 Then

            DataGridView1.Visible = False
            pBotones.Visible = False
            pCampos.Visible = False
            lLegajo.Visible = False
        Else

            DataGridView1.DataSource = ds.Tables("Customers")
            DataGridView1.Refresh()
            DataGridView1.Visible = True
            lLegajo.Visible = True
        End If

    End Sub
    Private Sub DataGridView1_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        FilaClick(e)
    End Sub

    Private Sub DataGridView1_RowEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.RowEnter
        FilaClick(e)
    End Sub
    Sub FilaClick(ByVal e As Object)
        Dim fila As Integer = e.RowIndex
        Dim tfila As String

        If IsNothing(DataGridView1.Rows(fila).Cells(0).Value) Then
            lLegajo.Text = "0"
            pBotones.Visible = False
            pCampos.Visible = False
            Exit Sub
        Else
            tfila = DataGridView1.Rows(fila).Cells(0).Value
            lLegajo.Text = tfila.ToString()
            CargarCamposClientes()
        End If

    End Sub

    Sub CargarCamposClientes()
        If Val(lLegajo.Text) = 0 Then
            pBotones.Visible = False
            pCampos.Visible = False

            Exit Sub
        Else
            pBotones.Visible = True
            pCampos.Visible = True
            Dim da As New SqlDataAdapter("SELECT  upper(ltrim(rtrim(isnull(companyname,'****')))) as nombrecompania, upper(ltrim(rtrim(isnull(contactname,'****')))) as nombrecontacto, ltrim(rtrim(isnull(contacttitle,''))) as títulocontacto, ltrim(rtrim(isnull(address,''))) as dirección, ltrim(rtrim(isnull(city,''))) as localidad,ltrim(rtrim(isnull(region,''))) as provincia,ltrim(rtrim(isnull(country,''))) as país,ltrim(rtrim(isnull(postalcode,''))) as códigopostal, ltrim(rtrim(isnull(phone,''))) as teléfonos, ltrim(rtrim(isnull(fax,''))) as fax from customers where id=" & Val(lLegajo.Text), con)
            Dim ds As New DataSet
            da.Fill(ds, "Customers")
            TextBox1.Text = ds.Tables("Customers").Rows(0)("nombrecompania")
            TextBox2.Text = ds.Tables("Customers").Rows(0)("nombrecontacto")
            TextBox3.Text = ds.Tables("Customers").Rows(0)("títulocontacto")
            TextBox4.Text = ds.Tables("Customers").Rows(0)("dirección")
            TextBox5.Text = ds.Tables("Customers").Rows(0)("localidad")
            TextBox8.Text = ds.Tables("Customers").Rows(0)("provincia")
            TextBox6.Text = ds.Tables("Customers").Rows(0)("códigopostal")
            TextBox7.Text = ds.Tables("Customers").Rows(0)("teléfonos")
            TextBox12.Text = ds.Tables("Customers").Rows(0)("fax")
        End If
    End Sub

    Private Sub bBuscar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bBuscar.Click
        buscar(" apeynom like '" & tApellido.Text & "%' ")
    End Sub

    Private Sub PictureBox8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox8.Click
        tApellido.Text = ""
        buscar(" apeynom like '" & tApellido.Text & "%' ")
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

        If MessageBox.Show("Está por ELIMINAR definitivamente al cliente: " & TextBox1.Text.Trim.ToUpper & ", " & TextBox12.Text.Trim.ToUpper & ". Está SEGURO?", "Eliminar cliente", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then Exit Sub

        If SQL_Accion("delete from Customers  where  id=" & Val(lLegajo.Text)) = False Then
            MsgBox("Hubo un error al intentar borrar al cliente, reintente, y si el error persiste, anote todos los datos que quizo ingresar y comuníquese con el programador.")
        Else

            buscar("id=" & Val(lLegajo.Text))
            MsgBox("El cliente fue ELIMINADO de la base de datos.")

        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim errores As String = "", en As String = vbCrLf
        If TextBox1.Text.Trim.Length < 3 Then
            errores &= "Debe completar el nombre de compania." & en
        End If
        If TextBox2.Text.Trim.Length < 3 Then
            errores &= "Debe completar el nombre de contacto." & en
        End If
        TextBox3.Text = Val(TextBox3.Text.Trim.Replace(".", "").Replace(" ", "").Replace(",", ""))
        If errores.Length > 0 Then
            MsgBox("Hubo errores, por favor verifique y corrija antes de intentar de nuevo:" & en & en & errores)
            Exit Sub
        End If
        If SQL_Accion("update customers set companyname='" & TextBox1.Text.Trim.ToUpper.Replace("'", "´") & "', contactname='" & TextBox2.Text.Trim.ToUpper.Replace("'", "´") & "', contacttitle=" & Val(TextBox3.Text.Trim.Replace(".", "").Replace(" ", "").Replace(",", "")) & ", address='" & TextBox4.Text.Trim.ToUpper.Replace("'", "´") & "', city='" & TextBox5.Text.Trim.ToUpper.Replace("'", "´") & "', region='" & TextBox8.Text.Trim.ToUpper.Replace("'", "´") & "', postalcode='" & TextBox6.Text.Trim.ToUpper.Replace("'", "´") & "', phone='" & TextBox7.Text.Trim.ToUpper.Replace("'", "´") & "', fax='" & TextBox12.Text.Trim.ToUpper.Replace("'", "´") & "' where id=" & VNum(lLegajo.Text)) = True Then
            MsgBox("Cambios realizados correctamente.")

            buscar(" id=" & VNum(lLegajo.Text))
        Else
            MsgBox("Se produjo un error al querer guardar los datos del cliente, reintente, y si el error persiste, anote todos los datos que quizo ingresar y comuníquese con el programador.")
        End If
    End Sub

    Private Sub bNuevoProfesor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bNuevoAlumno.Click
        If SQL_Accion("insert into customers (companyname, contactname, contacttitle, address, city, region, postalcode, phone, fax) values ('*****','*****',                  '',           '',           '',             '',           '',               ''       ,      ''    )  ") Then


            buscar(" apeynom like '****%' ")
            MsgBox("Se ha creado un nuevo registro para el cliente que desea ingresar, seleccione la línea nueva, cargue los datos y luego confirme con el botón 'Aceptar Cambios'.")
        End If
    End Sub

    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles Label6.Click

    End Sub
End Class