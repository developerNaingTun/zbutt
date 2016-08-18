using System;
using System.Collections.Generic;
using System.Text;

namespace formatConvert
{
    /// <summary>
    /// 
    /// </summary>
    public class myItem
    {
        private String _itemName;
        private String _itemExtension;
        private Object _itemWord;

        public String ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }

        public String ItemExtension
        {
            get { return _itemExtension; }
            set { _itemExtension = value; }
        }

        public Object ItemWord
        {
            get { return _itemWord; }
            set { _itemWord = value; }
        }

        public override String ToString()
        {
            return ItemName.ToString();
        }

        public myItem()
        { }

        public myItem(String inName, String inExtension, Object inWordType)
        {
            ItemName = inName;
            ItemExtension = inExtension;
            ItemWord = inWordType;
        }

    }
}
