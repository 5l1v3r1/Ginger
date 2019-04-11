﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginger.UserControlsLib.UCListView
{
    public interface IListViewItemInfo 
    {
        void SetItem(object item);

        string GetItemNameField();

        string GetItemDescriptionField();

        string GetItemIconField();

        string GetItemExecutionStatusField();

        List<ListItemNotification> GetNotificationsList();
    }
}
