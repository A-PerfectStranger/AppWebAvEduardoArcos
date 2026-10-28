<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Song.aspx.cs" Inherits="WebAppPoliMusicV2.Song" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <h2>Songs</h2>
<form id="formSong" runat="server">
    <div>
        <fieldset>
            <legend>Search</legend>
            <table>
                <tr>
                    <td>Song name:<br /><asp:TextBox ID="txtSearch" runat="server"></asp:TextBox></td>
                    <td>
                        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
                        <asp:Button ID="btnShowAll" runat="server" Text="Show all" OnClick="btnShowAll_Click" />
                        <asp:Button ID="btnMostPlayed" runat="server" Text="Most played" OnClick="btnMostPlayed_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    <div>
        <asp:GridView ID="gvSongs" runat="server" AutoGenerateColumns="false" EmptyDataText="There are no songs registered."
            AllowPaging="true" PageSize="10" OnPageIndexChanging="gvSongs_PageIndexChanging"
            OnRowCommand="gvSongs_RowCommand" CellPadding="4" GridLines="Both">
            <Columns>
                <asp:BoundField DataField="ID_SONG" HeaderText="ID" />
                <asp:BoundField DataField="SONG_NAME" HeaderText="Song name" />
                <asp:BoundField DataField="SONG_PATH" HeaderText="Path" />
                <asp:BoundField DataField="PLAYS" HeaderText="Plays" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkPlay" runat="server" Text="Play" CausesValidation="false"
                            CommandName="PlaySong" CommandArgument='<%# Eval("ID_SONG") %>'></asp:LinkButton>
                        &nbsp;
                        <asp:LinkButton ID="lnkEdit" runat="server" Text="Edit" CausesValidation="false"
                            CommandName="EditSong" CommandArgument='<%# Eval("ID_SONG") %>'></asp:LinkButton>
                        &nbsp;
                        <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete" CausesValidation="false"
                            CommandName="DeleteSong" CommandArgument='<%# Eval("ID_SONG") %>'
                            OnClientClick="return confirm('Are you sure you want to delete this song?');"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    <div>
        <fieldset>
            <legend>Player</legend>
            <asp:Literal ID="litPlayer" runat="server"></asp:Literal>
        </fieldset>
    </div>
    <asp:Panel ID="pnlAdmin" Visible="false" runat="server">
        <div>
            <fieldset>
                <legend>Register / edit song</legend>
                <asp:HiddenField ID="hfIdSong" runat="server" />
                <asp:HiddenField ID="hfSongPath" runat="server" />
                <table>
                    <tr>
                        <td>Song name*:<br /><asp:TextBox ID="txtSongName" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Song file (mp3):<br /><asp:FileUpload ID="fupSong" runat="server" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnSaveSong" runat="server" Text="Save" OnClick="btnSaveSong_Click" />
                            <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" Visible="false" OnClick="btnCancelEdit_Click" />
                        </td>
                    </tr>
                </table>
            </fieldset>
        </div>
    </asp:Panel>
    <div>
        <asp:Panel ID="frmConfirmation" Visible="true" Runat="server">
            <asp:Label id="lblMessage" ForeColor="Red" Font-Bold="true" Runat="server"></asp:Label>
        </asp:Panel>
    </div>
    <div>
        <asp:HyperLink ID="HyperLinkDefault" NavigateUrl="~/Default.aspx" runat="server">Home</asp:HyperLink>
        &nbsp;
        <asp:HyperLink ID="HyperLinkLogout" NavigateUrl="~/Logout.aspx" runat="server">Logout</asp:HyperLink>
    </div>
</form>
</body>
</html>
