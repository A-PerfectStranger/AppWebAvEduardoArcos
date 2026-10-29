<%@ Page Title="Canciones" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Song.aspx.cs" Inherits="WebAppPoliMusicV3.Song" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <div class="d-flex flex-wrap justify-content-between align-items-center mb-3">
            <div>
                <h2 id="title" class="mb-1">Canciones</h2>
                <p class="text-muted mb-0">Escucha, busca y administra las canciones de PoliMusic.</p>
            </div>
        </div>

        <%-- Buscador --%>
        <div class="card shadow-sm mb-3">
            <div class="card-body">
                <div class="row g-2 align-items-end">
                    <div class="col-12 col-md-5">
                        <label class="form-label" for="<%= txtSearch.ClientID %>">Nombre de la canción</label>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                            placeholder="Ej. Adventure" MaxLength="50"></asp:TextBox>
                    </div>
                    <div class="col-12 col-md-7">
                        <asp:Button ID="btnSearch" runat="server" Text="Buscar" CssClass="btn btn-primary"
                            OnClick="btnSearch_Click" />
                        <asp:Button ID="btnShowAll" runat="server" Text="Ver todas" CssClass="btn btn-outline-secondary"
                            OnClick="btnShowAll_Click" />
                        <asp:Button ID="btnMostPlayed" runat="server" Text="Más reproducidas" CssClass="btn btn-outline-dark"
                            OnClick="btnMostPlayed_Click" />
                    </div>
                </div>
            </div>
        </div>

        <%-- Mensajes --%>
        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-info" role="alert">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </asp:Panel>

        <%-- Listado de canciones --%>
        <div class="card shadow-sm mb-3">
            <div class="card-header bg-dark text-white">
                Listado de canciones
            </div>
            <div class="card-body p-0">
                <asp:GridView ID="gvSongs" runat="server" AutoGenerateColumns="false"
                    EmptyDataText="No hay canciones registradas." CssClass="table table-striped table-hover align-middle mb-0"
                    AllowPaging="true" PageSize="10" OnPageIndexChanging="gvSongs_PageIndexChanging"
                    OnRowCommand="gvSongs_RowCommand" CellPadding="4" GridLines="None">
                    <HeaderStyle CssClass="table-dark" />
                    <PagerSettings Mode="NumericFirstLast" FirstPageText="Primera" LastPageText="Última"
                        PageButtonCount="5" />
                    <PagerStyle CssClass="p-2" HorizontalAlign="Center" />
                    <Columns>
                        <asp:BoundField DataField="ID_SONG" HeaderText="ID" ItemStyle-Width="60px" />
                        <asp:BoundField DataField="SONG_NAME" HeaderText="Canción" />
                        <asp:BoundField DataField="SONG_PATH" HeaderText="Ruta" />
                        <asp:BoundField DataField="PLAYS" HeaderText="Reproducciones" ItemStyle-Width="130px" />
                        <asp:TemplateField HeaderText="Acciones" ItemStyle-Width="220px">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkPlay" runat="server" Text="Reproducir" CausesValidation="false"
                                    CssClass="btn btn-sm btn-outline-primary"
                                    CommandName="PlaySong" CommandArgument='<%# Eval("ID_SONG") %>'></asp:LinkButton>
                                <asp:LinkButton ID="lnkEdit" runat="server" Text="Editar" CausesValidation="false"
                                    CssClass="btn btn-sm btn-outline-secondary"
                                    CommandName="EditSong" CommandArgument='<%# Eval("ID_SONG") %>'></asp:LinkButton>
                                <asp:LinkButton ID="lnkDelete" runat="server" Text="Eliminar" CausesValidation="false"
                                    CssClass="btn btn-sm btn-outline-danger"
                                    CommandName="DeleteSong" CommandArgument='<%# Eval("ID_SONG") %>'
                                    OnClientClick="return confirm('¿Está seguro de que desea eliminar esta canción?');"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <%-- Reproductor --%>
        <div class="card shadow-sm mb-3">
            <div class="card-header bg-dark text-white">
                Reproductor
            </div>
            <div class="card-body">
                <asp:Literal ID="litPlayer" runat="server"></asp:Literal>
            </div>
        </div>

        <%-- Registro / edición (solo administradores) --%>
        <asp:Panel ID="pnlAdmin" runat="server" Visible="false">
            <div class="card shadow-sm mb-3">
                <div class="card-header bg-dark text-white">
                    Registrar / editar canción
                </div>
                <div class="card-body">
                    <asp:HiddenField ID="hfIdSong" runat="server" />
                    <asp:HiddenField ID="hfSongPath" runat="server" />
                    <div class="row g-2">
                        <div class="col-12 col-md-6">
                            <label class="form-label" for="<%= txtSongName.ClientID %>">Nombre de la canción *</label>
                            <asp:TextBox ID="txtSongName" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                        </div>
                        <div class="col-12 col-md-6">
                            <label class="form-label" for="<%= fupSong.ClientID %>">Archivo de la canción (MP3)</label>
                            <asp:FileUpload ID="fupSong" runat="server" CssClass="form-control" />
                            <div class="form-text">Formatos permitidos: .mp3</div>
                        </div>
                    </div>
                    <div class="mt-3">
                        <asp:Button ID="btnSaveSong" runat="server" Text="Guardar" CssClass="btn btn-success"
                            OnClick="btnSaveSong_Click" />
                        <asp:Button ID="btnCancelEdit" runat="server" Text="Cancelar" CssClass="btn btn-outline-secondary"
                            Visible="false" OnClick="btnCancelEdit_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </main>
</asp:Content>
