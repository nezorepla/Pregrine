<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PregrineData.aspx.cs" Inherits="PregrineData" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div>
        <asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:Button ID="Import" runat="server" Text="Import" onclick="Import_Click" />
        <br />
        <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
    </div>
    </div>
    </form>
</body>
</html>
