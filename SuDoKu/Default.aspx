<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SuDoKu._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SuDoKu</title>
    <meta name="viewport" content="width=device-width" />
    <link rel="Stylesheet" type="text/css" href="SuDoKuStyleSheet.css" />
    <script type="text/javascript" src="Scripts/jquery.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="gridScriptManager" runat="server" />
    <asp:UpdatePanel ID="pnlSuDoKuGrid" runat="server">
        <ContentTemplate>
            <table id="SuDoKuGrid" runat="server">
            </table>
            <div id="SuDoKuControls">
                <asp:Button ID="btnNewGame" Text="New" runat="server" OnClick="btnNewGame_OnClick" />
                <input type="button" id="btnReset" value="Reset" />
                <input type="button" id="btnFlag" value="Flag" />
                <input type="button" id="btnUndo" value="Undo" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
    <script type="text/javascript" src="Scripts/SuDoKu.js"></script>
</body>
</html>
