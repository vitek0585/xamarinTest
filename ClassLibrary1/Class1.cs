using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Database;
using Android.Provider;

namespace ClassLibrary1
{
    public class Class1
    {
        public void MyFunction(ContentResolver contentResolver)
        {
            var uri = ContactsContract.Contacts.ContentUri;//CommonDataKinds.Phone.ContentUri;
            string[] projection = new string[] {
            //ContactsContract.CommonDataKinds.Phone.SearchDisplayNameKey,
                ContactsContract.Contacts.InterfaceConsts.Count};
            ICursor people = contentResolver.Query(uri, projection, null, null, null);

            //int indexName = people.GetColumnIndex(projection[0]);
            int count = people.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.Count);
           
        }

     

    }
}
