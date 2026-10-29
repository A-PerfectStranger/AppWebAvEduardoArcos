using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppPoliMusicV3
{
    public partial class Song : System.Web.UI.Page
    {
        private const string SONG_FOLDER = "~/SongFiles/";
        string strConnString = ConfigurationManager.ConnectionStrings["BDD_PoliMusicConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                litPlayer.Text = "<p class=\"text-muted mb-0\">Selecciona una canción y pulsa «Reproducir» para escucharla.</p>";
            }
            pnlAdmin.Visible = IsAdmin();
            ApplyFilter();
        }

        private bool IsAdmin()
        {
            if (Session[EntityLayer.PoliMusicV3.Util.Constants.USER] == null)
            {
                return false;
            }
            EntityLayer.PoliMusicV3.User user =
                (EntityLayer.PoliMusicV3.User)Session[EntityLayer.PoliMusicV3.Util.Constants.USER];
            return user.UserType == EntityLayer.PoliMusicV3.Util.Constants.ID_USER_ADMIN;
        }

        private void ShowMessage(string message, bool isError)
        {
            lblMessage.Text = message;
            pnlMessage.Visible = message != string.Empty;
            pnlMessage.CssClass = isError ? "alert alert-danger" : "alert alert-success";
        }

        private void ApplyFilter()
        {
            string filterType = Convert.ToString(ViewState["FILTER_TYPE"]);
            string filterValue = Convert.ToString(ViewState["FILTER_VALUE"]);
            BLL.PoliMusicV3.Song bllSong = new BLL.PoliMusicV3.Song(strConnString);

            if (filterType == "NAME")
            {
                gvSongs.DataSource = bllSong.ReadByName(filterValue);
            }
            else if (filterType == "TOP")
            {
                gvSongs.DataSource = bllSong.ReadMostPlayed(10);
            }
            else
            {
                gvSongs.DataSource = bllSong.Read();
            }
            gvSongs.DataBind();
        }

        private void ResetForm()
        {
            hfIdSong.Value = string.Empty;
            hfSongPath.Value = string.Empty;
            txtSongName.Text = string.Empty;
            btnSaveSong.Text = "Guardar";
            btnCancelEdit.Visible = false;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ViewState["FILTER_TYPE"] = "NAME";
            ViewState["FILTER_VALUE"] = txtSearch.Text.Trim();
            ResetForm();
            ApplyFilter();
        }

        protected void btnShowAll_Click(object sender, EventArgs e)
        {
            ViewState["FILTER_TYPE"] = "ALL";
            ViewState["FILTER_VALUE"] = string.Empty;
            txtSearch.Text = string.Empty;
            ResetForm();
            ApplyFilter();
        }

        protected void btnMostPlayed_Click(object sender, EventArgs e)
        {
            ViewState["FILTER_TYPE"] = "TOP";
            ViewState["FILTER_VALUE"] = string.Empty;
            ResetForm();
            ApplyFilter();
        }

        protected void gvSongs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSongs.PageIndex = e.NewPageIndex;
            ApplyFilter();
        }

        protected void gvSongs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idSong = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "PlaySong")
            {
                PlaySong(idSong);
                return;
            }
            if (!IsAdmin())
            {
                ShowMessage("Solo los administradores pueden editar o eliminar canciones.", true);
                return;
            }
            if (e.CommandName == "EditSong")
            {
                LoadSongForEdit(idSong);
            }
            else if (e.CommandName == "DeleteSong")
            {
                DeleteSong(idSong);
            }
        }

        private void PlaySong(int idSong)
        {
            BLL.PoliMusicV3.Song bllSong = new BLL.PoliMusicV3.Song(strConnString);
            bllSong.IncrementPlays(idSong);
            DataTable dtSong = bllSong.ReadById(idSong);
            if (dtSong.Rows.Count == 0)
            {
                ShowMessage("La canción seleccionada no existe.", true);
                return;
            }
            string songPath = dtSong.Rows[0]["SONG_PATH"].ToString();
            litPlayer.Text = "<audio controls=\"controls\" autoplay=\"autoplay\" class=\"w-100\" src=\"" +
                Server.HtmlEncode(ResolveUrl(songPath)) + "\"></audio>";
            ShowMessage("Reproduciendo: " + dtSong.Rows[0]["SONG_NAME"].ToString(), false);
            ApplyFilter();
        }

        private void LoadSongForEdit(int idSong)
        {
            DataTable dtSong = new BLL.PoliMusicV3.Song(strConnString).ReadById(idSong);
            if (dtSong.Rows.Count == 0)
            {
                ShowMessage("La canción seleccionada no existe.", true);
                return;
            }
            hfIdSong.Value = dtSong.Rows[0]["ID_SONG"].ToString();
            hfSongPath.Value = dtSong.Rows[0]["SONG_PATH"].ToString();
            txtSongName.Text = dtSong.Rows[0]["SONG_NAME"].ToString();
            btnSaveSong.Text = "Actualizar";
            btnCancelEdit.Visible = true;
            ShowMessage("Editando la canción: " + txtSongName.Text, false);
            ApplyFilter();
        }

        private void DeleteSong(int idSong)
        {
            try
            {
                new BLL.PoliMusicV3.Song(strConnString).Delete(idSong);
                ShowMessage("¡Canción eliminada correctamente!", false);
                ResetForm();
            }
            catch (SqlException)
            {
                ShowMessage("No se puede eliminar la canción porque está asociada a un álbum, un artista o una lista de reproducción.", true);
            }
            ApplyFilter();
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ResetForm();
            ShowMessage(string.Empty, false);
            ApplyFilter();
        }

        protected void btnSaveSong_Click(object sender, EventArgs e)
        {
            if (!IsAdmin())
            {
                ShowMessage("Solo los administradores pueden registrar canciones.", true);
                return;
            }
            string songName = txtSongName.Text.Trim();
            string songPath = hfSongPath.Value;
            int idSong = 0;
            if (hfIdSong.Value != string.Empty)
            {
                idSong = Convert.ToInt32(hfIdSong.Value);
            }
            if (songName == string.Empty)
            {
                ShowMessage("¡El nombre de la canción es obligatorio!", true);
                return;
            }
            string newFileName = songName.Replace(" ", string.Empty);
            if (fupSong.HasFile)
            {
                string messageUpload = new FileManagement().SaveSongOnServer(fupSong, SONG_FOLDER, newFileName);
                if (messageUpload != string.Empty)
                {
                    ShowMessage(messageUpload, true);
                    return;
                }
                songPath = SONG_FOLDER + newFileName + Path.GetExtension(fupSong.FileName).ToLower();
            }
            if (songPath == string.Empty)
            {
                ShowMessage("¡El archivo de la canción es obligatorio!", true);
                return;
            }

            EntityLayer.PoliMusicV3.Song song = new EntityLayer.PoliMusicV3.Song();
            song.ID = idSong;
            song.Name = songName;
            song.Path = songPath;
            song.Plays = GetPlays(idSong);

            if (idSong == 0)
            {
                string messageValidations = new BLL.PoliMusicV3.Song(strConnString).ValidationsDuplicated(song);
                if (messageValidations != string.Empty)
                {
                    ShowMessage(messageValidations, true);
                    return;
                }
                new BLL.PoliMusicV3.Song(strConnString).Save(song);
                ShowMessage("¡Canción registrada correctamente!", false);
            }
            else
            {
                new BLL.PoliMusicV3.Song(strConnString).Update(song);
                ShowMessage("¡Canción actualizada correctamente!", false);
            }
            ResetForm();
            ApplyFilter();
        }

        private int GetPlays(int idSong)
        {
            if (idSong == 0)
            {
                return 0;
            }
            DataTable dtSong = new BLL.PoliMusicV3.Song(strConnString).ReadById(idSong);
            if (dtSong.Rows.Count == 0 || dtSong.Rows[0]["PLAYS"] == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToInt32(dtSong.Rows[0]["PLAYS"]);
        }
    }
}
