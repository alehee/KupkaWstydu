using SQLite;

namespace KW_XamarinForms
{
    public class DT_Arrear
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public short categoryId { get; set; }
        public short categoryPosition { get; set; }
        public short status { get; set; }
        // 0 - nie zrobione, 1 - zrobione
        public string name { get; set; }
    }

}