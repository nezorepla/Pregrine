<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type='text/javascript' src='http://code.jquery.com/jquery-1.11.2.min.js'></script>
     
    <style>
        /* Table styles */
        table {
            border-spacing: 0 0;
            border-collapse: collapse;
            font-size: 10pt;
        }

            table th {
                background: #E7E7E8;
                text-align: left;
                text-decoration: none;
                font-weight: normal;
                padding: 3px 6px 3px 6px;
            }

            table td {
                vertical-align: top;
                padding: 3px 6px 5px 5px;
                margin: 0px;
                border: 1px solid #E7E7E8;
                background: #F7F7F8;
            }

        .auto-style1 {
            width: 400px;
        }
    </style>
<script>
  $(function() {
         $("#txtBitis").attr("placeholder", "GUN/AY/YIL");
        $("#txtBasla").attr("placeholder", "GUN/AY/YIL");
});</script></head>
<body>
    
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="lblWarning" runat="server"></asp:Label>
            <table id="tblGet" runat="server">
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Button ID="btnGetIM" runat="server" Text="Sıradan Kayıt Getir" OnClick="btnGetIM_Click" /></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <hr />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtIMSearch" runat="server"></asp:TextBox></td>
                                            <td>
                                                <asp:Button ID="btnIMSearch" runat="server" Text="IM ARA" OnClick="btnIMSearch_Click" /></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <table style="margin-left:30px"><tr><td>Başlangıç:</td><td>
                            <asp:TextBox ID="txtBasla" CssClass="date" runat="server"></asp:TextBox></td>
                            <td>Bitiş:</td><td>
                            <asp:TextBox ID="txtBitis" CssClass="date" runat="server"></asp:TextBox></td></tr>
                            <tr><td colspan="4" >
                                <asp:Button ID="btnRapor" runat="server" Text="HAZIRLA" OnClick="btnRapor_Click" /></td></tr>
                        </table>
                    </td>
                </tr>
                <tr><td colspan="2"><h3>ONLINE AKSIYONLAR</h3>
                    <asp:Label ID="lblRapor" runat="server" ></asp:Label></td></tr>
            </table>
            <table id="tblPREGRINE" runat="server">
                <tr>
                    <td style="vertical-align: top;" class="auto-style1">
                        <asp:Label ID="lblPregrineArea" runat="server"></asp:Label>
                    </td>
                    <td style="vertical-align: top;">

                        <table>
                            <tr>
                                <th colspan="2"></th>
                            </tr>
                            <tr>
                                <td>MEMO</td>
                                <td>
                                    <asp:TextBox ID="txtMemo" runat="server" TextMode="MultiLine" Height="60px" Width="300px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <th colspan="2"></th>
                            </tr>
                            <tr>
                                <td>Aksiyon</td>
                                <td>
                                    <asp:DropDownList ID="ddlAksLst" runat="server" Height="17px" Width="300px"></asp:DropDownList></td>
                            </tr>
                            <tr>
                                <th colspan="2"></th>
                            </tr> <tr>
                               <td>CALL BACK</td>
                                <td>
                                    <asp:TextBox ID="txtCallBack" runat="server" Text="2015-04-12 23:28:26.457"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Button ID="btnSendAks" runat="server" Text="Kaydet" OnClick="btnSendAks_Click" />
                                    <asp:Button ID="btnExit" runat="server" Text="Aksiyonsuz Çıkış" OnClick="btnExit_Click" />
                                </td>
                            </tr>

                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <hr />
                        GECMIS MEMOLARI<asp:Label ID="lblHistory" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>

