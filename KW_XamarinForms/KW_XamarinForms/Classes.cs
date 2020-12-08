using System;
using System.Collections.Generic;
using System.Text;

namespace KW_XamarinForms
{
    public class Category
    {
        public short id;
        public short iconId;
        public string name;
        public bool hidden;
        public List<Arrear> arrears = new List<Arrear>();

        public Category(short TId, short icoId, string nam, bool hid)
        {
            id = TId;
            iconId = icoId;
            name = nam;
            hidden = hid;
        }
    }

    public class Arrear
    {
        public short id;
        public short categoryId;
        public short categoryPosition;
        public short status;
        public string name;
        public Object completeImage;
        public Object deleteImage;

        public Arrear(short Tid, short catId, short catPos, short sta, string nam)
        {
            id = Tid;
            categoryId = catId;
            categoryPosition = catPos;
            status = sta;
            name = nam;
        }


    }
}
