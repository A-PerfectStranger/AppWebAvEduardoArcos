using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppPoliMusicV2
{
    public partial class Song : System.Web.UI.Page
    {
        string strConnString = ConfigurationManager.ConnectionStrings["BDD_PoliMusicConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lblMessage.Text = string.Empty;
            }
            pnlAdmin.Visible = IsAdmin();
            ApplyFilter();
        }

        private bool IsAdmin()
        {
            if (Session[Util.Constants.USER] == null)
            {
                return false;
            }
            Entities.User user = (Entities.User)Session[Util.Constants.USER];
            return user.UserType == Util.Constants.ID_USER_ADMIN;
        }

        private void ApplyFilter()
        {
            string filterType = Convert.ToString(ViewState["FILTER_TYPE"]);
            string filterValue = Convert.ToString(ViewState["FILTER_VALUE"]);
            BLL.Song bllSong = new BLL.Song(strConnString);

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
            btnSaveSong.Text = "Save";
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
                lblMessage.Text = "Only administrators can edit or delete songs!.";
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
            new BLL.Song(strConnString).IncrementPlays(idSong);
            DataTable dtSong = new BLL.Song(strConnString).ReadById(idSong);
            if (dtSong.Rows.Count == 0)
            {
                lblMessage.Text = "The selected song does not exist!.";
                return;
            }
            string songPath = dtSong.Rows[0]["SONG_PATH"].ToString();
            litPlayer.Text = "<audio controls=\"controls\" autoplay=\"autoplay\" src=\"" +
                Server.HtmlEncode(ResolveUrl(songPath)) + "\"></audio>";
            lblMessage.Text = "Playing: " + dtSong.Rows[0]["SONG_NAME"].ToString();
            ApplyFilter();
        }

        private void LoadSongForEdit(int idSong)
        {
            DataTable dtSong = new BLL.Song(strConnString).ReadById(idSong);
            if (dtSong.Rows.Count == 0)
            {
                lblMessage.Text = "The selected song does not exist!.";
                return;
            }
            hfIdSong.Value = dtSong.Rows[0]["ID_SONG"].ToString();
            hfSongPath.Value = dtSong.Rows[0]["SONG_PATH"].ToString();
            txtSongName.Text = dtSong.Rows[0]["SONG_NAME"].ToString();
            btnSaveSong.Text = "Update";
            btnCancelEdit.Visible = true;
            lblMessage.Text = "Editing song: " + txtSongName.Text;
            ApplyFilter();
        }

        private void DeleteSong(int idSong)
        {
            try
            {
                new BLL.Song(strConnString).Delete(idSong);
                lblMessage.Text = "Song deleted successfully!.";
                ResetForm();
            }
            catch (SqlException)
            {
                lblMessage.Text = "The song cannot be deleted because it is associated with an album, an artist or a playlist!.";
            }
            ApplyFilter();
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ResetForm();
            lblMessage.Text = string.Empty;
            ApplyFilter();
        }

        protected void btnSaveSong_Click(object sender, EventArgs e)
        {
            if (!IsAdmin())
            {
                lblMessage.Text = "Only administrators can register songs!.";
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
                lblMessage.Text = "Song name is required!";
                return;
            }
            string newFileName = songName.Replace(" ", string.Empty);
            if (fupSong.HasFile)
            {
                string messageUpload = new FileManagement().SaveSongOnServer(fupSong, Util.Constants.SONG_FOLDER, newFileName);
                if (messageUpload != string.Empty)
                {
                    lblMessage.Text = messageUpload;
                    return;
                }
                songPath = Util.Constants.SONG_FOLDER + newFileName + Path.GetExtension(fupSong.FileName).ToLower();
            }
            if (songPath == string.Empty)
            {
                lblMessage.Text = "Song file is required!";
                return;
            }

            Entities.Song song = new Entities.Song();
            song.ID = idSong;
            song.Name = songName;
            song.Path = songPath;
            song.Plays = GetPlays(idSong);

            if (idSong == 0)
            {
                string messageValidations = new BLL.Song(strConnString).ValidationsDuplicated(song);
                if (messageValidations != string.Empty)
                {
                    lblMessage.Text = messageValidations;
                    return;
                }
                new BLL.Song(strConnString).Save(song);
                lblMessage.Text = "Song registered successfully!.";
            }
            else
            {
                new BLL.Song(strConnString).Update(song);
                lblMessage.Text = "Song updated successfully!.";
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
            DataTable dtSong = new BLL.Song(strConnString).ReadById(idSong);
            if (dtSong.Rows.Count == 0 || dtSong.Rows[0]["PLAYS"] == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToInt32(dtSong.Rows[0]["PLAYS"]);
        }
    }

}
