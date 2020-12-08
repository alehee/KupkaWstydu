using SQLite;

namespace KW_XamarinForms
{
    public class DT_Category
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public short iconId { get; set; }
        public string title { get; set; }
        public bool hidden { get; set; }
    }
    
}
