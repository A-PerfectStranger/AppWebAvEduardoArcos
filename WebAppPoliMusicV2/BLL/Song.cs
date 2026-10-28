using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace WebAppPoliMusicV2.BLL
{
    public class Song
    {
        private string strConnectionString;
        public Song(string strConnString)
        {
            strConnectionString = strConnString;
        }
        public int Save(Entities.Song song)
        {
            return new DAL.Song(strConnectionString).Save(song);
        }
        public int SaveAndReturnId(Entities.Song song)
        {
            return new DAL.Song(strConnectionString).SaveAndReturnId(song);
        }
        public DataTable Read()
        {
            return new DAL.Song(strConnectionString).Read();
        }
        public DataTable ReadById(int id)
        {
            return new DAL.Song(strConnectionString).ReadById(id);
        }
        public DataTable ReadByName(string songName)
        {
            return new DAL.Song(strConnectionString).ReadByName(songName);
        }
        public DataTable ReadMostPlayed(int top)
        {
            return new DAL.Song(strConnectionString).ReadMostPlayed(top);
        }
        public int Update(Entities.Song song)
        {
            return new DAL.Song(strConnectionString).Update(song);
        }
        public int IncrementPlays(int id)
        {
            return new DAL.Song(strConnectionString).IncrementPlays(id);
        }
        public int Delete(int id)
        {
            return new DAL.Song(strConnectionString).Delete(id);
        }
        public int CheckExistSong(string songPath)
        {
            return new DAL.Song(strConnectionString).CheckExistSong(songPath);
        }
        public string ValidationsDuplicated(Entities.Song song)
        {
            int returnValue = CheckExistSong(song.Path);
            if (returnValue != 0)
            {
                return "Song already registered!";
            }
            return string.Empty;
        }
    }

}
